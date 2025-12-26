using ezFFmpeg.Models.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Interfaces
{
    public interface ISettings
    {
        /// <summary>
        /// FFmpegフォルダパス
        /// </summary>
        string? FFmpegFolderPath { get; set; }

        /// <summary>
        /// FFmpegログ有効フラグ
        /// </summary>
        bool IsFFmpegEnableLog { get; set; }

        /// <summary>
        /// 並列処理数
        /// </summary>
        int ParallelCount { get; set; }

        /// <summary>
        /// GPU使用フラグ
        /// </summary>
        bool UseGpu { get; set; }

        /// <summary>
        /// プロファイル一覧(保存用)
        /// </summary>
        List<Profile> Profiles { get; set; }
    }
}
