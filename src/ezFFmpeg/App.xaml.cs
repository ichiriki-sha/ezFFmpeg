using ezFFmpeg.Common;
using ezFFmpeg.Models.Encoder;
using ezFFmpeg.Services.Dialog;
using ezFFmpeg.Services.FFmpeg;
using ezFFmpeg.Views;
using ezFFmpeg.Models.Profiles;
using System.Configuration;
using System.Data;
using System.IO;
using System.Runtime;
using System.Windows;

namespace ezFFmpeg
{
    /// <summary>
    /// アプリケーションのエントリーポイント。
    /// WPFの App.xaml に対応し、起動時の初期化処理を担当します。
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 単一インスタンスを保証するための Mutex。
        /// </summary>
        private static Mutex? _mutex;

        /// <summary>
        /// アプリケーション全体で共有される設定情報。
        /// </summary>
        AppSettings _setting = null!;

        /// <summary>
        /// 起動時に表示するスプラッシュウィンドウ。
        /// </summary>
        SplashWindow _splash = null!;

        // ------------------------------------------------------------------
        // 単一インスタンス制御
        // ------------------------------------------------------------------

        /// <summary>
        /// Mutex を取得し、単一インスタンスで起動できるかを判定する。
        /// </summary>
        /// <returns>true: 初回起動 / false: 既に起動中</returns>
        private static bool AcquireMutex()
        {
            const string mutexName = "ezFFmpeg_SingleInstanceMutex";
            _mutex = new Mutex(true, mutexName, out bool createdNew);
            return createdNew;
        }

        // ------------------------------------------------------------------
        // 設定・初期化関連
        // ------------------------------------------------------------------

        /// <summary>
        /// 設定ファイルを読み込み、存在しない場合は新規作成する。
        /// </summary>
        private static AppSettings LoadOrCreateSettings()
        {
            var setting = new AppSettings();

            if (File.Exists(AppPath.GetAppSettingPath()))
            {
                setting = AppSettingsManager.LoadSettings();
            }

            return setting;
        }

        /// <summary>
        /// アプリケーションで使用する各種フォルダを作成する。
        /// </summary>
        private static void EnsureFolders(AppSettings setting)
        {
            Directory.CreateDirectory(setting.SettingsFolderPath);
            Directory.CreateDirectory(setting.WorkFolderPath);
            Directory.CreateDirectory(AppPath.GetOutputFolderPath());
        }

        /// <summary>
        /// FFmpeg の設定が正しく行われているかを確認し、
        /// 未設定の場合はダイアログで設定を促す。
        /// </summary>
        /// <returns>true: 継続可能 / false: 起動中断</returns>
        private static bool EnsureFFmpegSetting(AppSettings setting)
        {
            if (!string.IsNullOrWhiteSpace(setting.FFmpegFolderPath) &&
                Directory.Exists(setting.FFmpegFolderPath))
            {
                return true;
            }

            var dialogService = new DialogService();
            var ret = dialogService.ShowOptionDialog(setting);

            if (!ret.IsAccepted)
                return false;

            setting.CopyFrom(ret);
            AppSettingsManager.SaveSettings(setting);

            return true;
        }

        /// <summary>
        /// FFmpeg サービスと各種エンコーダ情報を初期化する。
        /// </summary>
        private static void InitializeFFmpeg(AppSettings setting)
        {
            setting.FFmpegService = new FFmpegService(setting.FFmpegFolderPath!);

            VideoEncoders.Initialize(setting.FFmpegService);
            AudioEncoders.Initialize(setting.FFmpegService);
        }

        /// <summary>
        /// プロファイルを初期化し、使用するプロファイルを決定する。
        /// </summary>
        private static void InitializeProfiles(AppSettings setting)
        {
            if (setting.Profiles.Count == 0)
            {
                setting.Profiles = BuiltInProfileProvider.CreateDefaults(setting.UseGpu);
            }

            var profileManager = new ProfileManager(setting.Profiles);
            setting.CurrentProfile = profileManager.GetPreferredProfile();
        }

        // ------------------------------------------------------------------
        // UI 制御
        // ------------------------------------------------------------------

        /// <summary>
        /// メインウィンドウを表示する。
        /// </summary>
        private static void ShowMainWindow(AppSettings setting)
        {
            var mainWindow = new MainWindow(setting);
            mainWindow.Show();
        }

        /// <summary>
        /// 作業フォルダ内の一時ファイルを削除する。
        /// </summary>
        private static void CleanupWorkFolder(AppSettings setting)
        {
            foreach (var file in Directory.GetFiles(setting.WorkFolderPath))
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    // 削除失敗は無視
                }
            }
        }

        /// <summary>
        /// Splash 画面の進捗バーを更新する。
        /// </summary>
        private void UpdateSplashProgress(string message, int from, int to)
        {
            if (_splash == null)
            {
                _splash = new SplashWindow();
                _splash.Show();
            }

            _splash.ViewModel.Message = message;

            for (int i = from; i <= to; i++)
            {
                _splash.ViewModel.Progress = i;
                Thread.Sleep(10); // 10ms なら 1% = 10ms → 100% で 1秒
            }
        }

        // ------------------------------------------------------------------
        // アプリケーションライフサイクル
        // ------------------------------------------------------------------

        /// <summary>
        /// アプリケーション起動時に呼ばれる。
        /// 
        /// Splash 表示 → 初期化 → MainWindow 表示
        /// という流れを制御する。
        /// </summary>
        protected override async void OnStartup(StartupEventArgs e)
        {

            // 重複起動の抑止
            if (!AcquireMutex())
            {
                Shutdown();
                return;
            }

            base.OnStartup(e);

            // -----------------------------
            // Splash 表示
            // -----------------------------
            UpdateSplashProgress("起動中...", 0, 10);

            // 描画を即時反映
            await Dispatcher.InvokeAsync(() => { }, System.Windows.Threading.DispatcherPriority.Background);

            // -----------------------------
            // 重い初期化処理
            // -----------------------------

            await Task.Run(() =>
            {

                // 設定のロード
                UpdateSplashProgress("設定を読み込み中...", 10, 20);
                _setting = LoadOrCreateSettings();

                // 作業フォルダを作成
                UpdateSplashProgress("作業フォルダを作成中...", 20, 30);
                EnsureFolders(_setting);

                // FFmpeg 設定確認
                UpdateSplashProgress("FFmpeg を確認中...", 30, 40);
                if (!EnsureFFmpegSetting(_setting))
                {
                    Shutdown();
                    return;
                }

                // FFmpeg / Encoder 初期化
                UpdateSplashProgress("FFmpeg を初期化中...", 40, 80);
                InitializeFFmpeg(_setting);

                // プロファイル初期化
                UpdateSplashProgress("プロファイルを準備中...", 80, 90);
                InitializeProfiles(_setting);
            });

            // メインウィンドウ表示
            ShowMainWindow(_setting);

            // 準備完了
            UpdateSplashProgress("準備完了...", 90, 100);
            _splash.Close();
        }

        /// <summary>
        /// アプリケーション終了時の後処理。
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (_setting != null)
            {
                CleanupWorkFolder(_setting);
            }

            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
        }
    }
}