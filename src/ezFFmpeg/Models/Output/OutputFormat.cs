using ezFFmpeg.Models.Codec;
using ezFFmpeg.Models.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Output
{
    public sealed class OutputFormat
    {
        public string Extension { get; }
        public string Name { get; }
        public MediaType MediaType { get; }

        public VideoCodec? RecommendedVideo { get; }
        public AudioCodec? RecommendedAudio { get; }

        public IReadOnlyCollection<VideoCodec> SupportedVideoCodecs { get; }
        public IReadOnlyCollection<AudioCodec> SupportedAudioCodecs { get; }

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

        public bool IsVideoCodecSupported(VideoCodec codec)
                    => SupportedVideoCodecs.Contains(codec);

        public bool IsAudioCodecSupported(AudioCodec codec)
                    => SupportedAudioCodecs.Contains(codec);

    }
}
