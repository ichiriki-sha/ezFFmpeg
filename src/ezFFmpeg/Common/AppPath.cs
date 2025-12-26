using ezFFmpeg.Services.FFmpeg;
using System;
using System.IO;

namespace ezFFmpeg.Common
{
    /// <summary>
    /// アプリケーション内で使用する各種パスを一元管理するユーティリティクラス。
    /// 設定フォルダ、作業フォルダ、FFmpeg関連パス、テンポラリファイルパスなどを提供する。
    /// </summary>
    public static class AppPath
    {
        /// <summary>
        /// アプリケーション設定フォルダのパスを取得する。
        /// %APPDATA%\<アプリ名> を指す。
        /// </summary>
        /// <returns>アプリケーション設定フォルダの絶対パス</returns>
        public static string GetAppSettingFolderPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AppInfo.AppName);
        }

        /// <summary>
        /// アプリケーション専用の作業フォルダのパスを取得する。
        /// 設定フォルダ配下の「work」フォルダを指す。
        /// </summary>
        /// <returns>作業フォルダの絶対パス</returns>
        public static string GetAppWorkFolderPath()
        {
            return Path.Combine(GetAppSettingFolderPath(), "work");
        }

        /// <summary>
        /// アプリケーション設定ファイル（settings.json）のパスを取得する。
        /// </summary>
        /// <returns>settings.json の絶対パス</returns>
        public static string GetAppSettingPath()
        {
            return Path.Combine(GetAppSettingFolderPath(), "settings.json");
        }

        /// <summary>
        /// 指定したフォルダ内にある FFmpeg 実行ファイルのパスを取得する。
        /// </summary>
        /// <param name="folder">FFmpeg バイナリが配置されているフォルダ</param>
        /// <returns>ffmpeg 実行ファイルの絶対パス</returns>
        public static string GetFFmpegPath(string folder)
        {
            return Path.Combine(folder, FFmpegBinaries.FFmpeg);
        }

        /// <summary>
        /// 指定したフォルダ内にある FFprobe 実行ファイルのパスを取得する。
        /// </summary>
        /// <param name="folder">FFprobe バイナリが配置されているフォルダ</param>
        /// <returns>ffprobe 実行ファイルの絶対パス</returns>
        public static string GetFFprobePath(string folder)
        {
            return Path.Combine(folder, FFmpegBinaries.FFprobe);
        }

        /// <summary>
        /// アプリケーション作業フォルダ配下に、一意なテンポラリファイルパスを生成する。
        /// ファイル名には GUID が使用される。
        /// </summary>
        /// <param name="extension">拡張子（例: ".png", ".wav"）</param>
        /// <returns>テンポラリファイルの絶対パス</returns>
        public static string GetTempFilePath(string extension)
        {
            var guid = Guid.NewGuid();
            return Path.Combine(GetAppWorkFolderPath(), guid.ToString() + extension);
        }
    }
}
