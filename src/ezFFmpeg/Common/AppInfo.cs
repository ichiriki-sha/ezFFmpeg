using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ezFFmpeg.Common
{

    /// <summary>
    /// アプリケーション自体に関する基本情報を提供するユーティリティクラス。
    /// 
    /// ・アプリ名
    /// ・表示名（Assembly 情報）
    /// ・バージョン
    /// ・実行ファイルのパス
    /// ・AppData 配下の保存先
    /// ・ビルド種別（Debug / Release）
    /// 
    /// など、アプリ全体で共通して使用される情報を集約する。
    /// </summary>
    public static class AppInfo
    {
        /// <summary>
        /// 現在実行中のプロセス名を取得する。
        /// 通常は実行ファイル名（拡張子なし）が返る。
        /// </summary>
        public static string AppName =>
            Process.GetCurrentProcess().ProcessName;

        /// <summary>
        /// アプリケーションの表示名を取得する。
        /// AssemblyProductAttribute が設定されている場合はその値を使用し、
        /// 設定されていない場合は AppName を代替として返す。
        /// </summary>
        public static string AppDisplayName =>
            Assembly.GetEntryAssembly()?
                .GetCustomAttribute<AssemblyProductAttribute>()?
                .Product
            ?? AppName;

        /// <summary>
        /// アプリケーションのバージョン情報を取得する。
        /// Assembly の Version が取得できない場合は 1.0.0 を返す。
        /// </summary>
        public static Version AppVersion =>
            Assembly.GetEntryAssembly()?.GetName().Version
            ?? new Version(1, 0, 0);

        /// <summary>
        /// 実行中アプリケーションの実行ファイル（exe）のフルパスを取得する。
        /// .NET 6 以降では Environment.ProcessPath を優先して使用する。
        /// </summary>
        public static string ExecutablePath =>
            Environment.ProcessPath
            ?? Process.GetCurrentProcess().MainModule!.FileName;

        /// <summary>
        /// 実行ファイルが配置されているディレクトリのパスを取得する。
        /// </summary>
        public static string ExecutableDirectory =>
            Path.GetDirectoryName(ExecutablePath)!;

        /// <summary>
        /// 現在のビルドが Debug ビルドかどうかを示す。
        /// </summary>
        public static bool IsDebugBuild
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }
    }
}