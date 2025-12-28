using ezFFmpeg.Models.Codec;
using ezFFmpeg.Models.Common;
using ezFFmpeg.Models.Interfaces;
using ezFFmpeg.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Encoder
{
    /// <summary>
    /// 利用可能なオーディオエンコーダの定義と操作をまとめた静的クラス
    /// </summary>
    public static class AudioEncoders
    {
        // --------------------------
        // 主要エンコーダの定義
        // --------------------------
        public static readonly AudioEncoder Copy        = new("copy"        , "変更しない"      , AudioCodecs.Copy);
        public static readonly AudioEncoder Aac         = new("aac"         , "AAC"             , AudioCodecs.Aac);
        public static readonly AudioEncoder Aac_Fdk     = new("libfdk_aac"  , "AAC(Fraunhofer)" , AudioCodecs.Aac);
        public static readonly AudioEncoder Alac        = new("alac"        , "ALAC"            , AudioCodecs.Alac);
        //public static readonly AudioEncoder Pcm_S16le   = new("pcm_s16le"   , "PCM(s16le)"      , AudioCodecs.PCM);
        //public static readonly AudioEncoder Pcm_S16be   = new("pcm_s16be"   , "PCM(s16be)"      , AudioCodecs.PCM);
        //public static readonly AudioEncoder Pcm_S24le   = new("pcm_s24le"   , "PCM(s24le)"      , AudioCodecs.PCM);
        //public static readonly AudioEncoder Pcm_F31le   = new("pcm_f32le"   , "PCM(f32le)"      , AudioCodecs.PCM);
        public static readonly AudioEncoder Mp3         = new("mp3"         , "MP3"             , AudioCodecs.Mp3);
        public static readonly AudioEncoder Mp3_Lame    = new("libmp3lame"  , "MP3(Lame)"       , AudioCodecs.Mp3);
        public static readonly AudioEncoder Flac        = new("flac"        , "FLAC"            , AudioCodecs.Flac);

        /// <summary>
        /// 全てのエンコーダを配列で保持
        /// </summary>
        public static readonly AudioEncoder[] All =
        [
            Copy,
            Aac, Aac_Fdk,
            Alac,
            Mp3, Mp3_Lame,
            Flac
        ];

        /// <summary>
        /// 指定された名前のエンコーダを取得する
        /// </summary>
        /// <param name="encoder">エンコーダ識別名</param>
        /// <returns>対応する AudioEncoder インスタンス</returns>
        /// <exception cref="ArgumentException">未対応のエンコーダ名の場合</exception>
        public static AudioEncoder GetEncoder(string encoder)
        {
            var audioEncoder = Array.Find(All, c => c.Encoder == encoder);
            if (audioEncoder!.Encoder == null) throw new ArgumentException($"未対応の encoder: {encoder}");
            return audioEncoder;
        }

        /// <summary>
        /// 指定されたエンコーダ名から対応する AudioCodec を取得する
        /// </summary>
        /// <param name="encoder">エンコーダ識別名</param>
        /// <returns>対応する AudioCodec インスタンス</returns>
        /// <exception cref="ArgumentException">未対応のエンコーダ名の場合</exception>
        public static ICodec GetCodec(string encoder)
        {
            var audioEncoder = Array.Find(All, c => c.Encoder == encoder);
            if (audioEncoder!.Encoder == null) throw new ArgumentException($"未対応の encoder: {encoder}");
            return audioEncoder.Codec;
        }

        /// <summary>
        /// FFmpegサービスを使って、各エンコーダが利用可能かを判定し、CanUse プロパティを更新する
        /// </summary>
        /// <param name="service">IFFmpegService インスタンス</param>
        public static void Initialize(IFFmpegService service)
        {
            foreach (var encoder in All)
            {
                if (encoder.IsCopy)
                {
                    // コピーは常に使用可能
                    encoder.CanUse = true;
                }
                else
                {
                    // FFmpeg が使用可能かをチェック
                    encoder.CanUse = service.CanUseEncoder(encoder.Encoder);
                }
            }
        }
    }
}
