using ezFFmpeg.Common;
using ezFFmpeg.Models.Interfaces;
using ezFFmpeg.Models.Profiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.DialogResults
{
    /// <summary>
    /// 設定ダイアログやオプションダイアログの結果を表すクラス
    /// ユーザが OK/キャンセル を選択した際の情報を保持
    /// </summary>
    public class OptionDialogResult : ISettings
    {
        /// <summary>
        /// ダイアログが承認（OK）されたかどうか
        /// </summary>
        public bool IsAccepted { get; set; } = false;

        /// <summary>
        /// FFmpeg のフォルダパス
        /// </summary>
        public string? FFmpegFolderPath { get; set; }

        /// <summary>
        /// FFmpeg のログを有効にするかどうか
        /// </summary>
        public bool IsFFmpegEnableLog { get; set; }

        /// <summary>
        /// 並列処理の最大数
        /// </summary>
        public int ParallelCount { get; set; }

        /// <summary>
        /// GPU を使用するかどうか
        /// </summary>
        public bool UseGpu { get; set; }

        /// <summary>
        /// 設定されたプロファイル一覧
        /// </summary>
        public List<Profile> Profiles { get; set; } = [];

    }
}
