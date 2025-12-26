using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Services.FFmpeg.EncoderCapabilityProvider
{
    /// <summary>
    /// FFmpeg エンコーダー名から GPU 対応かどうかを判定するユーティリティクラス。
    /// </summary>
    internal static class EncoderGpuDetector
    {
        /// <summary>
        /// 指定されたエンコーダーが GPU エンコーディングに対応しているかどうかを判定します。
        /// 現在サポートしている GPU エンコーダーは以下です:
        /// - NVENC (NVIDIA)
        /// - QSV   (Intel Quick Sync Video)
        /// - AMF   (AMD)
        /// </summary>
        /// <param name="encoder">FFmpeg エンコーダー名</param>
        /// <returns>GPU 対応なら true、CPU 専用なら false</returns>
        public static bool IsGpu(string encoder)
        {
            encoder = encoder.ToLowerInvariant();

            return encoder.EndsWith("_nvenc")
                || encoder.EndsWith("_qsv")
                || encoder.EndsWith("_amf");
        }
    }
}
