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
        /// シングルトンインスタンスを保証するためのMutex。
        /// </summary>
        private static Mutex? _mutex;

        // ------------------------------------------------------------------
        // 起動補助メソッド
        // ------------------------------------------------------------------
        private bool AcquireMutex()
        {
            const string mutexName = "ezFFmpeg_SingleInstanceMutex";
            _mutex = new Mutex(true, mutexName, out bool createdNew);
            return createdNew;
        }

        private AppSettings LoadOrCreateSettings(out bool loaded)
        {
            loaded = false;
            var setting = new AppSettings();

            if (File.Exists(AppPath.GetAppSettingPath()))
            {
                setting = AppSettingsManager.LoadSettings();
                loaded = true;
            }

            return setting;
        }

        private void EnsureFolders(AppSettings setting)
        {
            Directory.CreateDirectory(setting.SettingsFolderPath);
            Directory.CreateDirectory(setting.WorkFolderPath);
            Directory.CreateDirectory(AppPath.GetOutputFolderPath());
        }

        private bool EnsureFFmpegSetting(AppSettings setting)
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

        private void InitializeFFmpeg(AppSettings setting)
        {
            setting.FFmpegService = new FFmpegService(setting.FFmpegFolderPath!);

            VideoEncoders.Initialize(setting.FFmpegService);
            AudioEncoders.Initialize(setting.FFmpegService);
        }

        private void InitializeProfiles(AppSettings setting, bool loaded)
        {
            if (!loaded)
            {
                setting.Profiles = BuiltInProfileProvider.CreateDefaults(setting.UseGpu);
            }

            var profileManager = new ProfileManager(setting.Profiles);
            setting.CurrentProfile = profileManager.GetPreferredProfile();
        }

        private void ShowMainWindow(AppSettings setting)
        {
            var mainWindow = new MainWindow(setting);
            mainWindow.Show();
        }

        private void CleanupWorkFolder(AppSettings setting)
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
        /// アプリケーション起動時に呼ばれるイベントハンドラー。
        /// 単一インスタンスの確認、設定の読み込み、必要フォルダの作成、
        /// FFmpeg設定確認、エンコーダ初期化、メインウィンドウ表示を行います。
        /// </summary>
        /// <param name="e">起動引数</param>
        protected override void OnStartup(StartupEventArgs e)
        {

            // 重複起動の抑止
            if (!AcquireMutex())
            {
                Shutdown();
                return;
            }

            base.OnStartup(e);

            // 設定のロード
            var setting = LoadOrCreateSettings(out bool loaded);

            // フォルダの保証
            EnsureFolders(setting);

            // FFmpeg 設定確認
            if (!EnsureFFmpegSetting(setting))
            {
                Shutdown();
                return;
            }

            // FFmpeg / Encoder 初期化
            InitializeFFmpeg(setting);

            // プロファイル初期化
            InitializeProfiles(setting, loaded);

            // メインウィンドウ表示
            ShowMainWindow(setting);

            // 作業フォルダクリーンアップ
            CleanupWorkFolder(setting);
        }
    }
}