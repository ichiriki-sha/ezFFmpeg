using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace ezFFmpeg.Models.Common
{
    /// <summary>
    /// 外部プロセス実行の結果を表すクラス
    /// </summary>
    public class ProcessResult
    {
        /// <summary>
        /// プロセスが正常終了したかどうか
        /// </summary>
        public  bool Success { get; set; }

        /// <summary>
        /// プロセスの標準出力（stdout）の内容
        /// </summary>
        public string? StdOut { get; set; } 

        /// <summary>
        /// プロセスの標準エラー出力（stderr）の内容
        /// </summary>
        public string? StdErr { get; set; } 

        /// <summary>
        /// プロセス終了コード
        /// </summary>
        public int ExitCode { get; set; }

        /// <summary>
        /// ProcessResult のコンストラクタ
        /// </summary>
        /// <param name="stdOut">標準出力の内容</param>
        /// <param name="stdErr">標準エラー出力の内容</param>
        /// <param name="exitCode">プロセス終了コード</param>
        public ProcessResult(string? stdOut, string? stdErr, int exitCode)
        {

            Success = exitCode == 0; // 終了コードが0なら成功とみなす
            StdOut = stdOut;
            StdErr = stdErr;
            ExitCode = exitCode;
        }
    }
}
