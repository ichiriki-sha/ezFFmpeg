using ezFFmpeg.Models.Interfaces;

namespace ezFFmpeg.Models.Presets
{
    /// <summary>
    /// オーディオビットレートのプリセットを表すクラス。
    /// 
    /// UI の選択肢、プロファイル設定、FFmpeg 引数生成などで
    /// 共通的に使用される値オブジェクト。
    /// </summary>
    public sealed class AudioBitRate : IPreset
    {
        /// <summary>
        /// ビットレート識別値。
        /// FFmpeg に渡される値（例: "128k", "192k", "source"）。
        /// </summary>
        public string BitRate { get; }

        /// <summary>
        /// IPreset 用の共通値。
        /// 内部的には <see cref="BitRate"/> をそのまま返す。
        /// </summary>
        string IPreset.Value => BitRate;

        /// <summary>
        /// UI 表示用の名称。
        /// 例: "128 kbps", "192 kbps", "元のまま"
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 入力ソースのビットレートをそのまま使用するかどうか。
        /// true の場合、再エンコード時にビットレート指定を行わない。
        /// </summary>
        public bool IsSource { get; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="bitRate">
        /// ビットレート識別値（FFmpeg 用）
        /// </param>
        /// <param name="name">
        /// 表示名（UI 用）
        /// </param>
        /// <param name="isSource">
        /// ソースのビットレートを使用するかどうか
        /// </param>
        public AudioBitRate(string bitRate, string name, bool isSource)
        {
            BitRate = bitRate;
            Name = name;
            IsSource = isSource;
        }
    }
}
