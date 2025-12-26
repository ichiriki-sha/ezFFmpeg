using ezFFmpeg.Models.Codec;
using ezFFmpeg.Models.Interfaces;
using ezFFmpeg.Services.FFmpeg;
using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Encoder
{
    /// <summary>
    /// オーディオエンコーダを表すクラス
    /// </summary>
    public sealed class AudioEncoder : IEncoder
    {
        /// <summary>
        /// オーディオエンコーダを表すクラス。
        /// FFmpeg による音声エンコード処理で使用される
        /// エンコーダ情報を保持する。
        /// </summary>
        public string Encoder { get; }

        /// <summary>
        /// エンコーダの表示名。
        /// UI 上でユーザーに表示するための名称を返す。
        /// </summary>
        /// <example>AAC</example>
        /// <example>MP3</example>
        public string Name { get; }

        /// <summary>
        /// このエンコーダが生成するオーディオコーデック。
        /// エンコード結果の形式を表す。
        /// </summary>
        public ICodec Codec { get; }

        /// <summary>
        /// このエンコーダが現在の環境で使用可能かどうか。
        /// FFmpeg のビルド構成や外部ライブラリの有無によって決定される。
        /// </summary>
        public bool CanUse { get; set; }

        /// <summary>
        /// ストリームコピー（再エンコードなし）を表すかどうか。
        /// 対応する <see cref="Codec"/> の設定に委譲する。
        /// </summary>
        public bool IsCopy 
        { 
            get =>  Codec.IsCopy; 
        }

        /// <summary>
        /// <see cref="AudioEncoder"/> の新しいインスタンスを初期化する。
        /// </summary>
        /// <param name="encoder">
        /// エンコーダ識別子（FFmpeg の <c>-c:a</c> で指定する値）
        /// </param>
        /// <param name="name">表示名</param>
        /// <param name="codec">対応するオーディオコーデック</param>
        public AudioEncoder(string encoder,
                            string name,
                            AudioCodec codec)
        {
            Encoder = encoder;
            Name = name;
            Codec = codec;

            CanUse = false;
        }
    }
}
