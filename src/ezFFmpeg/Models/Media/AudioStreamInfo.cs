using ezFFmpeg.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Media
{
    /// <summary>
    /// オーディオストリームの情報を保持するクラス。
    /// FFmpeg などから取得した音声トラックの詳細情報を格納する。
    /// </summary>
    public class AudioStreamInfo
    {
        /// <summary>
        /// 使用されているオーディオコーデックの名称。
        /// 例: "aac", "mp3" など。
        /// </summary>
        public string? Codec { get; set; }

        /// <summary>
        /// サンプルレート（Hz）。
        /// </summary>
        public int SampleRate { get; set; }

        /// <summary>
        /// ビットレート（bps）。
        /// </summary>
        public long BitRate { get; set; }

        /// <summary>
        /// チャンネル数（モノラル=1、ステレオ=2 など）。
        /// </summary>
        public int Channels { get; set; }

        /// <summary>
        /// サンプルレートを文字列として取得。
        /// 例: 44100 → "44.1 kHz"
        /// </summary>
        public string SampleRateString => SampleRate.ToHzString();

        /// <summary>
        /// ビットレートを文字列として取得。
        /// 例: 128000 → "128 kbps"
        /// </summary>
        public string BitRateString => BitRate.ToBitRateString();

        /// <summary>
        /// チャンネル数を文字列として取得。
        /// 例: 2 → "ステレオ"
        /// </summary>
        public string ChannelsString => Channels.ToChannelsString();
    }
}
