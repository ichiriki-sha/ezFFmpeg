using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Media
{
    /// <summary>
    /// メディアの種類を表す列挙型。
    /// ビデオ、オーディオ、字幕などのメディアタイプの分類に使用する。
    /// </summary>
    public enum MediaType
    {
        /// <summary>
        /// メディア種類が不明な場合に使用。
        /// </summary>
        Unknown,

        /// <summary>
        /// ビデオ（映像）タイプ。
        /// </summary>
        Video,

        /// <summary>
        /// オーディオ（音声）タイプ。
        /// </summary>
        Audio,

        /// <summary>
        /// 字幕タイプ。
        /// </summary>
        Subtitle
    }
}
