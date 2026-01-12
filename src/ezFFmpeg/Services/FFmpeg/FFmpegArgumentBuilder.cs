using ezFFmpeg.Models.Common;
using ezFFmpeg.Models.Encoder;
using ezFFmpeg.Models.Presets;
using ezFFmpeg.Models.Quality;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace ezFFmpeg.Services.FFmpeg
{
    /// <summary>
    /// FFmpeg コマンドライン引数を構築するユーティリティクラス。
    /// 動画・音声の変換やテスト、サムネイル作成用の引数文字列を生成します。
    /// </summary>
    public static class FFmpegArgumentBuilder
    {
        /// <summary>
        /// 最大公約数を求める (GCD)。
        /// アスペクト比計算に使用。
        /// </summary>
        private static int Gcd(int a, int b)
        {
            while (b != 0)
            {
                int r = a % b;
                a = b;
                b = r;
            }
            return a;
        }

        /// <summary>
        /// 幅・高さからアスペクト比を Size 型で取得。
        /// </summary>
        private static Size GetAspectRatio(int width, int height)
        {
            int g = Gcd(width, height);
            return new(width / g, height / g);
        }

        /// <summary>
        /// プロファイルに基づきフレームレート関連オプションを追加。
        /// </summary>
        private static void AppendFrameRateOptions(StringBuilder sb, Profile profile)
        {
            if(!string.IsNullOrWhiteSpace(profile.VideoFrameRateMode))
            {
                var videoFrameRateMode = VideoFrameRateModes.GetFrameRateMode(profile.VideoFrameRateMode);

                if (videoFrameRateMode.IsPassthrough)
                {
                    // 変更しない場合は何もしない
                }
                else if (videoFrameRateMode.IsCfr)
                {
                    if (!string.IsNullOrEmpty(profile.VideoFrameRate))
                    {
                        sb.Append($"-r {profile.VideoFrameRate} ");
                    }
                    sb.Append("-vsync cfr ");
                }
                else if (videoFrameRateMode.IsVfr)
                {
                    sb.Append("-vsync vfr ");
                }
            }
        }

        /// <summary>
        /// 指定されたプロファイルと入力ファイルから変換用 FFmpeg 引数を生成。
        /// </summary>
        /// <param name="profile">変換設定プロファイル</param>
        /// <param name="item">変換対象ファイル情報</param>
        /// <param name="outputPath">出力ファイルパス</param>
        /// <returns>FFmpeg コマンドライン引数文字列</returns>
        public static string BuildConvertArguments(Profile profile, FileItem item, string outputPath)
        {
            var sb = new StringBuilder();

            // バナー非表示
            sb.Append("-hide_banner ");


            // ★ 変換開始時間
            if (item.StartPosition > TimeSpan.Zero)
            {
                var startrTime = item.StartPosition.ToString(@"hh\:mm\:ss\.fff");
                sb.Append($"-ss {startrTime} ");
            }

            // ★ 変換終了時間
            if (item.EndPosition < item.VideoDuration)
            {
                var endTime = item.EndPosition.ToString(@"hh\:mm\:ss\.fff");
                sb.Append($"-to {endTime} ");
            }
            
            // 入力ファイル
            sb.Append($"-i \"{item.FilePath}\" ");

            // Video
            if (profile.IsVideoEnabled)
            {
                sb.Append("-map 0:v:0 ");

                // エンコーダ指定
                if (!string.IsNullOrEmpty(profile.VideoEncoder))
                {
                    sb.Append($"-c:v {profile.VideoEncoder} ");
                }

                var videoEncoder = VideoEncoders.GetEncoder(profile.VideoEncoder);
                var level = videoEncoder.GetQuality(profile.VideoQualityTier);

                if (!videoEncoder.IsCopy)
                {
                    // エンコーダ動作モード指定
                    if (videoEncoder.IsAmf)
                        sb.Append("-quality ").Append(level.PerformanceValue).Append(' ');
                    else
                        sb.Append("-preset ").Append(level.PerformanceValue).Append(' ');
                }

                // 画質制御
                switch (level.QualityType)
                {
                    case QualityType.Crf:
                        sb.Append("-crf ").Append(level.QualityValue).Append(' ');
                        break;

                    case QualityType.Cq:
                        if (videoEncoder.IsQsv)
                            sb.Append("-global_quality ");
                        else
                            sb.Append("-cq ");
                        sb.Append(level.QualityValue).Append(' ');
                        break;
                }

                // 解像度制御
                if (!string.IsNullOrEmpty(profile.VideoResolution))
                {
                    var resolution = VideoResolutions.GetResolution(profile.VideoResolution);
                    if(!resolution.IsSource)
                    {
                        var srcSize = item.VideoResolutionSize;
                        var trgSize = resolution.Size;

                        var srcAspect = GetAspectRatio(srcSize.Width, srcSize.Height);
                        var trgAspect = GetAspectRatio(trgSize.Width, trgSize.Height);

                        if (srcAspect.Width == trgAspect.Width && srcAspect.Height == trgAspect.Height)
                        {
                            // アスペクト比が同じ
                            sb.Append($"-s {trgSize.Width}x{trgSize.Height} ");
                        }
                        else
                        {
                            // アスペクト比が異なる場合
                            if (item.VideoResolutionSize.Width > item.VideoResolutionSize.Height)
                                sb.Append($"-vf scale={trgSize.Width}:-2,setsar=1 ");
                            else
                                sb.Append($"-vf scale=-2:{trgSize.Width},setsar=1 ");
                        }
                    }
                }

                // フレームレート制御
                AppendFrameRateOptions(sb, profile);
            }
            else
            {
                sb.Append("-vn "); // videoなし
            }

            // Audio
            if (profile.IsAudioEnabled && item.MediaInfo.Audio != null)
            {
                sb.Append("-map 0:a:0 ");

                if (!string.IsNullOrEmpty(profile.AudioEncoder))
                {
                    sb.Append("-c:a ");
                    sb.Append($"{profile.AudioEncoder} ");
                }

                var audioEncoder = AudioEncoders.GetEncoder(profile.AudioEncoder);
                var audioBitRate = AudioBitRates.GetBitRate(profile.AudioBitRate);

                if (!string.IsNullOrEmpty(profile.AudioBitRate) &&
                    !audioEncoder.IsCopy  &&
                    !audioBitRate.IsSource)
                {
                    sb.Append($"-b:a {profile.AudioBitRate} ");
                }
            }
            else
            {
                sb.Append("-an "); // audioなし
            }

            // 出力上書き
            sb.Append(profile.IsOutputOverwrite ? "-y " : "-n ");

            // 出力ファイル
            sb.Append($"\"{outputPath}\"");

            return sb.ToString().Trim();
        }

        /// <summary>
        /// FFmpeg のエンコーダー一覧取得用引数を生成
        /// </summary>
        public static string BuildEncodersArguments() => "-hide_banner -encoders";

        /// <summary>
        /// FFmpeg バージョン確認用引数を生成
        /// </summary>
        public static string BuildVersionArguments() => "-hide_banner -version";

        /// <summary>
        /// 指定エンコーダーでの簡易音声テスト用引数を生成
        /// </summary>
        public static string BuildAudioTestArguments(string encoder) =>
            $"-hide_banner -loglevel quiet -f lavfi -i anullsrc=channel_layout=stereo:sample_rate=44100 -t 0.1 -c:a {encoder} -f null NUL";

        /// <summary>
        /// 指定エンコーダーでの簡易動画テスト用引数を生成
        /// </summary>
        public static string BuildVideoTestArguments(string encoder) =>
            $"-hide_banner -loglevel quiet -f lavfi -i nullsrc -t 1 -c:v {encoder} -f null -";

        /// <summary>
        /// サムネイル作成用 FFmpeg 引数を生成
        /// </summary>
        /// <param name="filePath">入力動画ファイルパス</param>
        /// <param name="scale">スケール指定 (例: 400:-2)</param>
        /// <param name="thumbnailPath">出力サムネイルパス</param>
        public static string BuildThumbnaiArguments(string filePath, string scale, string thumbnailPath) =>
            $"-hide_banner -i \"{filePath}\" -ss 00:00:01 -vframes 1 -vf \"scale={scale}\" -q:v 2 -update 1 \"{thumbnailPath}\"";
    }
}
