using ezFFmpeg.Common;
using ezFFmpeg.Models.DialogResults;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using ezFFmpeg.Models.Common;
using ezFFmpeg.Models.Profiles;

namespace ezFFmpeg.Services.Interfaces
{
    /// <summary>
    /// ダイアログ表示サービスのインターフェース。
    /// メッセージボックス、フォルダ/ファイル選択、各種カスタムダイアログを提供します。
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// メッセージボックスを表示します。
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="title">メッセージボックスのタイトル</param>
        /// <param name="button">ボタンの種類 (OK/Cancel など)</param>
        /// <param name="icon">アイコンの種類</param>
        /// <returns>ユーザーが選択したボタンの結果</returns>
        MessageBoxResult ShowMessageBox(
            string message,
            string title,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxImage icon = MessageBoxImage.None);

        /// <summary>
        /// フォルダ選択ダイアログを表示します。
        /// </summary>
        /// <param name="initialDirectory">初期表示するディレクトリ (省略可能)</param>
        /// <returns>選択したフォルダのパス、キャンセル時は null</returns>
        string? ShowFolderDialog(string? initialDirectory = null);

        /// <summary>
        /// ファイル選択ダイアログを表示します。
        /// </summary>
        /// <param name="initialDirectory">初期表示するディレクトリ (省略可能)</param>
        /// <param name="multiselect">複数選択を許可するか</param>
        /// <returns>選択したファイルパスのリスト</returns>
        List<string> ShowFileDialog(string? initialDirectory = null, bool multiselect = true);

        /// <summary>
        /// オプション設定用のカスタムダイアログを表示します。
        /// </summary>
        /// <param name="settings">現在のアプリ設定</param>
        /// <returns>ユーザーの選択結果を含む OptionDialogResult</returns>
        OptionDialogResult ShowOptionDialog(AppSettings settings);

        /// <summary>
        /// パラメータ設定用のカスタムダイアログを表示します。
        /// </summary>
        /// <param name="settings">現在のアプリ設定</param>
        /// <param name="mode">ダイアログのモード (追加/編集など)</param>
        /// <param name="profile">対象のプロファイル情報</param>
        /// <returns>ユーザーの入力結果を含む ParameterDialogResult</returns>
        ParameterDialogResult ShowParameterDialog(AppSettings settings, ParameterDialogMode mode, Profile profile);

        /// <summary>
        /// アプリの「バージョン情報」などの About ダイアログを表示します。
        /// </summary>
        /// <param name="settings">現在のアプリ設定</param>
        void ShowAboutDialog(AppSettings settings);
    }
}
