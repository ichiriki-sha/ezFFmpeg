using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ezFFmpeg.Models.Presets
{
    /// <summary>
    /// ビデオ解像度のプリセットを定義する静的クラス。
    /// 各プリセットは (Resolution, Name, Size) のタプルで保持され、
    /// ユーザー選択やエンコードプロファイルで使用される。
    /// </summary>
    public static class VideoResolutions
    {
        /// <summary>
        /// 元の解像度を変更しない設定
        /// </summary>
        public static readonly (string Resolution, string Name, Size Size) Source = ("source", "変更しない", new Size(0, 0));

        /// <summary>427x240 (nHD) プリセット</summary>
        public static readonly (string Resolution, string Name, Size Size) NHD = ("240", "427x240", new Size(427, 240));

        /// <summary>720x480 (SD) プリセット</summary>
        public static readonly (string Resolution, string Name, Size Size) SD = ("480", "720x480", new Size(720, 480));

        /// <summary>1280x720 (HD) プリセット</summary>
        public static readonly (string Resolution, string Name, Size Size) HD = ("720", "1280x720", new Size(1280, 720));

        /// <summary>1920x1080 (FHD) プリセット</summary>
        public static readonly (string Resolution, string Name, Size Size) FHD = ("1080", "1920x1080", new Size(1920, 1080));

        /// <summary>2560x1440 (QHD) プリセット</summary>
        public static readonly (string Resolution, string Name, Size Size) QHD = ("1440", "2560x1440", new Size(2560, 1440));

        /// <summary>3840x2160 (UHD 4K) プリセット</summary>
        public static readonly (string Resolution, string Name, Size Size) UHD4K = ("2160", "3840x2160", new Size(3840, 2160));

        /// <summary>7680x4320 (UHD 8K) プリセット</summary>
        public static readonly (string Resolution, string Name, Size Size) UHD8K = ("4320", "7680x4320", new Size(7680, 4320));

        /// <summary>
        /// すべての解像度プリセットをまとめた配列
        /// </summary>
        public static readonly (string Resolution, string Name, Size Size)[] All =
        [
            Source, NHD, SD, HD, FHD, QHD, UHD4K, UHD8K
        ];

        /// <summary>
        /// 指定した解像度キーに対応する <see cref="Size"/> を取得する。
        /// </summary>
        /// <param name="resolution">解像度キー (例: "720", "1080")</param>
        /// <returns>対応する <see cref="Size"/>。見つからない場合は Size(0,0)</returns>
        public static Size GetSize(string resolution)
        {
            var ret = Array.Find(All, c => c.Resolution == resolution);
            return ret.Size;
        }
    }
}
