using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Interfaces
{
    internal interface IPreset
    {
        /// <summary>
        /// プリセットの識別子。FFmpegで使用される内部名称などを返す。
        /// </summary>
        string Value { get; }

        /// <summary>
        /// プリセットの表示名や説明。UI表示用などに利用される。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 変更ありかを表すかどうか。
        /// true の場合、FFmpeg の source が使用される。
        /// </summary>
        bool IsSource { get; }

    }
}
