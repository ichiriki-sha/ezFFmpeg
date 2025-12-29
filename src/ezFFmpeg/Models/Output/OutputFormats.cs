using ezFFmpeg.Common;
using ezFFmpeg.Models.Codec;
using ezFFmpeg.Models.Encoder;
using ezFFmpeg.Models.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ezFFmpeg.Models.Output
{
    /// <summary>
    /// 出力可能なメディアフォーマットを定義する静的クラス。
    /// 各フォーマットは拡張子、表示名、メディアタイプのタプルで管理される。
    /// </summary>
    public static class OutputFormats
    {

        /// <summary>
        /// MP4 動画フォーマット
        /// </summary>
        public static readonly OutputFormat Mp4 =
                        new(
                            ".mp4",
                            "MP4 ファイル",
                            MediaType.Video,
                            VideoCodecs.H264,
                            AudioCodecs.Copy,
                            [VideoCodecs.H264, VideoCodecs.Hevc, VideoCodecs.Copy] ,
                            [AudioCodecs.Aac, AudioCodecs.Mp3, AudioCodecs.Flac, AudioCodecs.Copy]
                        );

        /// <summary>
        /// MOV 動画フォーマット
        /// </summary>
        public static readonly OutputFormat Mov =
                        new(
                            ".mov",
                            "MOV ファイル",
                            MediaType.Video,
                            VideoCodecs.H264,
                            AudioCodecs.Copy,
                            [VideoCodecs.H264, VideoCodecs.Hevc, VideoCodecs.Copy],
                            [AudioCodecs.Aac, AudioCodecs.Alac, AudioCodecs.Mp3, AudioCodecs.Flac, AudioCodecs.Copy]
                        );

        /// <summary>
        /// AAC オーディオフォーマット
        /// </summary>
        public static readonly OutputFormat M4a =
                        new(
                            ".m4a",
                            "M4A ファイル",
                            MediaType.Audio,
                            VideoCodecs.Copy, // 表示用
                            AudioCodecs.Aac,
                            [VideoCodecs.Copy], // 表示用
                            [AudioCodecs.Aac, AudioCodecs.Copy]
                        );

        /// <summary>
        /// MP3 オーディオフォーマット
        /// </summary>
        public static readonly OutputFormat Mp3 =
                        new(
                            ".mp3",
                            "MP3 ファイル",
                            MediaType.Audio,
                            VideoCodecs.Copy, // 表示用
                            AudioCodecs.Mp3,
                            [VideoCodecs.Copy], // 表示用
                            [AudioCodecs.Mp3, AudioCodecs.Copy]
                        );

        /// <summary>
        /// FLAC オーディオフォーマット
        /// </summary>
        public static readonly OutputFormat Flac =
                        new(
                            ".flac",
                            "FLAC ファイル",
                            MediaType.Audio,
                            VideoCodecs.Copy, // 表示用
                            AudioCodecs.Flac,
                            [VideoCodecs.Copy], // 表示用
                            [AudioCodecs.Flac, AudioCodecs.Copy]
                        );

        public static readonly OutputFormat[] All =
        [
            Mp4,Mov,M4a,Mp3,Flac
        ];

        /// <summary>
        /// 指定された拡張子に対応するフォーマットの表示名を取得する。
        /// </summary>
        /// <param name="extension">拡張子（例: ".mp4"）</param>
        /// <returns>表示名（例: "MP4 ファイル"）</returns>
        /// <exception cref="ArgumentException">未対応の拡張子の場合にスローされる</exception>
        public static OutputFormat GetOutputFormat(string extension)
        {
            var format = Array.Find(All, c => c.Extension == extension);
            if (format == null) throw new ArgumentException($"未対応の extension: {extension}");
            return format;
        }

        /// <summary>
        /// 指定された拡張子に対応するフォーマットの表示名を取得する。
        /// </summary>
        /// <param name="extension">拡張子（例: ".mp4"）</param>
        /// <returns>表示名（例: "MP4 ファイル"）</returns>
        /// <exception cref="ArgumentException">未対応の拡張子の場合にスローされる</exception>
        public static string GetName(string extension)
        {
            var format = Array.Find(All, c => c.Extension == extension);
            if (format == null) throw new ArgumentException($"未対応の extension: {extension}");
            return format.Name;
        }

        /// <summary>
        /// 指定された拡張子に対応するメディアタイプを取得する。
        /// </summary>
        /// <param name="extension">拡張子（例: ".mp4"）</param>
        /// <returns>MediaType（Video/Audio など）</returns>
        /// <exception cref="ArgumentException">未対応の拡張子の場合にスローされる</exception>
        public static MediaType GetMediaType(string extension)
        {
            var format = Array.Find(All, c => c.Extension == extension);
            if (format == null) throw new ArgumentException($"未対応の extension: {extension}");
            return format.MediaType;
        }
    }
}
