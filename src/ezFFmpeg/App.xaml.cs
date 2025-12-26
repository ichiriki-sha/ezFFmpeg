using ezFFmpeg.Common;
using ezFFmpeg.Models.Encoder;
using ezFFmpeg.Services.Dialog;
using ezFFmpeg.Services.FFmpeg;
using ezFFmpeg.Services.Interfaces;
using ezFFmpeg.Views;
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

        /// <summary>
        /// アプリケーション起動時に呼ばれるイベントハンドラー。
        /// 単一インスタンスの確認、設定の読み込み、必要フォルダの作成、
        /// FFmpeg設定確認、エンコーダ初期化、メインウィンドウ表示を行います。
        /// </summary>
        /// <param name="e">起動引数</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            const string mutexName = "ezFFmpeg_SingleInstanceMutex";

            // すでに起動しているか確認
            _mutex = new Mutex(true, mutexName, out bool createdNew);

            if (!createdNew)
            {
                // すでに起動している場合はアプリを終了
                // MessageBoxで通知も可能（現在はコメントアウト）
                // MessageBox.Show("ezFFmpeg はすでに起動しています。",
                //                 "重複起動",
                //                 MessageBoxButton.OK,
                //                 MessageBoxImage.Information);

                Shutdown();
                return;
            }

            base.OnStartup(e);

            // 設定を読み込む
            var setting = AppSettingsManager.LoadSettings();

            // 必要なフォルダが存在しない場合は作成
            if (!Directory.Exists(setting.SettingsFolderPath))
                Directory.CreateDirectory(setting.SettingsFolderPath);

            if (!Directory.Exists(setting.WorkFolderPath))
                Directory.CreateDirectory(setting.WorkFolderPath);

            // FFmpegフォルダが未設定、または無効な場合は設定ダイアログを表示
            if (string.IsNullOrWhiteSpace(setting.FFmpegFolderPath) ||
               !FFmpegPathService.IsFFmpegFolder(setting.FFmpegFolderPath))
            {
                DialogService dialogService = new();
                var ret = dialogService.ShowOptionDialog(setting);

                if (ret.IsAccepted)
                {
                    // ユーザーが設定を承認した場合、設定を反映
                    setting.CopyFrom(ret);
                    setting.FFmpegService = new(ret.FFmpegFolderPath!);

                    // 設定を保存
                    AppSettingsManager.SaveSettings(setting);
                }
            }

            // FFmpegService が設定されていなければアプリを終了
            if (setting.FFmpegService == null)
            {
                Shutdown();
                return;
            }

            // エンコーダの初期化
            VideoEncoders.Initialize(setting.FFmpegService);
            AudioEncoders.Initialize(setting.FFmpegService);

            // メインウィンドウを作成・表示
            var mainWindow = new MainWindow(setting);
            mainWindow.Show();
        }
    }
}
