using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Quality
{
    /// <summary>
    /// エンコード品質レベルのプリセットを定義する静的クラス。
    /// 各エンコーダー・方式ごとに、VeryHigh～Low の品質ティアに対応する <see cref="QualityLevel"/> を保持する。
    /// </summary>
    public static class QualityLevels
    {
        /// <summary>
        /// なし（品質制御を行わない）プリセット
        /// </summary>
        public static readonly Dictionary<QualityTier, QualityLevel> NoneLevels =
            new()
            {
                { QualityTier.VeryHigh, new(QualityType.None, "", 18) },
                { QualityTier.High,     new(QualityType.None, "", 20) },
                { QualityTier.Medium,   new(QualityType.None, "", 23) },
                { QualityTier.Low,      new(QualityType.None, "", 28) },
            };

        /// <summary>
        /// libx264 用の品質プリセット（CRF + スピード設定）
        /// </summary>
        public static readonly Dictionary<QualityTier, QualityLevel> Libx264Levels =
            new()
            {
                { QualityTier.VeryHigh, new(QualityType.Crf, "veryslow" ,18) },
                { QualityTier.High,     new(QualityType.Crf, "slow"     ,20) },
                { QualityTier.Medium,   new(QualityType.Crf, "medium"   ,23) },
                { QualityTier.Low,      new(QualityType.Crf, "fast"     ,28) },
            };

        /// <summary>
        /// h264_nvenc 用の品質プリセット（CQ + NVENC パラメータ）
        /// </summary>
        public static readonly Dictionary<QualityTier, QualityLevel> H264NvencLevels =
            new()
            {
                { QualityTier.VeryHigh, new(QualityType.Cq, "p7"     ,18) },
                { QualityTier.High,     new(QualityType.Cq, "p6"     ,20) },
                { QualityTier.Medium,   new(QualityType.Cq, "p4"     ,23) },
                { QualityTier.Low,      new(QualityType.Cq, "p2"     ,28) },
            };

        /// <summary>
        /// h264_qsv 用の品質プリセット（CQ + QSV パラメータ）
        /// </summary>
        public static readonly Dictionary<QualityTier, QualityLevel> H264QsvLevels =
            new()
            {
                { QualityTier.VeryHigh, new(QualityType.Cq, "slow"   ,18) },
                { QualityTier.High,     new(QualityType.Cq, "medium" ,20) },
                { QualityTier.Medium,   new(QualityType.Cq, "medium" ,23) },
                { QualityTier.Low,      new(QualityType.Cq, "fast"   ,28) },
            };

        /// <summary>
        /// h264_amf 用の品質プリセット（CQ + AMF パラメータ）
        /// </summary>
        public static readonly Dictionary<QualityTier, QualityLevel> H264AmfLevels =
            new()
            {
                { QualityTier.VeryHigh, new(QualityType.Cq, "quality"  ,18) },
                { QualityTier.High,     new(QualityType.Cq, "balanced" ,20) },
                { QualityTier.Medium,   new(QualityType.Cq, "balanced" ,23) },
                { QualityTier.Low,      new(QualityType.Cq, "speed"    ,28) },
            };

        /// <summary>
        /// libx265 用の品質プリセット（CRF + スピード設定）
        /// </summary>
        public static readonly Dictionary<QualityTier, QualityLevel> Libx265Levels =
            new()
            {
                { QualityTier.VeryHigh, new(QualityType.Crf, "veryslow" ,18) },
                { QualityTier.High,     new(QualityType.Crf, "slow"     ,21) },
                { QualityTier.Medium,   new(QualityType.Crf, "medium"   ,24) },
                { QualityTier.Low,      new(QualityType.Crf, "fast"     ,28) },
            };

        /// <summary>
        /// hevc_nvenc 用の品質プリセット（CQ + NVENC パラメータ）
        /// </summary>
        public static readonly Dictionary<QualityTier, QualityLevel> HevcNvencLevels =
            new()
            {
                { QualityTier.VeryHigh, new(QualityType.Cq, "p7"     ,18) },
                { QualityTier.High,     new(QualityType.Cq, "p6"     ,21) },
                { QualityTier.Medium,   new(QualityType.Cq, "p4"     ,24) },
                { QualityTier.Low,      new(QualityType.Cq, "p2"     ,28) },
            };

        /// <summary>
        /// hevc_qsv 用の品質プリセット（CQ + QSV パラメータ）
        /// </summary>
        public static readonly Dictionary<QualityTier, QualityLevel> HevcQsvLevels =
            new()
            {
                { QualityTier.VeryHigh, new(QualityType.Cq, "slow"   ,18) },
                { QualityTier.High,     new(QualityType.Cq, "medium" ,21) },
                { QualityTier.Medium,   new(QualityType.Cq, "medium" ,24) },
                { QualityTier.Low,      new(QualityType.Cq, "fast"   ,28) },
            };

        /// <summary>
        /// hevc_amf 用の品質プリセット（CQ + AMF パラメータ）
        /// </summary>
        public static readonly Dictionary<QualityTier, QualityLevel> HevcAmfLevels =
            new()
            {
                { QualityTier.VeryHigh, new(QualityType.Cq, "quality"  ,18) },
                { QualityTier.High,     new(QualityType.Cq, "balanced" ,21) },
                { QualityTier.Medium,   new(QualityType.Cq, "balanced" ,24) },
                { QualityTier.Low,      new(QualityType.Cq, "speed"    ,28) },
            };
    }
}
