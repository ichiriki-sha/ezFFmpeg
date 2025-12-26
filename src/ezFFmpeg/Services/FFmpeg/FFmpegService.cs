using ezFFmpeg.Models.Common;
using ezFFmpeg.Services.FFmpeg.EncoderCapabilityProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace ezFFmpeg.Services.FFmpeg
{
    /// <summary>
    /// FFmpeg および FFprobe の操作を行うサービスクラス。
    /// エンコード、エンコーダ情報取得、進捗管理などの機能を提供します。
    /// </summary>
    public sealed class FFmpegService : IFFmpegService
    {
        private EncoderCapabilities? _encoderCapabilities;

        private string? _ffmpegFolderPath = null;

        /// <summary>
        /// FFmpeg および FFprobe が配置されたフォルダパス。
        /// 設定時に存在チェックを行い、エンコーダ情報をロードします。
        /// </summary>
        public string? FFmpegFolderPath
        {
            get => _ffmpegFolderPath;
            set
            {
                if (_ffmpegFolderPath == value) return;

                _ffmpegFolderPath = value;

                if (_ffmpegFolderPath != null)
                {
                    _ffmpegPath = Path.Combine(_ffmpegFolderPath, FFmpegBinaries.FFmpeg);
                    _ffprobePath = Path.Combine(_ffmpegFolderPath, FFmpegBinaries.FFprobe);

                    if (!File.Exists(_ffmpegPath))
                        throw new ArgumentException("FFmpegが見つかりません。", _ffmpegPath);
                    if (!File.Exists(_ffprobePath))
                        throw new ArgumentException("FFprobeが見つかりません。", _ffprobePath);

                    FFmpegVersion = GetFFmpegVersion();
                    FFprobeVersion = GetFFprobeVersion();

                    if (string.IsNullOrEmpty(FFmpegVersion))
                        throw new ArgumentException("FFmpeg のバージョン情報を取得できません。", nameof(_ffmpegPath));

                    if (string.IsNullOrEmpty(FFprobeVersion))
                        throw new ArgumentException("FFprobe のバージョン情報を取得できません。", nameof(_ffprobePath));

                    _encoderCapabilities = new EncoderCapabilities(this);
                    _encoderCapabilities.Load();
                }
            }
        }

        /// <summary>
        /// FFmpeg のバージョン文字列（キャッシュ）
        /// </summary>
        public string FFmpegVersion { get; private set; } = string.Empty;

        /// <summary>
        /// FFprobe のバージョン文字列（キャッシュ）
        /// </summary>
        public string FFprobeVersion { get; private set; } = string.Empty;


        private string? _ffmpegPath = null;
        private string? _ffprobePath = null;

        /// <summary>
        /// FFmpegService のコンストラクタ。
        /// 指定されたフォルダパスを元に FFmpeg/FFprobe のパスを初期化します。
        /// </summary>
        /// <param name="ffmpegFolderPath">FFmpeg が配置されているフォルダパス</param>
        public FFmpegService(string ffmpegFolderPath)
        {
            FFmpegFolderPath = ffmpegFolderPath;
        }

        /// <summary>
        /// FFmpeg を使用して非同期にメディアをエンコードします。
        /// 進捗は IProgress&lt;double&gt; で報告されます。
        /// キャンセル可能。
        /// </summary>
        public async Task<int> RunEncodeAsync(
            string arguments,
            double durationMs,
            IProgress<double> progress,
            IProgress<string>? logProgress,
            CancellationToken token)
        {
            int result = -1;
            var psi = new ProcessStartInfo
            {
                FileName = _ffmpegPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardErrorEncoding = Encoding.UTF8
            };

            using var process = new Process { StartInfo = psi };

            // UIにログ追加
            logProgress?.Report($"{FFmpegBinaries.FFmpeg} {arguments}");

            process.Start();

            try
            {
                while (!process.HasExited)
                {
                    token.ThrowIfCancellationRequested();

                    var line = await process.StandardError.ReadLineAsync(token);
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // UIにログ追加
                    logProgress?.Report(line);

                    ParseProgress(line, durationMs, progress);
                }

                await process.WaitForExitAsync(token);
                result = process.ExitCode;
            }
            catch (OperationCanceledException)
            {
                try { if (!process.HasExited) process.Kill(true); } catch { }
                throw;  // キャンセル例外を上位に伝える
            }

            return result;
        }

        /// <summary>
        /// FFmpeg を同期実行し、標準出力・標準エラーを取得します。
        /// </summary>
        public ProcessResult RunFFmpeg(string arguments)
            => RunProcess(_ffmpegPath!, arguments);

        /// <summary>
        /// FFprobe を同期実行し、標準出力・標準エラーを取得します。
        /// </summary>
        public ProcessResult RunFFprobe(string arguments)
            => RunProcess(_ffprobePath!, arguments);

        /// <summary>
        /// 任意のプロセスを同期実行して標準出力・標準エラーを取得します。
        /// </summary>
        private static ProcessResult RunProcess(string exe, string args)
        {
            ProcessResult result;
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = exe,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                using var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
                var stdOut = new StringBuilder();
                var stdErr = new StringBuilder();

                process.OutputDataReceived += (s, e) => { if (e.Data != null) stdOut.AppendLine(e.Data); };
                process.ErrorDataReceived += (s, e) => { if (e.Data != null) stdErr.AppendLine(e.Data); };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                result = new ProcessResult(stdOut.ToString(), stdErr.ToString(), process.ExitCode);
            }
            catch (Exception ex)
            {
                result = new ProcessResult(null, ex.Message, -1);
            }

            return result;
        }

        /// <summary>
        /// FFmpeg 標準エラー出力から進捗情報を解析して IProgress に報告します。
        /// </summary>
        private static void ParseProgress(
            string line,
            double durationMs,
            IProgress<double> progress)
        {
            var match = Regex.Match(line, @"time=(\d{2}):(\d{2}):(\d{2}\.\d+)");
            if (!match.Success) return;

            int hh = int.Parse(match.Groups[1].Value);
            int mm = int.Parse(match.Groups[2].Value);
            double ss = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

            double elapsedMs = ((hh * 3600) + (mm * 60) + ss) * 1000;
            double percent = Math.Min(elapsedMs / durationMs * 100, 100);

            progress.Report(percent);
        }

        /// <summary>
        /// FFmpeg のバージョン情報を取得します。
        /// </summary>
        private string GetFFmpegVersion()
        {
            string version = "";
            try
            {
                string args = FFmpegArgumentBuilder.BuildVersionArguments();
                ProcessResult ret = RunFFmpeg(args);

                if (ret.Success)
                {
                    foreach (var line in ret.StdOut!.Split(Environment.NewLine))
                    {
                        var match = Regex.Match(line, @"(ffmpeg\s+version.+)");
                        if (!match.Success) continue;
                        version = match.Groups[1].Value;
                        break;
                    }
                }
            }
            catch { }

            return version;
        }

        /// <summary>
        /// FFprobe のバージョン情報を取得します。
        /// </summary>
        private string GetFFprobeVersion()
        {
            string version = "";
            try
            {
                string args = FFprobeArgumentBuilder.BuildVersionArguments();
                ProcessResult ret = RunFFprobe(args);

                if (ret.Success)
                {
                    foreach (var line in ret.StdOut!.Split(Environment.NewLine))
                    {
                        var match = Regex.Match(line, @"(ffprobe\s+version.+)");
                        if (!match.Success) continue;
                        version = match.Groups[1].Value;
                        break;
                    }
                }
            }
            catch { }

            return version;
        }

        /// <summary>
        /// 利用可能な全エンコーダ情報を取得します。
        /// </summary>
        public IReadOnlyCollection<EncoderInfo> GetAvailableEncoders()
        {
            if (_encoderCapabilities == null)
                throw new InvalidOperationException("FFmpegのパスが設定されていません。");
            return _encoderCapabilities.GetAll();
        }

        /// <summary>
        /// 指定した名前のエンコーダ情報を取得します。
        /// </summary>
        public EncoderInfo? GetEncoder(string name)
        {
            if (_encoderCapabilities == null)
                throw new InvalidOperationException("FFmpegのパスが設定されていません。");
            return _encoderCapabilities.Get(name);
        }

        /// <summary>
        /// 指定したエンコーダが使用可能かどうかを判定します。
        /// </summary>
        public bool CanUseEncoder(string encoderName)
        {
            if (_encoderCapabilities == null)
                throw new InvalidOperationException("FFmpegのパスが設定されていません。");
            return _encoderCapabilities.CanUse(encoderName);
        }

        /// <summary>
        /// 指定したエンコーダが利用可能かをテストします。
        /// 音声用エンコーダなら isAudio を true に設定します。
        /// </summary>
        public bool IsEncoderAvailable(string encoder, bool isAudio = false)
        {
            string args = isAudio
                ? FFmpegArgumentBuilder.BuildAudioTestArguments(encoder)
                : FFmpegArgumentBuilder.BuildVideoTestArguments(encoder);

            try
            {
                ProcessResult ret = RunFFmpeg(args);
                return ret.Success;
            }
            catch
            {
                return false;
            }
        }
    }
}
