using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Interfaces
{
    /// <summary>
    /// コーデックを表すインターフェイス。
    /// FFmpeg によるエンコード／デコードで使用される
    /// 動画・音声コーデックの基本情報を提供する。
    /// </summary>
    public interface ICodec
    {
        /// <summary>
        /// コーデック識別子。FFmpegで使用される内部名称などを返す。
        /// 例: "h264", "aac" など。
        /// </summary>
        string Codec { get; }

        /// <summary>
        /// コーデックの表示名や説明。UI表示用などに利用される。
        /// 例: "H.264 / AVC" など。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// ストリームコピー（再エンコードなし）を表すかどうか。
        /// true の場合、FFmpeg の <c>-c copy</c> が使用される。
        /// </summary>
        bool IsCopy { get; }
    }
}