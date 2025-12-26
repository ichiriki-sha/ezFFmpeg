using ezFFmpeg.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Codec
{
    /// <summary>
    /// ビデオコーデックを表すクラス。
    /// ICodec インターフェースを実装し、コーデック識別子と表示名を保持する。
    /// </summary>
    public sealed class VideoCodec : ICodec
    {
        /// <summary>
        /// FFmpegなどで使用されるコーデック識別子（例: "h264", "hevc", "av1"）。
        /// </summary>
        public string Codec { get; }

        /// <summary>
        /// ユーザー向けに表示するコーデック名（例: "H.264 / AVC"）。
        /// </summary>
        public string Name { get; }

        public bool IsCopy { get; }

        /// <summary>
        /// 指定されたコーデック識別子と表示名で VideoCodec インスタンスを初期化する。
        /// </summary>
        /// <param name="codec">コーデック識別子（例: "h264"）</param>
        /// <param name="name">ユーザー向け表示名（例: "H.264 / AVC"）</param>
        public VideoCodec(string codec, string name, bool isCopy)
        {
            Codec = codec;
            Name = name;
            IsCopy = isCopy;
        }
    }
}
