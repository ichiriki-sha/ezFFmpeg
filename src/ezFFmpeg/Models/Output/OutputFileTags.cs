using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Output
{
    /// <summary>
    /// 出力ファイル情報のタグ定義をまとめた静的クラス。
    /// 各タグは (Tag, Name) のタプルで定義され、タグ名と表示用名称を保持する。
    /// </summary>
    internal static class OutputFileTags
    {
        /// <summary>
        /// ファイル名を表すタグ
        /// </summary>
        internal static readonly (string Tag, string Name) FileName = ("filename", "ファイル名");

        /// <summary>
        /// ビデオコーデックを表すタグ
        /// </summary>
        internal static readonly (string Tag, string Name) VideoCodec = ("vcodec", "ビデオコーデック");

        /// <summary>
        /// オーディオコーデックを表すタグ
        /// </summary>
        internal static readonly (string Tag, string Name) AudioCodec = ("acodec", "オーディオコーデック");

        /// <summary>
        /// ビデオ品質レベルを表すタグ
        /// </summary>
        internal static readonly (string Tag, string Name) VideoQuality = ("quality", "品質レベル");

        /// <summary>
        /// ビデオ解像度を表すタグ
        /// </summary>
        internal static readonly (string Tag, string Name) VideoResolution = ("resolution", "解像度");

        /// <summary>
        /// 出力日時などのタイムスタンプを表すタグ
        /// </summary>
        internal static readonly (string Tag, string Name) TimeStamp = ("timestamp", "タイムスタンプ");

        /// <summary>
        /// すべてのタグをまとめた配列。
        /// 表示順や一括処理に利用可能。
        /// </summary>
        internal static readonly (string Tag, string Name)[] All =
        [
            FileName, VideoCodec, AudioCodec, VideoQuality, VideoResolution, TimeStamp
        ];
    }
}
