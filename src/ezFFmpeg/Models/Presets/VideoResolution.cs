using ezFFmpeg.Models.Interfaces;
using System.Drawing;

namespace ezFFmpeg.Models.Presets
{
    /// <summary>
    /// ビデオの解像度を表すプリセットクラス。
    /// UI の選択肢および FFmpeg の解像度指定引数生成に使用される。
    /// </summary>
    public class VideoResolution : IPreset
    {
        /// <summary>
        /// 解像度の識別子（例: "720", "1080", "source"）。
        /// FFmpeg の引数指定や内部判定に使用される。
        /// </summary>
        public string Resolution { get; }

        /// <summary>
        /// IPreset インターフェース用の値。
        /// 内部処理で使用され、通常は UI には直接表示されない。
        /// </summary>
        string IPreset.Value => Resolution;

        /// <summary>
        /// UI 表示用の名称（例: "1280x720", "変更しない"）。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 実際の解像度サイズ。
        /// 幅と高さをピクセル単位で保持する。
        /// </summary>
        public Size Size { get; }

        /// <summary>
        /// 元動画の解像度をそのまま使用するかどうかを示す。
        /// true の場合、明示的な解像度変更は行われない。
        /// </summary>
        public bool IsSource { get; }

        /// <summary>
        /// VideoResolution の新しいインスタンスを作成する。
        /// </summary>
        /// <param name="resolution">
        /// 解像度の識別子（FFmpeg 用、例: "720", "1080", "source"）
        /// </param>
        /// <param name="name">
        /// UI 表示用の名称
        /// </param>
        /// <param name="size">
        /// 解像度の実サイズ（ピクセル単位）
        /// </param>
        /// <param name="isSource">
        /// 元動画の解像度を使用するかどうか
        /// </param>
        public VideoResolution(string resolution, string name, Size size, bool isSource)
        {
            Resolution = resolution;
            Name = name;
            Size = size;
            IsSource = isSource;
        }
    }
}
