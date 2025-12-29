using ezFFmpeg.Common;
using ezFFmpeg.Models.Common;
using ezFFmpeg.Models.Conversion;
using ezFFmpeg.Models.Output;
using ezFFmpeg.Services.FFmpeg;
using ezFFmpeg.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace ezFFmpeg.Services.Conversion
{
    /// <summary>
    /// 変換処理全体を統括するサービス。
    /// <para>
    /// ConversionContext に含まれる複数のファイルを対象に、
    /// 並列数制御・進捗通知・キャンセル対応を行いながら
    /// FFmpeg によるエンコード処理を実行する。
    /// </para>
    /// </summary>
    internal sealed class ConversionService : IConversionService
    {
        /// <summary>
        /// 変換処理を非同期で実行する。
        /// </summary>
        /// <param name="context">
        /// 変換対象ファイル、設定、キャンセルトークンをまとめたコンテキスト
        /// </param>
        /// <param name="progress">
        /// 全体およびファイル単位の進捗を通知するための Progress
        /// </param>
        public async Task ExecuteAsync(
            ConversionContext context,
            IProgress<ConversionProgress> progress)
        {
            var files = context.Files;
            var settings = context.Settings;
            var token = context.Token;

            try
            {
                // 並列実行数を制限するためのセマフォ
                // ParallelCount が 0 以下にならないよう最低 1 に丸める
                var semaphore = new SemaphoreSlim(
                    Math.Max(1, settings.ParallelCount));

                // 各ファイルごとに並列エンコードタスクを作成
                var tasks = files.Select((file, index) =>
                    Task.Run(async () =>
                    {
                        // セマフォ取得（キャンセル対応）
                        await semaphore.WaitAsync(token);
                        try
                        {
                            await EncodeSingleAsync(
                                file,
                                index,
                                files,
                                settings,
                                progress,
                                token);
                        }
                        finally
                        {
                            // 必ずセマフォを解放する
                            semaphore.Release();
                        }
                    }, token));

                // すべてのファイルの処理が完了するまで待機
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                // キャンセル時の個別処理は finally 側で行う
            }
            finally
            {
                // キャンセル要求があった場合、
                // 未処理または処理中のファイルを Canceled 状態に更新する
                if (token.IsCancellationRequested)
                {
                    foreach (var file in files)
                    {
                        if (file.Status == ProcessingStatus.Pending ||
                            file.Status == ProcessingStatus.Processing)
                        {
                            await Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                file.Status = ProcessingStatus.Canceled;
                                file.Progress = 0;
                                file.ProcessingLogLines.Add(
                                    $"[Canceled] {DateTime.Now:yyyy-MM-dd HH:mm:ss} キャンセルされました。");
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 単一ファイルのエンコード処理を実行する。
        /// </summary>
        /// <param name="file">処理対象のファイル</param>
        /// <param name="workerIndex">並列処理上のインデックス</param>
        /// <param name="allFiles">全ファイル一覧（全体進捗計算用）</param>
        /// <param name="settings">アプリケーション設定</param>
        /// <param name="progress">全体進捗通知用 Progress</param>
        /// <param name="token">キャンセルトークン</param>
        private static async Task EncodeSingleAsync(
            FileItem file,
            int workerIndex,
            IReadOnlyCollection<FileItem> allFiles,
            AppSettings settings,
            IProgress<ConversionProgress> progress,
            CancellationToken token)
        {
            // 処理開始状態に更新
            file.Status = ProcessingStatus.Processing;
            file.Progress = 0;

            // 出力ファイル名を設定に基づいて生成
            var outputFileName =
                OutputFileNameGenerator.Generate(file, settings);

            // 出力先フルパスを構築
            var outputPath = Path.Combine(
                settings.CurrentProfile.OutputFolderPath,
                outputFileName);

            // FFmpeg の変換引数を生成
            var args = FFmpegArgumentBuilder.BuildConvertArguments(
                settings.CurrentProfile, file, outputPath);

            // 進捗計算用に動画の総再生時間（ミリ秒）を取得
            var durationMs = file.VideoDuration.TotalMilliseconds;

            // ファイル単位の進捗更新処理
            var fileProgress = new Progress<double>(p =>
            {
                file.Progress = p;

                // 全体進捗を計算して上位へ通知
                progress.Report(new ConversionProgress
                {
                    TotalPercent = CalculateTotalProgress(allFiles),
                    CurrentFileName = file.FileName,
                    CurrentIndex = workerIndex
                });
            });

            var logProgress = new Progress<string>(log =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    file.ProcessingLogLines.Add(log);
                });
            });

            try
            {
                // FFmpeg によるエンコード実行
                var ret = await settings.FFmpegService!
                    .RunEncodeAsync(args, durationMs, fileProgress, logProgress, token);

                // UI スレッドで完了状態を反映
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    file.Status = ret == 0
                        ? ProcessingStatus.Completed
                        : ProcessingStatus.Error;

                    file.IsChecked = file.Status != ProcessingStatus.Completed;
                    file.Progress = file.Status == ProcessingStatus.Completed ? 100 : 0;
                    file.ProcessingLogLines.Add(
                        $"[Completed] {DateTime.Now:yyyy-MM-dd HH:mm:ss} ExitCode={ret}");
                });
            }
            catch (OperationCanceledException)
            {
                // キャンセル時の状態更新は上位でまとめて行う
            }
            catch (Exception ex)
            {
                // 想定外エラー時は Error 状態として記録
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    file.Status = ProcessingStatus.Error;
                    file.Progress = 0;
                    file.ProcessingLogLines.Add(
                        $"[Error] {DateTime.Now:yyyy-MM-dd HH:mm:ss} {ex.Message}");
                });
            }
        }

        /// <summary>
        /// 全ファイルの平均進捗率を計算する。
        /// </summary>
        /// <param name="files">進捗を集計するファイル一覧</param>
        /// <returns>全体進捗率（0～100）</returns>
        private static double CalculateTotalProgress(
            IEnumerable<FileItem> files)
            => files.Any() ? files.Average(f => f.Progress) : 0;
    }
}
