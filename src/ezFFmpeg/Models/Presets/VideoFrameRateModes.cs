using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Presets
{
    /// <summary>
    /// ビデオのフレームレートモードのプリセットを定義する静的クラス。
    /// 各プリセットは (FrameRateMode, Name) のタプルで保持され、
    /// ユーザー選択やエンコードプロファイルで使用される。
    /// </summary>
    public static class VideoFrameRateModes
    {
        /// <summary>
        /// 元のフレームレートを変更しない設定
        /// </summary>
        public static readonly (string FrameRateMode, string Name) Source = ("source", "変更しない");

        /// <summary>
        /// 固定フレームレート (CFR) モード
        /// </summary>
        public static readonly (string FrameRateMode, string Name) CFR = ("cfr", "CFR(固定)");

        /// <summary>
        /// 可変フレームレート (VFR) モード
        /// </summary>
        public static readonly (string FrameRateMode, string Name) VFR = ("vfr", "VFR(可変)");

        /// <summary>
        /// パススルー (元のフレームレートをそのまま保持)
        /// </summary>
        public static readonly (string FrameRateMode, string Name) Passthrough = ("passthrough", "パススルー");

        /// <summary>
        /// すべてのフレームレートモードをまとめた配列
        /// </summary>
        public static readonly (string FrameRateMode, string Name)[] All =
        [
            Source, CFR, VFR, Passthrough
        ];
    }
}
