using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Codec
{
    /// <summary>
    /// 標準的なビデオコーデックを定義する静的クラス。
    /// </summary>
    public static class VideoCodecs
    {
        /// <summary>
        /// 映像をそのままコピーするコーデック。
        /// </summary>
        public static readonly VideoCodec Copy = new("copy", "COPY", true);

        /// <summary>
        /// H.264 / AVC コーデック。
        /// 高圧縮率で広く使用される。
        /// </summary>
        public static readonly VideoCodec H264 = new("h264", "H.264", false);

        /// <summary>
        /// H.265 / HEVC コーデック。
        /// H.264より高圧縮率。
        /// </summary>
        public static readonly VideoCodec Hevc = new("hevc", "H.265", false);

        // 将来的に AV1 コーデックを追加可能
        // public static readonly VideoCodec AV1 = new("av1", "AV1", false);
    }
}
