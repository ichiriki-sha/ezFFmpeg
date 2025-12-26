using ezFFmpeg.Models.Common;
using ezFFmpeg.Models.Interfaces;
using ezFFmpeg.Services.FFmpeg;
using ezFFmpeg.Services.Mapping;
using System.Windows;

namespace ezFFmpeg.Common
{
    /// <summary>
    /// アプリケーション全体の設定および実行時状態を保持するクラス。
    /// JSON による永続化対象の設定と、実行時のみ使用する一時情報の両方を管理する。
    /// </summary>
    public class AppSettings : ISettings
    {
        // =====================================================
        // JSON シリアライズ対象外（実行時のみ使用する情報）
        // =====================================================

        /// <summary>
        /// アプリケーション名。
        /// 実行中のプロセス名から取得され、設定ファイルには保存されない。
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public string AppName { get; set; }

        /// <summary>
        /// 設定ファイルが保存されるフォルダのパス。
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public string SettingsFolderPath { get; set; }

        /// <summary>
        /// 作業用一時ファイルを保存するフォルダのパス。
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public string WorkFolderPath { get; set; }

        /// <summary>
        /// アプリケーションの処理開始時刻。
        /// ログ出力や経過時間計測などに使用される。
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime ProcessStartTime { get; set; }

        // =====================================================
        // 画面表示制御（UI 状態）
        // =====================================================

        /// <summary>
        /// ツールバーの表示／非表示。
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public bool ToolbarVisible { get; set; }

        /// <summary>
        /// ログ表示エリアの表示／非表示。
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public bool LogVisible { get; set; }

        /// <summary>
        /// プレビューエリアの表示／非表示。
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public bool PreviewVisible { get; set; }

        /// <summary>
        /// ステータスバーの表示／非表示。
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public bool StatusbarVisible { get; set; }

        /// <summary>
        /// ログ表示行の高さ。
        /// Grid レイアウト用のサイズ情報。
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public GridLength FileListRowHeight { get; set; }

        /// <summary>
        /// ログ表示行の高さ。
        /// Grid レイアウト用のサイズ情報。
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public GridLength LogRowHeight { get; set; }

        /// <summary>
        /// プレビュー表示カラムの幅。
        /// Grid レイアウト用のサイズ情報。
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public GridLength PreviewColumnWidth { get; set; }

        /// <summary>
        /// 現在有効なエンコードプロファイル。
        /// 最後に使用したプロファイルを保持するため、JSON には保存しない。
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public Profile CurrentProfile { get; set; }

        /// <summary>
        /// FFmpeg 操作用サービス。
        /// 実行時に生成されるため、設定ファイルには保存されない。
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public FFmpegService? FFmpegService;

        // =====================================================
        // JSON シリアライズ対象（永続情報）
        // =====================================================

        /// <summary>
        /// FFmpegフォルダパス
        /// </summary>
        public string? FFmpegFolderPath { get; set; }

        /// <summary>
        /// FFmpegログ有効フラグ
        /// </summary>
        public bool IsFFmpegEnableLog { get; set; }

        /// <summary>
        /// 並列処理数
        /// </summary>
        public int ParallelCount { get; set; }

        /// <summary>
        /// GPU使用フラグ
        /// </summary>
        public bool UseGpu { get; set; }

        /// <summary>
        /// プロファイル一覧(保存用)
        /// </summary>
        public List<Profile> Profiles { get; set; } = [];

        // =====================================================
        // コンストラクタ
        // =====================================================

        /// <summary>
        /// AppSettings の新しいインスタンスを初期化する。
        /// 初期値の設定、FFmpeg 環境の検出、デフォルトプロファイルの生成を行う。
        /// </summary>
        public AppSettings()
        {
            // -----------------------------------------------
            // 共通・実行時情報の初期化
            // -----------------------------------------------
            AppName             = AppInfo.AppName;
            SettingsFolderPath  = AppPath.GetAppSettingFolderPath();
            WorkFolderPath      = AppPath.GetAppWorkFolderPath();
            ProcessStartTime    = DateTime.Now;

            FFmpegFolderPath    = FFmpegPathService.GetFFmpegFolderPath() ?? "";
            IsFFmpegEnableLog   = false;
            UseGpu              = true;
            ParallelCount       = ConversionParallelLimits.Default;

            // -----------------------------------------------
            // プロファイル初期化
            // -----------------------------------------------
            var profile = new Profile() { IsDefault = true };
            Profiles.Add(profile);
            //Profiles.Add(profile);
            CurrentProfile = profile;

            // FFmpeg フォルダが有効な場合のみサービスを生成
            if (FFmpegPathService.IsFFmpegFolder(FFmpegFolderPath))
            {
                FFmpegService = new FFmpegService(FFmpegFolderPath);
            }

            // -----------------------------------------------
            // UI 表示設定
            // -----------------------------------------------
            ToolbarVisible = true;
            LogVisible = true;
            PreviewVisible = true;
            StatusbarVisible = true;
            FileListRowHeight = new GridLength(1, GridUnitType.Star);
            LogRowHeight = new GridLength(220);
            PreviewColumnWidth = new GridLength(320);
        }

        /// <summary>
        /// 指定された <see cref="ISettings"/> の内容を
        /// 現在のインスタンスへコピーする。
        /// 
        /// ViewModel / Model を問わず共通で使用できる
        /// <see cref="SettingsMapper"/> を利用して
        /// 設定値の同期を行う。
        /// </summary>
        /// <param name="source">コピー元となる設定オブジェクト</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> が null の場合にスローされる
        /// </exception>
        public void CopyFrom(ISettings source)
        {
            // null ガード
            ArgumentNullException.ThrowIfNull(source);

            // 共通マッパーを使用して設定値をコピー
            SettingsMapper.CopyTo(source, this);
        }
    }
}
