using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Services.FFmpeg
{
    /// <summary>
    /// FFmpeg 関連の実行可能ファイル名を定義する定数クラス。
    /// 他の場所で直接文字列を使用せず、このクラスを参照することで安全にファイル名を扱える。
    /// </summary>
    public static class FFmpegBinaries
    {
        /// <summary>
        /// FFmpeg 実行ファイル名
        /// </summary>
        public const string FFmpeg = "ffmpeg.exe";

        /// <summary>
        /// FFprobe 実行ファイル名
        /// </summary>
        public const string FFprobe = "ffprobe.exe";
    }
}
