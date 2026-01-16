using ezFFmpeg.Models.Profiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.DialogResults
{
    /// <summary>
    /// パラメータ入力ダイアログの結果を表すクラス
    /// Profile クラスを継承しているため、選択または編集されたプロファイル情報を保持する
    /// </summary>
    public class ParameterDialogResult : Profile
    {
        /// <summary>
        /// ダイアログが承認（OK）されたかどうか
        /// </summary>
        public bool IsAccepted { get; set; } = false;
    }
}
