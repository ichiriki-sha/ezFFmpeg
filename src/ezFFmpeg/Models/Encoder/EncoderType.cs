using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Encoder
{
    /// <summary>
    /// エンコーダの種類を表す列挙型
    /// </summary>
    public enum EncoderType
    {
        /// <summary>
        /// CPU を使用するソフトウェアエンコーダ
        /// </summary>
        Cpu,

        /// <summary>
        /// GPU を使用するハードウェアエンコーダ
        /// </summary>
        Gpu
    }
}
