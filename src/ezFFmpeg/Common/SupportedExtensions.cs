
namespace ezFFmpeg.Common
{
    /// <summary>
    /// アプリケーションで処理対象とする
    /// 動画ファイルの拡張子一覧を定義するクラス。
    /// </summary>
    /// <remarks>
    /// - 主にファイル選択ダイアログのフィルタ
    /// - ドロップされたファイルの拡張子判定
    /// などに使用する。
    /// </remarks>
    public static class SupportedExtensions
    {
        /// <summary>
        /// 対応している動画ファイル拡張子の一覧。
        /// </summary>
        /// <remarks>
        /// <para>
        /// ワイルドカード形式（*.mp4 など）でカンマ区切り。
        /// </para>
        /// <para>
        /// 例:
        /// <code>
        /// "*.mp4,*.mov,*.avi,*.mkv"
        /// </code>
        /// </para>
        /// <para>
        /// FFmpeg / FFprobe で処理可能な拡張子を前提とする。
        /// </para>
        /// </remarks>
        public const string Extensions = "*.mp4,*.mov,*.avi,*.mkv";
    }
}
