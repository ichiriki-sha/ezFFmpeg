using System;

namespace ezFFmpeg.Helpers
{
    /// <summary>
    /// メディア情報のフォーマット変換や文字列化に関するヘルパークラス
    /// </summary>
    public static class MediaFormatHelper
    {
        /// <summary>
        /// 周波数 (Hz) を kHz または MHz に変換して文字列化
        /// </summary>
        /// <param name="hz">周波数 (Hz)</param>
        /// <returns>例: 44100 → "44.1 kHz", 5000000 → "5.00 MHz"</returns>
        public static string ToHzString(this int hz)
        {
            if (hz >= 1_000_000)
                return $"{hz / 1_000_000.0:F2} MHz";
            if (hz >= 1_000)
                return $"{hz / 1000.0:F1} kHz";

            return $"{hz} Hz";
        }

        /// <summary>
        /// ビットレート (bps) を Kbps, Mbps, Gbps に変換して文字列化
        /// </summary>
        /// <param name="bps">ビットレート (bps)</param>
        /// <returns>例: 128000 → "128 kbps", 5000000 → "5.00 Mbps"</returns>
        public static string ToBitRateString(this long bps)
        {
            if (bps <= 0)
                return "0 kbps";

            if (bps >= 1_000_000_000)
                return $"{bps / 1_000_000_000.0:F2} Gbps";
            if (bps >= 1_000_000)
                return $"{bps / 1_000_000.0:F2} Mbps";

            return $"{bps / 1000.0:F0} kbps";
        }

        /// <summary>
        /// 平均フレームレート文字列 ("30000/1001" など) を fps の double に変換
        /// </summary>
        /// <param name="avgFrameRate">フレームレート文字列</param>
        /// <returns>fps 値 (例: "30000/1001" → 29.97)</returns>
        public static double ToFps(this string? avgFrameRate)
        {
            if (string.IsNullOrWhiteSpace(avgFrameRate)) return 0;

            if (avgFrameRate.Contains('/'))
            {
                var parts = avgFrameRate.Split('/');
                if (double.TryParse(parts[0], out double n) &&
                    double.TryParse(parts[1], out double d) &&
                    d != 0)
                {
                    return n / d;
                }
            }

            // 整数の場合
            if (double.TryParse(avgFrameRate, out double fps))
                return fps;

            return 0;
        }

        /// <summary>
        /// 平均フレームレート文字列を "xx.xx fps" の形式で返す
        /// </summary>
        /// <param name="avgFrameRate">フレームレート文字列</param>
        /// <returns>例: "29.97 fps"</returns>
        public static string ToFpsString(this string? avgFrameRate)
            => $"{avgFrameRate.ToFps():F2} fps";

        /// <summary>
        /// チャンネル数に応じた表記文字列を返す
        /// </summary>
        /// <param name="channels">チャンネル数</param>
        /// <returns>
        /// 1 → "モノラル", 2 → "ステレオ",
        /// 6 → "5.1サラウンド", 8 → "7.1サラウンド",
        /// それ以外 → ""
        /// </returns>
        public static string ToChannelsString(this int channels)
        {
            if (channels == 1)
            {
                return "モノラル";
            }
            else if (channels == 2)
            {
                return "ステレオ";
            }
            else if (channels == 6)
            {
                return "5.1サラウンド";
            }
            else if (channels == 8)
            {
                return "7.1サラウンド";
            }
            return "";
        }
    }
}
