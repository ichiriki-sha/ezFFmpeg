using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Quality
{
    /// <summary>
    /// エンコード品質レベルを表すクラス。
    /// 性能（速度）と品質のバランスを保持するための情報を格納する。
    /// </summary>
    public sealed class QualityLevel
    {
        /// <summary>
        /// 品質レベルの種類
        /// </summary>
        public QualityType QualityType { get; }

        /// <summary>
        /// エンコード時の性能値や速度パラメータ（例: "fast", "medium", "slow"）
        /// </summary>
        public string PerformanceValue { get; }

        /// <summary>
        /// 品質の数値値（一般的に高いほど高品質）
        /// </summary>
        public int QualityValue { get; }

        /// <summary>
        /// QualityLevel の新しいインスタンスを初期化する。
        /// </summary>
        /// <param name="qualityType">品質タイプ</param>
        /// <param name="performanceValue">エンコード性能パラメータ</param>
        /// <param name="qualityValue">品質の数値値</param>
        public QualityLevel(QualityType qualityType, string performanceValue, int qualityValue)
        {
            QualityType = qualityType;
            PerformanceValue = performanceValue;
            QualityValue = qualityValue;
        }
    }
}
