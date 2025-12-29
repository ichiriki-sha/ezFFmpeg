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
        /// 元のフレームレートをそのまま保持する（パススルー）モード。 
        /// パススルーを変更しないとする。    
        /// </summary>
        public static readonly VideoFrameRateMode Passthrough = new("passthrough", "変更しない", true);

        /// <summary>
        /// 固定フレームレート (CFR: Constant Frame Rate) モード。
        /// フレーム落ちや補完を行い、指定 FPS に固定する。
        /// </summary>
        public static readonly VideoFrameRateMode Cfr = new("cfr", "CFR(固定)", false);

        /// <summary>
        /// 可変フレームレート (VFR: Variable Frame Rate) モード。
        /// 入力のタイムスタンプを優先し、可変 FPS を許可する。
        /// </summary>
        public static readonly VideoFrameRateMode Vfr = new("vfr", "VFR(可変)", false);

        /// <summary>
        /// 定義されているすべてのフレームレートモードの一覧。
        /// ComboBox やバリデーション処理で利用される。
        /// </summary>
        public static readonly VideoFrameRateMode[] All =
        [
            Passthrough, Cfr, Vfr
        ];

        /// <summary>
        /// 指定された識別名に対応するフレームレートモードを取得する。
        /// </summary>
        /// <param name="frameRateMode">フレームレートモード識別名</param>
        /// <returns>対応する <see cref="VideoFrameRateMode"/></returns>
        /// <exception cref="ArgumentException">指定された識別名が未対応の場合</exception>
        public static VideoFrameRateMode GetFrameRateMode(string frameRateMode)
        {
            var videoFrameRateMode = Array.Find(All, c => c.FrameRateMode == frameRateMode);
            if (videoFrameRateMode!.FrameRateMode == null) 
                throw new ArgumentException($"未対応の frameRateMode: {frameRateMode}");

            return videoFrameRateMode;
        }
    }
}
