using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ezFFmpeg.Helpers
{
    /// <summary>
    /// ファイルやフォルダに関連付けられた Windows 標準アイコンを取得するヘルパークラス。
    /// 
    /// Explorer と同じアイコンを WPF の Image に表示したい場合に使用する。
    /// 内部では Win32 API（SHGetFileInfo）を利用している。
    /// </summary>
    public static class FileIconHelper
    {
        // ------------------------
        // SHGetFileInfo フラグ定義
        // ------------------------

        /// <summary>
        /// アイコンハンドルを取得する
        /// </summary>
        private const uint SHGFI_ICON = 0x000000100;

        /// <summary>
        /// 小さいサイズのアイコンを取得する
        /// </summary>
        private const uint SHGFI_SMALLICON = 0x000000001;

        /// <summary>
        /// 実ファイルが存在しなくても拡張子・属性から取得する
        /// </summary>
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;

        // ------------------------
        // ファイル属性定義
        // ------------------------

        /// <summary>
        /// 通常ファイル
        /// </summary>
        private const uint FILE_ATTRIBUTE_FILE = 0x00000100;

        /// <summary>
        /// フォルダ
        /// </summary>
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

        // ------------------------
        // Win32 構造体
        // ------------------------

        /// <summary>
        /// SHGetFileInfo で使用される情報格納用構造体
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            /// <summary>
            /// 取得したアイコンのハンドル
            /// </summary>
            public IntPtr hIcon;

            /// <summary>
            /// システムイメージリスト内のアイコンインデックス
            /// </summary>
            public int iIcon;

            /// <summary>
            /// ファイル属性
            /// </summary>
            public uint dwAttributes;

            /// <summary>
            /// 表示名（未使用）
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            /// <summary>
            /// 種類名（例: "テキスト ドキュメント"）
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        // ------------------------
        // Win32 API
        // ------------------------

        /// <summary>
        /// ファイル情報（アイコン等）を取得する Win32 API
        /// </summary>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbFileInfo,
            uint uFlags
        );

        /// <summary>
        /// アイコンハンドルを解放する Win32 API
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        // ------------------------
        // 公開メソッド
        // ------------------------

        /// <summary>
        /// 指定されたパスに関連付けられたアイコンを取得する。
        /// 
        /// 実ファイルが存在しなくても、拡張子やフォルダ属性から
        /// Explorer と同じアイコンを取得できる。
        /// </summary>
        /// <param name="path">
        /// ファイルパスまたはフォルダ名（拡張子判定用）
        /// </param>
        /// <param name="isFolder">
        /// フォルダとして扱うかどうか
        /// </param>
        /// <returns>
        /// WPF で使用可能な BitmapSource
        /// </returns>
        public static BitmapSource GetIcon(string path, bool isFolder)
        {
            SHFILEINFO shinfo = new();

            // ファイルかフォルダかで属性を切り替える
            uint attr = isFolder
                ? FILE_ATTRIBUTE_DIRECTORY
                : FILE_ATTRIBUTE_FILE;

            // Windows にアイコン情報を問い合わせ
            SHGetFileInfo(
                path,
                attr,
                ref shinfo,
                (uint)Marshal.SizeOf(shinfo),
                SHGFI_ICON | SHGFI_USEFILEATTRIBUTES);

            // HICON → WPF BitmapSource に変換
            BitmapSource bitmap = Imaging.CreateBitmapSourceFromHIcon(
                shinfo.hIcon,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(32, 32)
            );

            // 重要：Win32 のアイコンハンドルは必ず解放する
            DestroyIcon(shinfo.hIcon);

            return bitmap;
        }
    }
}
