using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Output
{
    /// <summary>
    /// 出力ファイル情報のタグ定義をまとめた静的クラス。
    /// 各タグは (Tag, Name) のタプルで定義され、タグ名と表示用名称を保持する。
    /// </summary>
    public static class OutputFileTags
    {
        /// <summary>
        /// ファイル名を表すタグ
        /// </summary>
        public static readonly (string Tag, string Name) FileName = ("filename", "ファイル名");

        /// <summary>
        /// ビデオコーデックを表すタグ
        /// </summary>
        public static readonly (string Tag, string Name) VideoCodec = ("vcodec", "ビデオコーデック");

        /// <summary>
        /// オーディオコーデックを表すタグ
        /// </summary>
        public static readonly (string Tag, string Name) AudioCodec = ("acodec", "オーディオコーデック");

        /// <summary>
        /// ビデオ解像度を表すタグ
        /// </summary>
        public static readonly (string Tag, string Name) VideoResolution = ("resolution", "解像度");

        /// <summary>
        /// ビデオ品質レベルを表すタグ
        /// </summary>
        public static readonly (string Tag, string Name) VideoQuality = ("quality", "品質レベル");

        /// <summary>
        /// 出力日時などのタイムスタンプを表すタグ
        /// </summary>
        public static readonly (string Tag, string Name) TimeStamp = ("timestamp", "タイムスタンプ");

        /// <summary>
        /// 出力フォーマットの拡張子を表すタグ
        /// </summary>
        public static readonly (string Tag, string Name) Extension = ("extension", "出力形式の拡張子");

        /// <summary>
        /// すべてのタグをまとめた配列。
        /// 表示順や一括処理に利用可能。
        /// </summary>
        public static readonly (string Tag, string Name)[] All =
        [
            FileName, VideoCodec, AudioCodec, VideoResolution, TimeStamp, Extension
        ];
    }
}
