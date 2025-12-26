using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Interfaces
{
    /// <summary>
    /// エンコーダを表すインターフェイス。
    /// 動画・音声のエンコードに使用されるエンコーダ情報を提供する。
    /// </summary>
    public interface IEncoder
    {
        /// <summary>
        /// エンコーダの識別子。
        /// FFmpegで使用される内部名称などを返す。
        /// 例: "libx264", "aac" など。
        /// </summary>
        string Encoder { get; }

        /// <summary>
        /// エンコーダの表示名や説明。UI表示用などに利用される。
        /// 例: "H.264 / x264" など。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// エンコード対象のコーデック情報。
        /// エンコーダがどのコーデックを使用しているかを取得する。
        /// </summary>
        ICodec Codec { get; }

        /// <summary>
        /// エンコーダが使用可能かどうか。
        /// FFmpeg環境やライブラリの有無により true/false が変わる。
        /// 実行時にチェックされ、UIで選択可能かどうかを制御するのに使う。
        /// </summary>
        bool CanUse { get; set; }

        /// <summary>
        /// ストリームコピー（再エンコードなし）を表すかどうか。
        /// true の場合、FFmpeg の <c>-c copy</c> が指定される。
        /// </summary>
        bool IsCopy { get; }
    }
}
