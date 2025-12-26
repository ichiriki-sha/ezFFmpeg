using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ezFFmpeg.Services.FFmpeg
{
    /// <summary>
    /// FFmpeg 実行ファイル (ffmpeg.exe, ffprobe.exe) のフォルダパスを検出・確認するユーティリティクラス。
    /// </summary>
    public static class FFmpegPathService
    {
        /// <summary>
        /// 指定したフォルダが FFmpeg フォルダかどうかを判定します。
        /// 判定基準は ffmpeg.exe と ffprobe.exe が両方存在することです。
        /// </summary>
        /// <param name="path">判定するフォルダパス</param>
        /// <returns>FFmpeg フォルダなら true、そうでなければ false</returns>
        public static bool IsFFmpegFolder(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            if (!Directory.Exists(path))
                return false;

            // ffmpeg.exe と ffprobe.exe があるかを確認
            string[] exeNames = [FFmpegBinaries.FFmpeg, FFmpegBinaries.FFprobe];

            foreach (string exe in exeNames)
            {
                string fullPath = Path.Combine(path, exe);
                if (!File.Exists(fullPath))
                {
                    return false; // どちらか一方でも無ければ FFmpeg フォルダではない
                }
            }

            return true; // 両方存在 → FFmpeg フォルダである
        }

        /// <summary>
        /// 環境変数 PATH から FFmpeg のインストールフォルダを探して返します。
        /// 見つからない場合は null を返します。
        /// </summary>
        /// <returns>FFmpeg フォルダのパス、または null</returns>
        public static string? GetFFmpegFolderPath()
        {
            // PATH を取得
            string? pathEnv = Environment.GetEnvironmentVariable("PATH");
            if (string.IsNullOrEmpty(pathEnv))
                return null;

            // PATH は ; 区切り
            string[] paths = pathEnv.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (string dir in paths)
            {
                try
                {
                    string fullDir = dir.Trim();

                    // ディレクトリが存在しない場合は無視
                    if (!Directory.Exists(fullDir))
                        continue;

                    if (IsFFmpegFolder(fullDir))
                        return fullDir; // FFmpeg のフォルダを返す
                }
                catch
                {
                    // アクセスできないパスなどは無視
                }
            }

            return null; // 見つからない
        }
    }
}
