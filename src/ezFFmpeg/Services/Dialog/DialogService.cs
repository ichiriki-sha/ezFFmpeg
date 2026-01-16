using ezFFmpeg.Common;
using ezFFmpeg.Models.Common;
using ezFFmpeg.Models.DialogResults;
using ezFFmpeg.Models.Profiles;
using ezFFmpeg.Services.Interfaces;
using ezFFmpeg.Views;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace ezFFmpeg.Services.Dialog
{
    /// <summary>
    /// ダイアログ表示サービス。
    /// MessageBox、フォルダ選択、ファイル選択、オプション・パラメータ・Aboutダイアログの表示を提供します。
    /// </summary>
    public class DialogService(Window? owner = null) : IDialogService, IOwnerAware
    {
        /// <summary>
        /// 所有者ウィンドウ。ダイアログ表示時に親ウィンドウとして使用されます。
        /// </summary>
        public Window? Owner { get; set; } = owner;

        /// <summary>
        /// メッセージボックスを表示します。
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="title">ウィンドウタイトル</param>
        /// <param name="button">表示するボタン</param>
        /// <param name="icon">表示するアイコン</param>
        /// <returns>ユーザーが選択したボタン</returns>
        public MessageBoxResult ShowMessageBox(string message,
                                               string title,
                                               MessageBoxButton button = MessageBoxButton.OK,
                                               MessageBoxImage icon = MessageBoxImage.None)
        {
            return MessageBox.Show(Owner, message, title, button, icon);
        }

        /// <summary>
        /// フォルダ選択ダイアログを表示します。
        /// </summary>
        /// <param name="initialDirectory">初期ディレクトリ</param>
        /// <returns>選択されたフォルダパス、キャンセル時は null</returns>
        public string? ShowFolderDialog(string? initialDirectory = null)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true, // フォルダ選択モード
                Title = "フォルダを選択してください",
                AllowNonFileSystemItems = false,
                InitialDirectory = initialDirectory,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true
            };

            if (Owner != null)
            {
                var hwnd = new WindowInteropHelper(Owner).Handle;
                if (dialog.ShowDialog(hwnd) == CommonFileDialogResult.Ok)
                {
                    return dialog.FileName;
                }
            }
            else
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    return dialog.FileName;
                }
            }

            return null;
        }

        /// <summary>
        /// ファイル選択ダイアログを表示します。
        /// </summary>
        /// <param name="initialDirectory">初期ディレクトリ</param>
        /// <param name="multiselect">複数選択を許可するか</param>
        /// <returns>選択されたファイルパスのリスト</returns>
        public List<string> ShowFileDialog(string? initialDirectory = null, bool multiselect = true)
        {
            string exts = SupportedExtensions.Extensions.Replace(",", ";");
            List<string> ret = new List<string>();

            var dialog = new CommonOpenFileDialog
            {
                Title = "ファイルを開く",
                IsFolderPicker = false,
                InitialDirectory = initialDirectory,
                Multiselect = multiselect
            };

            dialog.Filters.Add(new CommonFileDialogFilter("動画ファイル", exts));
            dialog.Filters.Add(new CommonFileDialogFilter("すべてのファイル", "*.*"));

            if (Owner != null)
            {
                var hwnd = new WindowInteropHelper(Owner).Handle;
                if (dialog.ShowDialog(hwnd) != CommonFileDialogResult.Ok)
                    return ret;
            }
            else
            {
                if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                    return ret;
            }

            if (multiselect)
            {
                ret.AddRange(dialog.FileNames);
            }
            else
            {
                ret.Add(dialog.FileName);
            }

            return ret;
        }

        /// <summary>
        /// オプション設定ダイアログを表示します。
        /// </summary>
        /// <param name="settings">アプリ設定</param>
        /// <returns>ユーザーの選択結果</returns>
        public OptionDialogResult ShowOptionDialog(AppSettings settings)
        {
            var dialog = new OptionDialog(settings);

            if (Owner != null)
            {
                dialog.Owner = Owner;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            dialog.ShowDialog();

            return dialog.ViewModel.Result;
        }

        /// <summary>
        /// パラメータ設定ダイアログを表示します。
        /// </summary>
        /// <param name="settings">アプリ設定</param>
        /// <param name="mode">パラメータダイアログのモード</param>
        /// <param name="profile">対象プロファイル</param>
        /// <returns>ユーザーの選択結果</returns>
        public ParameterDialogResult ShowParameterDialog(AppSettings settings, ParameterDialogMode mode, Profile profile)
        {
            var dialog = new ParameterDialog(settings, mode, profile)
            {
                Owner = Owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            dialog.ShowDialog();

            return dialog.ViewModel.Result;
        }

        /// <summary>
        /// About ダイアログを表示します。
        /// </summary>
        /// <param name="settings">アプリ設定</param>
        public void ShowAboutDialog(AppSettings settings)
        {
            var dialog = new AboutDialog(settings)
            {
                Owner = Owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            dialog.ShowDialog();
        }
    }
}
