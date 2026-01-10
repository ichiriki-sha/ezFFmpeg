using ezFFmpeg.Models.Codec;
using ezFFmpeg.Models.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Output
{
    /// <summary>
    /// 出力コンテナ形式（mp4 / mkv など）を表すクラス。
    /// 
    /// ・拡張子  
    /// ・対応メディア種別  
    /// ・推奨コーデック  
    /// ・使用可能なコーデック一覧  
    /// 
    /// をまとめて定義する。
    /// </summary>
    public sealed class OutputFormat
    {
        /// <summary>
        /// ファイル拡張子（例: "mp4", "mkv"）。
        /// </summary>
        public string Extension { get; }

        /// <summary>
        /// UI 表示用のフォーマット名。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 出力対象のメディア種別（Video / Audio など）。
        /// </summary>
        public MediaType MediaType { get; }

        /// <summary>
        /// 推奨されるビデオコーデック。
        /// null の場合、ビデオ非対応または制限なし。
        /// </summary>
        public VideoCodec? RecommendedVideo { get; }

        /// <summary>
        /// 推奨されるオーディオコーデック。
        /// null の場合、オーディオ非対応または制限なし。
        /// </summary>
        public AudioCodec? RecommendedAudio { get; }

        /// <summary>
        /// この出力形式で使用可能なビデオコーデック一覧。
        /// </summary>
        public IReadOnlyCollection<VideoCodec> SupportedVideoCodecs { get; }

        /// <summary>
        /// この出力形式で使用可能なオーディオコーデック一覧。
        /// </summary>
        public IReadOnlyCollection<AudioCodec> SupportedAudioCodecs { get; }

        /// <summary>
        /// 出力フォーマットを初期化する。
        /// </summary>
        /// <param name="extension">
        /// ファイル拡張子（ドットなし）。
        /// </param>
        /// <param name="name">
        /// UI 表示用フォーマット名。
        /// </param>
        /// <param name="mediaType">
        /// 出力メディア種別。
        /// </param>
        /// <param name="recommendedVideo">
        /// 推奨ビデオコーデック（存在しない場合は null）。
        /// </param>
        /// <param name="recommendedAudio">
        /// 推奨オーディオコーデック（存在しない場合は null）。
        /// </param>
        /// <param name="supportedVideoCodecs">
        /// 使用可能なビデオコーデック一覧。
        /// </param>
        /// <param name="supportedAudioCodecs">
        /// 使用可能なオーディオコーデック一覧。
        /// </param>
        public OutputFormat(
            string extension,
            string name,
            MediaType mediaType,
            VideoCodec? recommendedVideo,
            AudioCodec? recommendedAudio,            
            IEnumerable<VideoCodec> supportedVideoCodecs,
            IEnumerable<AudioCodec> supportedAudioCodecs)
        {
            Extension = extension;
            Name = name;
            MediaType = mediaType;
            RecommendedVideo = recommendedVideo;
            RecommendedAudio = recommendedAudio;
            SupportedVideoCodecs = supportedVideoCodecs.ToList();
            SupportedAudioCodecs = supportedAudioCodecs.ToList();
        }

        /// <summary>
        /// 指定されたビデオコーデックが
        /// この出力形式で使用可能かどうかを判定する。
        /// </summary>
        public bool IsVideoCodecSupported(VideoCodec codec)
                    => SupportedVideoCodecs.Contains(codec);

        /// <summary>
        /// 指定されたオーディオコーデックが
        /// この出力形式で使用可能かどうかを判定する。
        /// </summary>
        public bool IsAudioCodecSupported(AudioCodec codec)
                    => SupportedAudioCodecs.Contains(codec);

    }
}
