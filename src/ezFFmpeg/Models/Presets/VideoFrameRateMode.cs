using ezFFmpeg.Models.Interfaces;

namespace ezFFmpeg.Models.Presets
{
    /// <summary>
    /// ビデオのフレームレート制御モードのプリセットを表すクラス。
    /// 
    /// CFR / VFR / Passthrough などのモードを値オブジェクトとして表現し、
    /// プロファイル設定、バリデーション、FFmpeg 引数生成で共通的に使用される。
    /// </summary>
    public sealed class VideoFrameRateMode : IPreset
    {
        /// <summary>
        /// フレームレートモード識別値。
        /// FFmpeg に渡される値（例: "cfr", "vfr", "passthrough"）。
        /// </summary>
        public string FrameRateMode { get; }

        /// <summary>
        /// IPreset 用の共通値。
        /// 内部的には <see cref="FrameRateMode"/> をそのまま返す。
        /// </summary>
        string IPreset.Value => FrameRateMode;

        /// <summary>
        /// UI 表示用の名称。
        /// 例: "CFR(固定)", "VFR(可変)", "変更しない"
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// ソース（入力動画）のフレームレート制御をそのまま使用するかどうか。
        /// true の場合、フレームレート制御に関する指定を行わない。
        /// </summary>
        public bool IsSource { get; }

        /// <summary>
        /// 固定フレームレート (CFR) モードかどうか。
        /// </summary>
        public bool IsCfr { get; }

        /// <summary>
        /// 可変フレームレート (VFR) モードかどうか。
        /// </summary>
        public bool IsVfr { get; }

        /// <summary>
        /// パススルー（フレームレートを変更しない）モードかどうか。
        /// </summary>
        public bool IsPassthrough { get; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="frameRateMode">
        /// フレームレートモード識別値（FFmpeg 用）
        /// </param>
        /// <param name="name">
        /// 表示名（UI 用）
        /// </param>
        /// <param name="isSource">
        /// ソースのフレームレート制御を使用するかどうか
        /// </param>
        public VideoFrameRateMode(string frameRateMode, string name, bool isSource)
        {
            FrameRateMode = frameRateMode;
            Name = name;
            IsSource = isSource;

            // モード判定（大小文字を区別しない）
            IsCfr = frameRateMode.Equals("cfr", StringComparison.OrdinalIgnoreCase);
            IsVfr = frameRateMode.Equals("vfr", StringComparison.OrdinalIgnoreCase);
            IsPassthrough = frameRateMode.Equals("passthrough", StringComparison.OrdinalIgnoreCase);
        }
    }
}
