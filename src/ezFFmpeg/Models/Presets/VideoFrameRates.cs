using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Presets
{
    /// <summary>
    /// ビデオフレームレートのプリセットを定義する静的クラス。
    /// 各プリセットは (FrameRate, Name) のタプルで保持され、
    /// ユーザー選択やエンコードプロファイルで使用される。
    /// </summary>
    public static class VideoFrameRates
    {
        /// <summary>
        /// 元のフレームレートを変更しない設定
        /// </summary>
        public static readonly (string FrameRate, string Name) Source = ("source", "変更しない");

        /// <summary>24fps プリセット</summary>
        public static readonly (string FrameRate, string Name) Fps24 = ("24", "24fps");

        /// <summary>25fps プリセット</summary>
        public static readonly (string FrameRate, string Name) Fps25 = ("25", "25fps");

        /// <summary>29.97fps プリセット</summary>
        public static readonly (string FrameRate, string Name) Fps29_97 = ("29.97", "29.97fps");

        /// <summary>30fps プリセット</summary>
        public static readonly (string FrameRate, string Name) Fps30 = ("30", "30fps");

        /// <summary>50fps プリセット</summary>
        public static readonly (string FrameRate, string Name) Fps50 = ("50", "50fps");

        /// <summary>59.94fps プリセット</summary>
        public static readonly (string FrameRate, string Name) Fps59_94 = ("59.94", "59.94fps");

        /// <summary>60fps プリセット</summary>
        public static readonly (string FrameRate, string Name) Fps60 = ("60", "60fps");

        /// <summary>120fps プリセット</summary>
        public static readonly (string FrameRate, string Name) Fps120 = ("120", "120fps");

        /// <summary>
        /// すべてのフレームレートプリセットをまとめた配列。
        /// UI 選択肢や一括処理に利用される。
        /// </summary>
        public static readonly (string FrameRate, string Name)[] All =
        [
            Source, Fps24, Fps25, Fps29_97, Fps30, Fps50, Fps59_94, Fps60, Fps120
        ];
    }
}
