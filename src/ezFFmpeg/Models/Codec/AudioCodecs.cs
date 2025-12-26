using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Codec
{
    /// <summary>
    /// 標準的なオーディオコーデックの定義を提供する静的クラス。
    /// </summary>
    public static class AudioCodecs
    {
        /// <summary>
        /// 元のオーディオストリームをそのままコピーするコーデック。
        /// </summary>
        public static readonly AudioCodec Copy = new("copy", "COPY", true);

        /// <summary>
        /// AAC（Advanced Audio Coding）コーデック。
        /// </summary>
        public static readonly AudioCodec Aac = new("aac", "AAC", false);

        /// <summary>
        /// ALAC（Apple Lossless Audio Codec）コーデック。
        /// </summary>
        public static readonly AudioCodec Alac = new("alac", "ALAC", false);

        /// <summary>
        /// PCM（Pulse Code Modulation）コーデック。
        /// </summary>
        //public static readonly AudioCodec PCM = new("pcm", "PCM", false);

        /// <summary>
        /// MP3（MPEG-1 Audio Layer III）コーデック。
        /// </summary>
        public static readonly AudioCodec Mp3 = new("mp3", "MP3", false);

        /// <summary>
        /// FLAC（Free Lossless Audio Codec）コーデック。
        /// </summary>
        public static readonly AudioCodec Flac = new("flac", "FLAC", false);
    }
}
