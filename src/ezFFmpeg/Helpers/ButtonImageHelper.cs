using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ezFFmpeg.Helpers
{
    /// <summary>
    /// <see cref="Button"/> の有効／無効状態に応じて、
    /// 表示する <see cref="Image"/> を通常画像／グレー画像に自動で切り替える
    /// ヘルパークラス。
    /// 
    /// XAML で IsEnabled によるスタイル制御が難しい場合や、
    /// 画像の見た目を明示的に切り替えたい場合に使用する。
    /// </summary>
    public static class ButtonImageHelper
    {
        /// <summary>
        /// ボタンの IsEnabled 状態に応じて、
        /// 画像を通常画像とグレー画像で自動切り替えする設定を行う。
        /// </summary>
        /// <param name="button">
        /// 状態監視対象のボタン
        /// </param>
        /// <param name="image">
        /// 表示を切り替える Image コントロール
        /// </param>
        /// <param name="normalSource">
        /// ボタンが有効時に表示する元画像
        /// </param>
        public static void SetupGrayImageSwitch(
            Button button,
            Image image,
            BitmapSource normalSource)
        {
            // 元画像から、アルファ値を保持したグレー画像を生成
            var grayImage = ToGrayKeepAlpha(normalSource);

            // ボタンの有効／無効切り替え時に画像を変更
            button.IsEnabledChanged += (s, e) =>
            {
                image.Source = button.IsEnabled
                    ? normalSource
                    : grayImage;
            };

            // 初期状態にも即座に反映
            image.Source = button.IsEnabled
                ? normalSource
                : grayImage;
        }

        /// <summary>
        /// 元画像のアルファ値（透過）を維持したまま、
        /// グレー画像へ変換する。
        /// </summary>
        /// <param name="source">
        /// 変換元の BitmapSource（BGRA32 を想定）
        /// </param>
        /// <returns>
        /// アルファ値を保持したグレー画像
        /// </returns>
        private static BitmapSource ToGrayKeepAlpha(BitmapSource source)
        {
            int width = source.PixelWidth;
            int height = source.PixelHeight;
            int stride = width * 4;

            // グレーの明度（固定値）
            byte grayValue = 160;

            // ピクセルデータを格納
            byte[] pixels = new byte[height * stride];
            source.CopyPixels(pixels, stride, 0);

            // BGRA32 形式で各ピクセルを処理
            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte a = pixels[i + 3];

                // 完全透過ピクセルは処理しない
                if (a == 0) continue;

                // RGB を固定グレー値に変換（アルファは保持）
                pixels[i] = grayValue; // B
                pixels[i + 1] = grayValue; // G
                pixels[i + 2] = grayValue; // R
                pixels[i + 3] = a;         // A
            }

            // 新しい BitmapSource を生成して返す
            return BitmapSource.Create(
                width,
                height,
                96,
                96,
                PixelFormats.Bgra32,
                null,
                pixels,
                stride);
        }
    }
}
