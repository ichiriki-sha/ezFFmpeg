using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ezFFmpeg.Converters
{
    /// <summary>
    /// bool 値を <see cref="Visibility"/> に変換するコンバーター。
    /// 
    /// 主に WPF の XAML バインディングにおいて、
    /// フラグによる UI 表示／非表示制御に使用する。
    /// </summary>
    /// <remarks>
    /// true  → Visibility.Visible  
    /// false → Visibility.Collapsed  
    /// 
    /// Collapsed を使用することで、
    /// 非表示時にレイアウト領域を占有しない。
    /// </remarks>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// bool 値を Visibility に変換する。
        /// </summary>
        /// <param name="value">変換元の値（bool を想定）</param>
        /// <param name="targetType">バインド先の型</param>
        /// <param name="parameter">未使用</param>
        /// <param name="culture">カルチャ情報</param>
        /// <returns>
        /// value が true の場合は <see cref="Visibility.Visible"/>、
        /// false の場合は <see cref="Visibility.Collapsed"/> を返す。
        /// </returns>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? Visibility.Visible : Visibility.Collapsed;
            }

            // 想定外の型が渡された場合は安全側として Visible を返す
            return Visibility.Visible;
        }

        /// <summary>
        /// Visibility 値を bool に逆変換する。
        /// </summary>
        /// <param name="value">変換元の値（Visibility を想定）</param>
        /// <param name="targetType">バインド先の型</param>
        /// <param name="parameter">未使用</param>
        /// <param name="culture">カルチャ情報</param>
        /// <returns>
        /// <see cref="Visibility.Visible"/> の場合は true、
        /// それ以外の場合は false。
        /// </returns>
        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is Visibility v)
            {
                return v == Visibility.Visible;
            }

            return false;
        }
    }
}
