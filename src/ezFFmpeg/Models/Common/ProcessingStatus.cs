using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Common
{
    /// <summary>
    /// ファイルや変換処理の進行状況を表す列挙型
    /// </summary>
    public enum ProcessingStatus
    {
        /// <summary>
        /// 未処理の状態
        /// </summary>
        Pending,

        /// <summary>
        /// 現在処理中の状態
        /// </summary>
        Processing,

        /// <summary>
        /// 処理が正常に完了した状態
        /// </summary>
        Completed,

        /// <summary>
        /// 処理がユーザーまたはシステムによってキャンセルされた状態
        /// </summary>
        Canceled,

        /// <summary>
        /// 処理中にエラーが発生した状態
        /// </summary>
        Error
    }
}
