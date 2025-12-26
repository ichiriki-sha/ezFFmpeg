using ezFFmpeg.Models.Codec;
using ezFFmpeg.Models.Interfaces;
using ezFFmpeg.Models.Quality;
using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Encoder
{
    /// <summary>
    /// ビデオエンコーダを表すクラス。
    /// FFmpeg による動画エンコード処理で使用される
    /// エンコーダ情報を保持する。
    /// </summary>
    public sealed class VideoEncoder : IEncoder
    {
        /// <summary>
        /// エンコーダ識別子。
        /// FFmpeg の <c>-c:v</c> オプションで指定される内部名称を返す。
        /// </summary>
        /// <example>libx264</example>
        /// <example>h264_nvenc</example>
        public string Encoder { get; }

        /// <summary>
        /// エンコーダの表示名。
        /// UI 上でユーザーに表示するための名称を返す。
        /// </summary>
        /// <example>H.264</example>
        /// <example>H.264(NVENC)</example>
        public string Name { get; }

        /// <summary>
        /// このエンコーダが生成するビデオコーデック。
        /// エンコード結果の形式を表す。
        /// </summary>
        public ICodec Codec { get; }

        /// <summary>
        /// エンコーダの実行方式。
        /// CPU ベースか GPU ベースかを表す。
        /// </summary>
        public EncoderType Type { get; }

        /// <summary>
        /// NVIDIA NVENC に対応したエンコーダかどうか。
        /// </summary>
        public bool IsNvenc { get; }

        /// <summary>
        /// Intel Quick Sync Video (QSV) に対応したエンコーダかどうか。
        /// </summary>
        public bool IsQsv { get; }

        /// <summary>
        /// AMD Advanced Media Framework (AMF) に対応したエンコーダかどうか。
        /// </summary>
        public bool IsAmf { get; }

        /// <summary>
        /// Gpuを使用するかどうか。
        /// </summary>
        public bool UseGpu { get; }

        /// <summary>
        /// このエンコーダが現在の環境で使用可能かどうか。
        /// FFmpeg のビルド構成、GPU ドライバ、実行環境により決定される。
        /// </summary>
        public bool CanUse { get; set; }

        /// <summary>
        /// 品質ティアと FFmpeg 用の品質レベルを対応付けるマップ。
        /// </summary>
        private readonly Dictionary<QualityTier, QualityLevel> _qualityMap;

        /// <summary>
        /// ストリームコピー（再エンコードなし）を表すかどうか。
        /// 対応する <see cref="Codec"/> の設定に委譲する。
        /// </summary>
        public bool IsCopy
        {
            get => Codec.IsCopy;
        }

        /// <summary>
        /// <see cref="VideoEncoder"/> の新しいインスタンスを初期化する。
        /// </summary>
        /// <param name="encoder">
        /// エンコーダ識別子（FFmpeg の <c>-c:v</c> で指定する値）
        /// </param>
        /// <param name="name">表示名</param>
        /// <param name="codec">対応するビデオコーデック</param>
        /// <param name="type">エンコーダタイプ（CPU / GPU）</param>
        /// <param name="qualityMap">
        /// 品質ティアと品質レベルの対応辞書。
        /// </param>
        public VideoEncoder(string encoder,
                            string name,
                            VideoCodec codec,
                            EncoderType type,
                            Dictionary<QualityTier, QualityLevel> qualityMap)
        {
            Encoder = encoder;
            Name = name;
            Codec = codec;
            Type = type;
            _qualityMap = qualityMap;

            // GPU 系統の判定
            IsNvenc = encoder.EndsWith("_nvenc", StringComparison.OrdinalIgnoreCase);
            IsQsv = encoder.EndsWith("_qsv", StringComparison.OrdinalIgnoreCase);
            IsAmf = encoder.EndsWith("_amf", StringComparison.OrdinalIgnoreCase);
            UseGpu= IsNvenc || IsQsv || IsAmf;
            CanUse = false;
        }

        /// <summary>
        /// 指定した品質ティアに対応する品質レベルを取得する。
        /// </summary>
        /// <param name="tier">品質ティア</param>
        /// <returns>対応する品質レベル</returns>
        /// <exception cref="KeyNotFoundException">
        /// 指定した品質ティアがマップに存在しない場合にスローされる
        /// </exception>
        public QualityLevel GetQuality(QualityTier tier)
                => _qualityMap[tier];
    }
}
