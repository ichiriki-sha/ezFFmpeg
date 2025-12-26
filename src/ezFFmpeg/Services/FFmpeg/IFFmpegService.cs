using ezFFmpeg.Models.Common;
using ezFFmpeg.Services.FFmpeg.EncoderCapabilityProvider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ezFFmpeg.Services.FFmpeg
{
    /// <summary>
    /// FFmpeg および FFprobe の操作を抽象化するインターフェイス。
    /// エンコード、メディア情報取得、エンコーダ情報の取得などを提供します。
    /// </summary>
    public interface IFFmpegService
    {
        /// <summary>
        /// FFmpeg を使用して非同期にメディアをエンコードします。
        /// エンコード進捗は IProgress&lt;double&gt; で取得可能です。
        /// </summary>
        /// <param name="arguments">FFmpeg 実行引数</param>
        /// <param name="durationMs">対象メディアの再生時間（ミリ秒）</param>
        /// <param name="progress">エンコード進捗を報告する IProgress&lt;double&gt;</param>
        /// <param name="logProgress">エンコードログを報告する </param>
        /// <param name="token">キャンセルトークン</param>
        /// <returns>エンコード完了時に終了コードを返すタスク</returns>
        //Task<int> RunEncodeAsync(
        //    string arguments,
        //    double durationMs,
        //    IProgress<double> progress,
        //    FileItem item,
        //    CancellationToken token);

        Task<int> RunEncodeAsync(
            string arguments,
            double durationMs,
            IProgress<double> progress,
            IProgress<string>? logProgress,
            CancellationToken token);

        /// <summary>
        /// 指定した FFmpeg 引数でプロセスを同期実行します。
        /// 標準出力・標準エラーの結果を <see cref="ProcessResult"/> で取得できます。
        /// </summary>
        /// <param name="arguments">FFmpeg 実行引数</param>
        /// <returns>プロセス実行結果</returns>
        ProcessResult RunFFmpeg(string arguments);

        /// <summary>
        /// 指定した FFprobe 引数でプロセスを同期実行します。
        /// 標準出力・標準エラーの結果を <see cref="ProcessResult"/> で取得できます。
        /// </summary>
        /// <param name="arguments">FFprobe 実行引数</param>
        /// <returns>プロセス実行結果</returns>
        ProcessResult RunFFprobe(string arguments);

        /// <summary>
        /// 利用可能な全てのエンコーダ情報を取得します。
        /// </summary>
        /// <returns>エンコーダ情報の読み取り専用コレクション</returns>
        IReadOnlyCollection<EncoderInfo> GetAvailableEncoders();

        /// <summary>
        /// 指定された名前のエンコーダ情報を取得します。
        /// </summary>
        /// <param name="name">エンコーダ名</param>
        /// <returns>エンコーダ情報、存在しない場合は null</returns>
        EncoderInfo? GetEncoder(string name);

        /// <summary>
        /// 指定したエンコーダを使用可能かどうか判定します。
        /// </summary>
        /// <param name="encoderName">エンコーダ名</param>
        /// <returns>使用可能なら true</returns>
        bool CanUseEncoder(string encoderName);

        /// <summary>
        /// エンコーダが利用可能かどうかをチェックします。
        /// audio フラグを true にすると音声用エンコーダとしてチェックします。
        /// </summary>
        /// <param name="encoderName">エンコーダ名</param>
        /// <param name="isAudio">音声用エンコーダとしてチェックするか</param>
        /// <returns>利用可能なら true</returns>
        bool IsEncoderAvailable(string encoderName, bool isAudio = false);
    }
}
