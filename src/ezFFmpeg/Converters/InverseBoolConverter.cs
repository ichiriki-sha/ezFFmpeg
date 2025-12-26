using System;
using System.Globalization;
using System.Windows.Data;

namespace ezFFmpeg.Converters
{
    /// <summary>
    /// bool 値を反転（true ⇔ false）するコンバーター。
    /// 
    /// WPF の XAML バインディングで、
    /// 「有効／無効」「表示／非表示」などの条件を
    /// 反転させたい場合に使用する。
    /// </summary>
    public class InverseBoolConverter : IValueConverter
    {
        /// <summary>
        /// bool 値を反転して返す。
        /// </summary>
        /// <param name="value">変換元の値（bool または null を想定）</param>
        /// <param name="targetType">バインド先の型</param>
        /// <param name="parameter">未使用</param>
        /// <param name="culture">カルチャ情報</param>
        /// <returns>
        /// true → false  
        /// false → true  
        /// null や bool 以外の場合は false として扱い、true を返す。
        /// </returns>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            // bool? として受け取り、null の場合は false とみなす
            return !(value as bool? ?? false);
        }

        /// <summary>
        /// 逆変換はサポートしない。
        /// </summary>
        /// <remarks>
        /// このコンバーターは OneWay バインディングでの使用を想定しているため、
        /// ConvertBack は実装しない。
        /// </remarks>
        /// <exception cref="NotImplementedException">
        /// 常にスローされる。
        /// </exception>
        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
