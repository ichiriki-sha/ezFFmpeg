using System;
using System.Globalization;
using System.Windows.Data;

namespace ezFFmpeg.Converters
{
    /// <summary>
    /// 数値（double）を符号反転するためのコンバーター。
    /// 
    /// 例：
    ///  10.0  → -10.0  
    /// -5.5  →  5.5  
    /// 
    /// 主に以下の用途で使用する：
    /// ・スライダーやレイアウト計算の向きを反転したい場合
    /// ・Transform や Margin などで逆方向の値を使いたい場合
    /// </summary>
    public class InverseDoubleConverter : IValueConverter
    {
        /// <summary>
        /// double 値を符号反転して返す。
        /// </summary>
        /// <param name="value">変換元の値（数値を想定）</param>
        /// <param name="targetType">バインド先の型（通常は double）</param>
        /// <param name="parameter">未使用</param>
        /// <param name="culture">カルチャ情報</param>
        /// <returns>
        /// 数値の場合は符号反転した double を返す。
        /// null の場合は 0 を返す。
        /// </returns>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value == null)
                return 0d;

            return -System.Convert.ToDouble(value, culture);
        }

        /// <summary>
        /// Convert と同様に、符号反転を行う逆変換。
        /// 
        /// 同じ処理を行うことで、TwoWay バインディングでも
        /// 往復変換が正しく成立する。
        /// </summary>
        /// <param name="value">変換元の値（数値を想定）</param>
        /// <param name="targetType">バインド先の型</param>
        /// <param name="parameter">未使用</param>
        /// <param name="culture">カルチャ情報</param>
        /// <returns>
        /// 符号反転した double 値。
        /// null の場合は 0 を返す。
        /// </returns>
        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value == null)
                return 0d;

            return -System.Convert.ToDouble(value, culture);
        }
    }
}
