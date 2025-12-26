using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ezFFmpeg.Helpers
{
    /// <summary>
    /// ファイル拡張子からファイルの種類（表示用の説明文字列）を取得するヘルパークラス
    /// </summary>
    public static class FileTypeHelper
    {
        // ----------------------------
        // WinAPI: SHGetFileInfo を呼び出すための宣言
        // ----------------------------
        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(
            string pszPath,            // ファイルパスまたは拡張子
            uint dwFileAttributes,     // ファイル属性
            out SHFILEINFO psfi,       // SHFILEINFO 構造体への出力
            uint cbFileInfo,           // SHFILEINFO のサイズ
            uint uFlags                // 情報取得フラグ
        );

        // ----------------------------
        // SHGetFileInfo 用のフラグ定義
        // ----------------------------
        private const uint SHGFI_TYPENAME = 0x000000400;          // ファイルの種類名を取得
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010; // ファイルが存在しなくても属性を使用して取得

        /// <summary>
        /// SHGetFileInfo で使用する構造体
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;       // アイコンハンドル（今回は使用しない）
            public int iIcon;          // アイコンインデックス（今回は使用しない）
            public uint dwAttributes;  // ファイル属性（今回は使用しない）

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName; // 表示名（今回は使用しない）

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;    // ファイルの種類名
        }

        /// <summary>
        /// 指定した拡張子のファイル種類名を取得する
        /// </summary>
        /// <param name="extension">拡張子（例: ".mp4" または "mp4"）</param>
        /// <returns>拡張子に対応するファイル種類名（例: "MP4 ビデオ"）</returns>
        public static string GetFileTypeDescription(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
                return "";

            // ドットがなければ付与
            if (!extension.StartsWith('.'))
                extension = "." + extension;

            SHFILEINFO shinfo;

            // SHGetFileInfo を呼び出して種類名を取得
            SHGetFileInfo(extension,
                          0x80, // FILE_ATTRIBUTE_NORMAL
                          out shinfo,
                          (uint)Marshal.SizeOf(typeof(SHFILEINFO)),
                          SHGFI_TYPENAME | SHGFI_USEFILEATTRIBUTES);

            return shinfo.szTypeName;
        }
    }
}
