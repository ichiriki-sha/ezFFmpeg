using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Common
{
    /// <summary>
    /// パラメータ入力ダイアログのモードを表す列挙型。
    /// ダイアログの用途に応じてモードを切り替えることで、UI や動作を制御する。
    /// </summary>
    public enum ParameterDialogMode
    {
        /// <summary>
        /// 新しいプロファイルを登録するためのモード
        /// </summary>
        ProfileAdd,

        /// <summary>
        /// 既存のプロファイルを編集するためのモード
        /// </summary>
        ProfileEdit,

        /// <summary>
        /// 既存のプロファイルを削除するためのモード
        /// </summary>
        ProfileRemove,

        /// <summary>
        /// 変換実行時にパラメータを入力するためのモード
        /// </summary>
        RunParameter
    }
}
