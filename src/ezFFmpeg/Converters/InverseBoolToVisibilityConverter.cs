using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ezFFmpeg.Converters
{
    /// <summary>
    /// bool 値を反転して Visibility に変換するコンバーター。
    /// 
    /// true  → Collapsed  
    /// false → Visible  
    /// 
    /// 「false のときに表示したい」「true のときに非表示にしたい」
    /// といった UI 制御用途で使用する。
    /// </summary>
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// bool 値を反転して Visibility に変換する。
        /// </summary>
        /// <param name="value">変換元の値（bool を想定）</param>
        /// <param name="targetType">バインド先の型（通常は Visibility）</param>
        /// <param name="parameter">未使用</param>
        /// <param name="culture">カルチャ情報</param>
        /// <returns>
        /// true  → Visibility.Collapsed  
        /// false → Visibility.Visible  
        /// 
        /// bool 以外や null の場合は安全側として Visible を返す。
        /// </returns>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? Visibility.Collapsed : Visibility.Visible;
            }

            return Visibility.Visible;
        }

        /// <summary>
        /// Visibility から反転した bool 値に変換する。
        /// </summary>
        /// <param name="value">Visibility 値</param>
        /// <param name="targetType">バインド先の型（通常は bool）</param>
        /// <param name="parameter">未使用</param>
        /// <param name="culture">カルチャ情報</param>
        /// <returns>
        /// Visibility.Visible    → false  
        /// Visibility.Collapsed  → true  
        /// Visibility.Hidden     → true  
        /// 
        /// Visibility 以外の場合は false を返す。
        /// </returns>
        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is Visibility v)
            {
                // Visible 以外（Collapsed / Hidden）は true とみなす
                return v != Visibility.Visible;
            }

            return false;
        }
    }
}
