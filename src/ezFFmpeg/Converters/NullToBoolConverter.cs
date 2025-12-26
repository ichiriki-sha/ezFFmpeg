using System;
using System.Globalization;
using System.Windows.Data;

namespace ezFFmpeg.Converters
{
    /// <summary>
    /// 値が null かどうかを判定し、bool に変換するコンバーター。
    /// 
    /// 主な用途：
    /// ・MediaSource が設定されているかの判定
    /// ・オブジェクトの存在有無で表示／非表示や有効／無効を制御
    /// 
    /// 例：
    ///  value == null      → false
    ///  value != null      → true
    /// </summary>
    public class NullToBoolConverter : IValueConverter
    {
        /// <summary>
        /// null でなければ true、null の場合は false を返す。
        /// </summary>
        /// <param name="value">判定対象の値</param>
        /// <param name="targetType">変換先の型（通常は bool）</param>
        /// <param name="parameter">未使用</param>
        /// <param name="culture">カルチャ情報</param>
        /// <returns>
        /// value が null でなければ true、それ以外は false
        /// </returns>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return value != null;
        }

        /// <summary>
        /// 逆変換はサポートしない。
        /// 
        /// null チェックは一方向の判定用途であり、
        /// bool から元のオブジェクトを復元できないため
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
            throw new NotSupportedException("NullToBoolConverter does not support ConvertBack.");
        }
    }
}
