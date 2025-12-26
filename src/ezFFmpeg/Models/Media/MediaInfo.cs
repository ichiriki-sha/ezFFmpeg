using ezFFmpeg.Common;
using ezFFmpeg.Models.Common;
using ezFFmpeg.Services.FFmpeg;
using System;
using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ezFFmpeg.Models.Media
{
    /// <summary>
    /// メディアファイルの情報を取得・管理するクラス。
    /// FFmpeg/FFprobe を使用して動画・音声ストリーム情報を解析し、
    /// サムネイル作成などの操作も提供する。
    /// </summary>
    public class MediaInfo
    {
        /// <summary>
        /// サポートされている拡張子の集合。
        /// ファイルの拡張子チェックに使用する。
        /// </summary>
        public static readonly HashSet<string> AllowedExtensions =
                           SupportedExtensions.Extensions
                               .Split(',', StringSplitOptions.RemoveEmptyEntries)
                               .Select(x => x.Trim().TrimStart('*').TrimStart('.').ToLowerInvariant())
                               .ToHashSet();

        /// <summary>
        /// 動画ストリーム情報
        /// </summary>
        public VideoStreamInfo? Video { get; private set; }

        /// <summary>
        /// 音声ストリーム情報
        /// </summary>
        public AudioStreamInfo? Audio { get; private set; }

        /// <summary>
        /// メディア情報の取得が成功したかどうか
        /// </summary>
        public bool IsSuccess { get; private set; } = false;

        /// <summary>
        /// 対象ファイルのパス
        /// </summary>
        public string FilePath { get; private set; } = "";

        private readonly IFFmpegService _service;

        /// <summary>
        /// 指定したファイルが処理可能かどうかを判定する。
        /// </summary>
        /// <param name="filePath">対象ファイルのパス</param>
        /// <returns>処理可能なら true、それ以外は false</returns>
        public static bool CanProcess(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return false;

            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            string extWithoutDot = ext.TrimStart('.');
            if (!AllowedExtensions.Contains(extWithoutDot))
                return false;

            return true;
        }

        /// <summary>
        /// 指定したファイルパスから MediaInfo インスタンスを生成する。
        /// 処理に失敗した場合は null を返す。
        /// </summary>
        /// <param name="service">FFmpeg サービス</param>
        /// <param name="filePath">対象ファイルパス</param>
        /// <returns>MediaInfo 成功時はインスタンス、失敗時は null</returns>
        public static MediaInfo? TryCreate(IFFmpegService service, string filePath)
        {
            if (!CanProcess(filePath))
                return null;

            var info = new MediaInfo(service, filePath);
            return info.IsSuccess ? info : null;
        }

        /// <summary>
        /// JSON ノードから double 値を安全に取得する。
        /// </summary>
        private static double GetDouble(JsonNode? node)
        {
            if (node == null) return 0;

            if (node is JsonValue jv && jv.TryGetValue<double>(out var d))
                return d;

            if (double.TryParse(node.ToString(),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out d))
                return d;

            return 0;
        }

        /// <summary>
        /// JSON ノードから long 値を安全に取得する。
        /// </summary>
        private static long GetLong(JsonNode? node)
        {
            if (node == null) return 0;

            if (node is JsonValue jv && jv.TryGetValue<long>(out var l))
                return l;

            if (long.TryParse(node.ToString(),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out l))
                return l;

            return 0;
        }

        /// <summary>
        /// JSON ノードから int 値を安全に取得する。
        /// </summary>
        private static int GetInt(JsonNode? node)
        {
            if (node == null) return 0;

            if (node is JsonValue jv && jv.TryGetValue<int>(out var v))
                return v;

            if (int.TryParse(node.ToString(),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out v))
                return v;

            return 0;
        }

        /// <summary>
        /// 指定パスから ImageSource を作成する。
        /// </summary>
        private static ImageSource LoadThumbnail(string path)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var ms = new MemoryStream();
            fs.CopyTo(ms);
            ms.Position = 0;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.CacheOption = BitmapCacheOption.OnLoad; // 必須：ストリームを全部読む
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }

        /// <summary>
        /// MediaInfo の新しいインスタンスを生成し、指定ファイルの情報を取得する。
        /// </summary>
        /// <param name="service">FFmpeg サービス</param>
        /// <param name="filePath">対象ファイルのパス</param>
        public MediaInfo(IFFmpegService service, string filePath)
        {
            var jsonPath = AppPath.GetTempFilePath(".json");

            _service = service;
            FilePath = filePath;

            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("対象ファイルが見つかりません。", filePath);

                string ffprobeArgs = FFprobeArgumentBuilder.BuildMediaInfoArguments(filePath);
                var ffprobeResult = service.RunFFprobe(ffprobeArgs);
                if (!ffprobeResult.Success)
                {
                    IsSuccess = false;
                    return;
                }

                File.WriteAllText(jsonPath, ffprobeResult.StdOut, System.Text.Encoding.UTF8);

                var jsonData = new JsonData(jsonPath);
                var streams = jsonData.GetArray("streams");

                if (streams != null)
                {
                    foreach (var item in streams)
                    {
                        var type = item?["codec_type"]?.ToString() ?? "";

                        if (type == "video")
                        {
                            Video = new VideoStreamInfo
                            {
                                Codec = item?["codec_name"]?.ToString(),
                                Width = GetInt(item?["width"]),
                                Height = GetInt(item?["height"]),
                                Duration = TimeSpan.FromSeconds(GetDouble(item?["duration"])),
                                DisplayAspectRatio = item?["display_aspect_ratio"]?.ToString(),
                                BitRate = GetLong(item?["bit_rate"]),
                                AvgFrameRate = item?["avg_frame_rate"]?.ToString()
                            };
                        }
                        else if (type == "audio")
                        {
                            Audio = new AudioStreamInfo
                            {
                                Codec = item?["codec_name"]?.ToString(),
                                SampleRate = GetInt(item?["sample_rate"]),
                                BitRate = GetLong(item?["bit_rate"]),
                                Channels = GetInt(item?["channels"])
                            };
                        }
                    }
                }

                IsSuccess = Video != null;
            }
            catch
            {
                IsSuccess = false;
            }
            finally
            {
                if (File.Exists(jsonPath))
                    File.Delete(jsonPath);
            }
        }

        /// <summary>
        /// サムネイルを作成して ImageSource として取得する。
        /// </summary>
        /// <param name="maxSize">長辺の最大サイズ（デフォルト 400px）</param>
        /// <returns>作成に成功すれば ImageSource、失敗すれば null</returns>
        public ImageSource? CreateThumbnail(int maxSize = 400)
        {
            if (Video == null)
                return null;

            var thumbnailPath = AppPath.GetTempFilePath(".png");

            try
            {
                string scale = Video.Width > Video.Height
                    ? $"{maxSize}:-2"
                    : $"-2:{maxSize}";

                string ffmpegArgs = FFmpegArgumentBuilder.BuildThumbnaiArguments(
                    FilePath,
                    scale,
                    thumbnailPath);

                var result = _service.RunFFmpeg(ffmpegArgs);
                if (!result.Success)
                    return null;

                return LoadThumbnail(thumbnailPath);
            }
            finally
            {
                if (File.Exists(thumbnailPath))
                    File.Delete(thumbnailPath);
            }
        }
    }
}
