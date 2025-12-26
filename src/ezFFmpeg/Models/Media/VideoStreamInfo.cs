using ezFFmpeg.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace ezFFmpeg.Models.Media
{
    /// <summary>
    /// 動画ストリームの情報を保持するクラス。
    /// FFmpeg などから取得した映像トラックの詳細情報を格納する。
    /// </summary>
    public class VideoStreamInfo
    {
        /// <summary>
        /// 使用されているビデオコーデックの名称。
        /// 例: "h264", "hevc" など。
        /// </summary>
        public string? Codec { get; set; }

        /// <summary>
        /// 映像の幅（ピクセル単位）。
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 映像の高さ（ピクセル単位）。
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 映像の再生時間。
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// ビットレート（bps）。
        /// </summary>
        public long BitRate { get; set; }

        /// <summary>
        /// 平均フレームレートを文字列で表現。
        /// 例: "30/1" や "29.97" など。
        /// </summary>
        public string? AvgFrameRate { get; set; }

        /// <summary>
        /// 表示アスペクト比。
        /// 例: "16:9", "4:3" など。
        /// </summary>
        public string? DisplayAspectRatio { get; set; }

        /// <summary>
        /// ビットレートを文字列として取得。
        /// 例: 500000 → "500 kbps"
        /// </summary>
        public string BitRateString => BitRate.ToBitrateString();

        /// <summary>
        /// フレームレートを文字列として取得。
        /// 例: "30/1" → "30 fps"
        /// </summary>
        public string FrameRateString => AvgFrameRate.ToFpsString();

        /// <summary>
        /// 解像度を文字列として取得。
        /// 例: Width=1920, Height=1080 → "1920x1080"
        /// </summary>
        public string SizeString => $"{Width}x{Height}";
    }
}
