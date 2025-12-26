using ezFFmpeg.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Codec
{
    /// <summary>
    /// オーディオコーデックを表すクラス。
    /// </summary>
    public sealed class AudioCodec : ICodec
    {
        /// <summary>
        /// コーデックの内部識別名（例: "aac", "mp3"）。
        /// </summary>
        public string Codec { get; }

        /// <summary>
        /// ユーザー向け表示名（例: "AAC", "MP3"）。
        /// </summary>
        public string Name { get; }

        public bool IsCopy { get; }

        /// <summary>
        /// AudioCodec のコンストラクタ。
        /// </summary>
        /// <param name="codec">内部識別名</param>
        /// <param name="name">ユーザー向け表示名</param>
        public AudioCodec(string codec, string name, bool isCopy)
        {
            Codec = codec;
            Name = name;
            IsCopy = isCopy;
        }
    }
}
