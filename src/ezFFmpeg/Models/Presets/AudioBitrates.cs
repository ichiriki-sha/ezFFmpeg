using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Presets
{
    /// <summary>
    /// オーディオビットレートのプリセットを定義する静的クラス。
    /// 各プリセットは (BitRate, Name) のタプルで保持され、ユーザー選択やプロファイル設定に利用される。
    /// </summary>
    public static class AudioBitrates
    {
        /// <summary>
        /// ソースのまま変更しない設定
        /// </summary>
        public static readonly (string BitRate, string Name) Source = ("source", "変更しない");

        /// <summary>
        /// 64Kbps のプリセット
        /// </summary>
        public static readonly (string BitRate, string Name) Kbps64 = ("64", "64Kbps");

        /// <summary>
        /// 96Kbps のプリセット
        /// </summary>
        public static readonly (string BitRate, string Name) Kbps96 = ("96", "96Kbps");

        /// <summary>
        /// 128Kbps のプリセット
        /// </summary>
        public static readonly (string BitRate, string Name) Kbps128 = ("128", "128Kbps");

        /// <summary>
        /// 192Kbps のプリセット
        /// </summary>
        public static readonly (string BitRate, string Name) Kbps192 = ("192", "192Kbps");

        /// <summary>
        /// 256Kbps のプリセット
        /// </summary>
        public static readonly (string BitRate, string Name) Kbps256 = ("256", "256Kbps");

        /// <summary>
        /// 320Kbps のプリセット
        /// </summary>
        public static readonly (string BitRate, string Name) Kbps320 = ("320", "320Kbps");

        /// <summary>
        /// すべてのビットレートプリセットをまとめた配列
        /// </summary>
        public static readonly (string BitRate, string Name)[] All =
        [
            Source, Kbps64, Kbps96, Kbps128, Kbps192, Kbps256, Kbps320
        ];
    }
}
