using ezFFmpeg.Common;
using ezFFmpeg.Models.Codec;
using ezFFmpeg.Models.Common;
using ezFFmpeg.Models.Interfaces;
using ezFFmpeg.Models.Media;
using ezFFmpeg.Models.Quality;
using ezFFmpeg.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ezFFmpeg.Models.Encoder
{
    /// <summary>
    /// ビデオエンコーダの静的管理クラス
    /// 使用可能なCPU/GPUエンコーダを定義し、取得・初期化を提供する
    /// </summary>
    public static class VideoEncoders
    {
        // -----------------------------
        // 各エンコーダの定義
        // -----------------------------

        /// <summary>コピー（変換しない）</summary>
        public static readonly VideoEncoder Copy =
            new("copy", "変更しない", VideoCodecs.Copy, EncoderType.Cpu, QualityLevels.NoneLevels);

        // H.264 CPU/GPU エンコーダ
        public static readonly VideoEncoder LibX264 =
            new("libx264", "H.264", VideoCodecs.H264, EncoderType.Cpu, QualityLevels.Libx264Levels);
        public static readonly VideoEncoder H264_Nvenc =
            new("h264_nvenc", "H.264(nvenc)", VideoCodecs.H264, EncoderType.Gpu, QualityLevels.H264NvencLevels);
        public static readonly VideoEncoder H264_Qsv =
            new("h264_qsv", "H.264(qsv)", VideoCodecs.H264, EncoderType.Gpu, QualityLevels.H264QsvLevels);
        public static readonly VideoEncoder H264_Amf =
            new("h264_amf", "H.264(amf)", VideoCodecs.H264, EncoderType.Gpu, QualityLevels.H264AmfLevels);

        // H.265 / HEVC CPU/GPU エンコーダ
        public static readonly VideoEncoder LibX265 =
            new("libx265", "H.265", VideoCodecs.Hevc , EncoderType.Cpu, QualityLevels.Libx265Levels);
        public static readonly VideoEncoder H265_Nvenc =
            new("hevc_nvenc", "H.265(nvenc)", VideoCodecs.Hevc, EncoderType.Gpu, QualityLevels.HevcNvencLevels);
        public static readonly VideoEncoder H265_Qsv =
            new("hevc_qsv", "H.265(qsv)", VideoCodecs.Hevc, EncoderType.Gpu, QualityLevels.HevcQsvLevels);
        public static readonly VideoEncoder H265_Amf =
            new("hevc_amf", "H.264(amf)", VideoCodecs.Hevc, EncoderType.Gpu, QualityLevels.HevcAmfLevels);

        /// <summary>すべてのエンコーダ一覧</summary>
        public static readonly VideoEncoder[] All =
        [
            Copy, LibX264, LibX265, H264_Nvenc, H265_Nvenc, H264_Qsv, H265_Qsv, H264_Amf, H265_Amf
        ];

        // -----------------------------
        // エンコーダ取得
        // -----------------------------

        /// <summary>
        /// 指定のエンコーダ名に対応する VideoEncoder を取得
        /// </summary>
        /// <param name="encoder">エンコーダ識別名</param>
        /// <returns>VideoEncoder</returns>
        public static VideoEncoder GetEncoder(string encoder)
        {
            var videoEncoder = Array.Find(All, c => c.Encoder == encoder);
            if (videoEncoder!.Encoder == null) throw new ArgumentException($"未対応の encoder: {encoder}");
            return videoEncoder;
        }

        /// <summary>
        /// 指定のエンコーダ名から VideoCodec を取得
        /// </summary>
        /// <param name="encoder">エンコーダ識別名</param>
        /// <returns>VideoCodec</returns>
        public static ICodec GetCodec(string encoder)
        {
            var videoEncoder = Array.Find(All, c => c.Encoder == encoder);
            if (videoEncoder!.Encoder == null) throw new ArgumentException($"未対応の encoder: {encoder}");
            return videoEncoder.Codec;
        }

        // -----------------------------
        // GPU エンコーダ関連
        // -----------------------------

        /// <summary>
        /// 利用可能な GPU エンコーダのリストを取得
        /// </summary>
        public static List<VideoEncoder> GetAvailableGpuEncoders()
        {
            List<VideoEncoder> ret = [];
            foreach (VideoEncoder encoder in All)
            {
                if (encoder.Type == EncoderType.Gpu && encoder.CanUse)
                {
                    ret.Add(encoder);
                }
            }
            return ret;
        }

        /// <summary>
        /// GPU エンコーダが利用可能か
        /// </summary>
        public static bool CanGpuEncoder()
        {
            return GetAvailableGpuEncoders().Count > 0;
        }

        /// <summary>
        /// 利用可能な GPU エンコーダのうちデフォルトのエンコーダ名を取得
        /// </summary>
        public static string GetDefaultGpuEncoderName()
        {
            return GetAvailableGpuEncoders().First().Encoder;
        }

        /// <summary>
        /// 利用可能な GPU エンコーダのうちデフォルトのエンコーダ名を取得
        /// </summary>
        public static VideoEncoder SelectDefaultEncoder(IEnumerable<SelectionItem> list, string codec, bool useGpu)
        {

            foreach (var item in list)
            {

                var encoder = GetEncoder(item.Key.ToString()!);

                if (useGpu)
                {
                    if (encoder.Codec.Codec  == codec && encoder.CanUse && encoder.UseGpu )
                    {
                        return encoder;
                    }
                }
                else
                {
                    if (encoder.Codec.Codec == codec && encoder.CanUse && !encoder.UseGpu)
                    {
                        return encoder;
                    }
                }
            }

            return VideoEncoders.Copy;
        }


        // -----------------------------
        // 初期化
        // -----------------------------

        /// <summary>
        /// エンコーダの使用可否を初期化（非同期）
        /// </summary>
        private static async Task InitializeAsync(IFFmpegService service)
        {
            List<VideoEncoder> availableList = [];

            // 簡易判定（CPU/GPU）
            foreach (var encoder in All)
            {
                if (encoder.IsCopy)
                {
                    encoder.CanUse = true;
                }
                else
                {
                    if (!service.CanUseEncoder(encoder.Encoder))
                    {
                        encoder.CanUse = false;
                    }
                    else
                    {
                        if (encoder.Type == EncoderType.Cpu)
                            encoder.CanUse = true;
                        else
                        {
                            encoder.CanUse = false; // とりあえずfalse
                            availableList.Add(encoder);
                        }
                    }
                }
            }

            // GPU エンコーダの非同期チェック
            var semaphore = new SemaphoreSlim(InternalParallel.Default);
            var tasks = availableList.Select(async encoder =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var isAvailable = service.IsEncoderAvailable(encoder.Encoder); // 同期呼び出し
                    return (encoder, isAvailable);
                }
                finally
                {
                    semaphore.Release();
                }
            }).ToList();

            var results = await Task.WhenAll(tasks);

            foreach (var (encoder, isAvailable) in results)
            {
                encoder.CanUse = isAvailable;
            }
        }

        /// <summary>
        /// エンコーダの使用可否を初期化（同期）
        /// </summary>
        public static void Initialize(IFFmpegService service)
        {
            // 同期的に完全に初期化を待つ
            InitializeAsync(service).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
