using ezFFmpeg.Models.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Services.Conversion
{
    public class ConversionProgress
    {
        /// <summary>
        /// 全体の進捗率（0.0 ～ 100.0）
        /// </summary>
        public double TotalPercent { get; init; }

        /// <summary>
        /// 現在処理中のファイル名
        /// </summary>
        public string? CurrentFileName { get; init; }

        /// <summary>
        /// 現在処理中のファイル番号（1始まり）
        /// </summary>
        public int CurrentIndex { get; init; }

        public ProcessingStatus CurrentStatus = ProcessingStatus.Pending;
    }
}
