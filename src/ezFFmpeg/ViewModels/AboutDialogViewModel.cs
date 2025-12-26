using ezFFmpeg.Common;
using ezFFmpeg.Helpers;
using ezFFmpeg.Models.Common;
using System.Windows.Input;

namespace ezFFmpeg.ViewModels
{
    /// <summary>
    /// 「About」ダイアログの ViewModel
    /// アプリケーション情報やバージョン情報を提供し、ダイアログの閉じる操作を管理する
    /// </summary>
    public class AboutDialogViewModel : BindableBase
    {
        /// <summary>
        /// ダイアログを閉じるリクエストイベント
        /// 引数は閉じるかどうかの結果 (bool?) を返す
        /// </summary>
        public event Action<bool?>? RequestClose;

        /// <summary>
        /// アプリケーション名
        /// </summary>
        public static string AppName => AppInfo.AppName;

        /// <summary>
        /// アプリケーションのバージョン文字列
        /// </summary>
        public static string Version => $"Version {AppInfo.AppVersion}";

        /// <summary>
        /// アプリケーションの簡単な説明
        /// </summary>
        public static string Description =>
            "FFmpeg を簡単に使うための GUI フロントエンド";

        /// <summary>
        /// 著作権情報
        /// </summary>
        public static string Copyright =>
            "© 2025 ichiriki-sha";

        /// <summary>
        /// 使用ライブラリ情報
        /// </summary>
        public static string LibraryInfo =>
            "Powered by FFmpeg";

        private string _versionInfo = "";

        /// <summary>
        /// FFmpeg のバージョン情報
        /// </summary>
        public string VersionInfo
        {
            get => _versionInfo;
            set => SetProperty(ref _versionInfo, value);
        }

        private readonly AppSettings _settings;

        /// <summary>
        /// ダイアログを閉じるコマンド
        /// </summary>
        public ICommand CloseCommand { get; }

        /// <summary>
        /// コンストラクタ
        /// FFmpeg サービスからバージョン情報を取得し、CloseCommand を初期化する
        /// </summary>
        /// <param name="settings">アプリケーション設定</param>
        public AboutDialogViewModel(AppSettings settings)
        {
            _settings = settings;

            // FFmpeg バージョン情報の取得
            _versionInfo = _settings.FFmpegService!.FFmpegVersion;

            CloseCommand = new RelayCommand(Close);
        }

        /// <summary>
        /// ダイアログを閉じる処理を呼び出す
        /// </summary>
        private void Close()
        {
            RequestClose?.Invoke(true);
        }
    }
}
