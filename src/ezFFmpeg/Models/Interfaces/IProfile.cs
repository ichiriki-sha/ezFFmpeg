using ezFFmpeg.Models.Quality;

namespace ezFFmpeg.Models.Interfaces
{
    /// <summary>
    /// 変換プロファイルを表すインターフェイス。
    /// </summary>
    public interface IProfile
    {
        // ============================
        // 内部キー（不変・一意）
        // ============================
        Guid ProfileId { get; set; }

        // ============================
        // 表示名
        // ============================
        string ProfileName { get; set; }

        // ============================
        // 出力設定
        // ============================
        /// <summary>出力形式（例: mp4, mkv）</summary>
        string  OutputFormat { get; set; }

        /// <summary>出力先フォルダ</summary>
        string OutputFolderPath { get; set; }

        /// <summary>出力ファイル名フォーマット</summary>
        string OutputFileFormat { get; set; }

        /// <summary>既存ファイルの上書きフラグ</summary>
        bool IsOutputOverwrite { get; set; }

        // ============================
        // ビデオ設定
        // ============================
        /// <summary>ビデオ出力の有効/無効</summary>
        bool IsVideoEnabled { get; set; }

        /// <summary>ビデオエンコーダー名</summary>
        string VideoEncoder { get; set; }

        /// <summary>ビデオ品質レベル</summary>
        QualityTier VideoQualityTier { get; set; }

        /// <summary>ビデオ解像度</summary>
        string VideoResolution { get; set; }

        /// <summary>フレームレート制御方式</summary>
        string VideoFrameRateMode { get; set; }

        /// <summary>フレームレート</summary>
        string VideoFrameRate { get; set; }

        // ============================
        // オーディオ設定
        // ============================
        /// <summary>オーディオ出力の有効/無効</summary>
        bool IsAudioEnabled { get; set; }

        /// <summary>オーディオエンコーダー名</summary>
        string AudioEncoder { get; set; }

        /// <summary>オーディオビットレート</summary>
        string AudioBitRate { get; set; }

        // ============================
        // 管理用フラグ
        // ============================
        /// <summary>ユーザー定義プロファイルかどうか</summary>
        bool IsUserDefined { get; set; }

        /// <summary>デフォルトプロファイルかどうか</summary>
        bool IsDefault { get; set; }

        /// <summary>最後に使用されたプロファイルかどうか</summary>
        bool IsLastUsed { get; set; }
    }
}
