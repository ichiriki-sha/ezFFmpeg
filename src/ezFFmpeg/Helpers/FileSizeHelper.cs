using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Helpers
{
    /// <summary>
    /// ファイルサイズを人間が読みやすい文字列に変換するヘルパークラス
    /// </summary>
    public static class FileSizeHelper
    {
        /// <summary>
        /// バイト単位のサイズを「KB」「MB」「GB」などに変換して返す
        /// </summary>
        /// <param name="size">ファイルサイズ（バイト）</param>
        /// <returns>読みやすいサイズ表記（例: "1.23 MB"）</returns>
        public static string GetReadableFileSize(long size)
        {
            // サイズ単位の配列
            string[] sizes = ["B", "KB", "MB", "GB", "TB"];

            double len = size; // double に変換して小数計算用
            int order = 0;     // 単位インデックス

            // 1024 で割って適切な単位を求める
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // 小数点以下2桁まで表示して単位を付加
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
