using ezFFmpeg.Common;
using ezFFmpeg.Models.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ezFFmpeg.ViewModels
{
    /// <summary>
    /// スプラッシュ画面用の ViewModel。
    /// 起動中に表示するメッセージ、進捗率、バージョン情報を View に提供します。
    /// </summary>
    public class SplashWindowViewModel : BindableBase
    {
        // ------------------------------------------------------------------
        // 起動メッセージ
        // ------------------------------------------------------------------

        /// <summary>
        /// スプラッシュ画面に表示するメッセージ。
        /// 初期状態では「起動中...」を表示します。
        /// </summary>
        private string _message = "起動中...";

        /// <summary>
        /// 現在の状態を表すメッセージ。
        /// 設定読み込み中・初期化中などに更新されます。
        /// </summary>
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        // ------------------------------------------------------------------
        // 進捗率
        // ------------------------------------------------------------------

        /// <summary>
        /// 起動処理の進捗率（0～100）。
        /// ProgressBar にバインドされます。
        /// </summary>
        private int _progress = 0;

        /// <summary>
        /// 起動処理の進捗率。
        /// 値が変更されると PropertyChanged が通知されます。
        /// </summary>
        public int Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        // ------------------------------------------------------------------
        // バージョン情報
        // ------------------------------------------------------------------

        /// <summary>
        /// アプリケーションのバージョン文字列。
        /// 例: "Version 1.2.3"
        /// </summary>
        public string Version { get; } = $"Version {AppInfo.AppVersion}";
    }
}
