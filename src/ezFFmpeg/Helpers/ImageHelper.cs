using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ezFFmpeg.Helpers
{
    /// <summary>
    /// 画像関連の便利関数を提供するヘルパークラス
    /// </summary>
    internal static class ImageHelper
    {
        /// <summary>
        /// 指定した ImageSource をグレースケール化する
        /// </summary>
        /// <param name="source">変換元の画像 (BitmapSource 推奨)</param>
        /// <returns>グレースケール化された ImageSource。元の画像が BitmapSource でない場合はそのまま返す。</returns>
        public static ImageSource ToGray(ImageSource source)
        {
            if (source is BitmapSource bitmap)
            {
                // FormatConvertedBitmap を使ってグレースケールに変換
                var grayBitmap = new FormatConvertedBitmap();
                grayBitmap.BeginInit();
                grayBitmap.Source = bitmap;
                grayBitmap.DestinationFormat = PixelFormats.Gray32Float; // 32bit 浮動小数点のグレースケール
                grayBitmap.EndInit();
                grayBitmap.Freeze(); // UI スレッド間で安全に使用可能にするためフリーズ
                return grayBitmap;
            }

            // BitmapSource でない場合は変換せずに返す
            return source;
        }
    }
}
