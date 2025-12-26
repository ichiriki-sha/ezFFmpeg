using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ezFFmpeg.Converters
{
    /// <summary>
    /// 値が null かどうかを判定し、Visibility に変換するコンバーター。
    /// 
    /// 主な用途：
    /// ・MediaSource が存在する場合のみ UI を表示する
    /// ・オブジェクトの有無に応じてコントロールを表示／非表示する
    /// 
    /// 変換ルール：
    ///  value == null      → Visibility.Collapsed
    ///  value != null      → Visibility.Visible
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 値が null の場合は Collapsed、それ以外は Visible を返す。
        /// </summary>
        /// <param name="value">判定対象の値</param>
        /// <param name="targetType">変換先の型（Visibility）</param>
        /// <param name="parameter">未使用</param>
        /// <param name="culture">カルチャ情報</param>
        /// <returns>
        /// null → Visibility.Collapsed  
        /// 非 null → Visibility.Visible
        /// </returns>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return value == null
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        /// <summary>
        /// 逆変換はサポートしない。
        /// 
        /// Visibility から元のオブジェクトを復元することはできず、
        /// このコンバーターは一方向バインディング用途のみを想定しているため、
        /// ConvertBack は例外を投げる。
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// 常にスローされる
        /// </exception>
        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException(
                "NullToVisibilityConverter does not support ConvertBack.");
        }
    }
}
