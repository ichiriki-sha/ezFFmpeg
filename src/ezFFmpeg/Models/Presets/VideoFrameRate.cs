using ezFFmpeg.Models.Interfaces;

namespace ezFFmpeg.Models.Presets
{
    /// <summary>
    /// ビデオのフレームレート（FPS）のプリセットを表すクラス。
    /// 
    /// UI の選択肢、プロファイル設定、FFmpeg 引数生成などで
    /// 共通的に使用される値オブジェクト。
    /// </summary>
    public sealed class VideoFrameRate : IPreset
    {
        /// <summary>
        /// フレームレート識別値。
        /// FFmpeg に渡される値（例: "30", "60", "source"）。
        /// </summary>
        public string FrameRate { get; }

        /// <summary>
        /// IPreset 用の共通値。
        /// 内部的には <see cref="FrameRate"/> をそのまま返す。
        /// </summary>
        string IPreset.Value => FrameRate;

        /// <summary>
        /// UI 表示用の名称。
        /// 例: "30 fps", "60 fps", "元のまま"
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 入力ソースのフレームレートをそのまま使用するかどうか。
        /// true の場合、FPS の強制指定を行わない。
        /// </summary>
        public bool IsSource { get; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="frameRate">
        /// フレームレート識別値（FFmpeg 用）
        /// </param>
        /// <param name="name">
        /// 表示名（UI 用）
        /// </param>
        /// <param name="isSource">
        /// ソースのフレームレートを使用するかどうか
        /// </param>
        public VideoFrameRate(string frameRate, string name, bool isSource)
        {
            FrameRate = frameRate;
            Name = name;
            IsSource = isSource;
        }
    }
}
