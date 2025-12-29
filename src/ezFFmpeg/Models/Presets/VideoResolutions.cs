using System;
using System.Drawing;

namespace ezFFmpeg.Models.Presets
{
    /// <summary>
    /// ビデオ解像度のプリセットを定義する静的クラス。
    /// 各プリセットは FFmpeg の引数生成や UI 選択肢として使用される。
    /// </summary>
    public static class VideoResolutions
    {
        /// <summary>
        /// 元の解像度を変更しないプリセット。
        /// </summary>
        public static readonly VideoResolution Source = new("source", "変更しない", new Size(0, 0), true);

        /// <summary>427x240（nHD）プリセット</summary>
        public static readonly VideoResolution NHD = new("240", "427x240", new Size(427, 240), false);

        /// <summary>720x480（SD）プリセット</summary>
        public static readonly VideoResolution SD = new("480", "720x480", new Size(720, 480), false);

        /// <summary>1280x720（HD）プリセット</summary>
        public static readonly VideoResolution HD = new("720", "1280x720", new Size(1280, 720), false);

        /// <summary>1920x1080（Full HD）プリセット</summary>
        public static readonly VideoResolution FHD = new("1080", "1920x1080", new Size(1920, 1080), false);

        /// <summary>2560x1440（QHD）プリセット</summary>
        public static readonly VideoResolution QHD = new("1440", "2560x1440", new Size(2560, 1440), false);

        /// <summary>3840x2160（UHD 4K）プリセット</summary>
        public static readonly VideoResolution UHD4K = new("2160", "3840x2160", new Size(3840, 2160), false);

        /// <summary>7680x4320（UHD 8K）プリセット</summary>
        public static readonly VideoResolution UHD8K = new("4320", "7680x4320", new Size(7680, 4320), false);

        /// <summary>
        /// 定義されているすべての解像度プリセット一覧。
        /// </summary>
        public static readonly VideoResolution[] All =
        [
            Source, NHD, SD, HD, FHD, QHD, UHD4K, UHD8K
        ];

        /// <summary>
        /// 解像度識別子から対応する VideoResolution を取得する。
        /// </summary>
        /// <param name="resolution">解像度識別子（例: "720", "1080", "source"）</param>
        /// <returns>対応する VideoResolution</returns>
        /// <exception cref="ArgumentException">指定された解像度が定義されていない場合</exception>
        public static VideoResolution GetResolution(string resolution)
        {
            var videoResolution = Array.Find(All, r => r.Resolution == resolution);
            if (videoResolution == null)
                throw new ArgumentException($"未対応の resolution: {resolution}");

            return videoResolution;
        }
    }
}
