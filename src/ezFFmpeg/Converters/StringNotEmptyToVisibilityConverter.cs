using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ezFFmpeg.Converters
{
    /// <summary>
    /// 文字列が「空でない」場合に Visibility.Visible を返すコンバーター。
    /// 
    /// 主な用途：
    /// ・エラーメッセージやステータスメッセージの表示制御
    /// ・ログや説明文が存在する場合のみ UI を表示する
    /// 
    /// 判定ルール：
    /// ・null
    /// ・空文字列 ("")
    /// ・空白のみの文字列 ("   ")
    /// 
    /// 上記いずれかの場合 → Visibility.Collapsed  
    /// それ以外            → Visibility.Visible
    /// </summary>
    public class StringNotEmptyToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 文字列が null / 空 / 空白のみかどうかを判定し、
        /// 表示状態（Visibility）に変換する。
        /// </summary>
        /// <param name="value">判定対象の文字列</param>
        /// <param name="targetType">変換先の型（Visibility）</param>
        /// <param name="parameter">未使用</param>
        /// <param name="culture">カルチャ情報</param>
        /// <returns>
        /// null / 空 / 空白のみ → Visibility.Collapsed  
        /// それ以外              → Visibility.Visible
        /// </returns>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace(value as string)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        /// <summary>
        /// 逆変換はサポートしない。
        /// 
        /// Visibility から元の文字列状態を復元することはできないため、
        /// 本コンバーターは OneWay バインディング専用。
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// 常にスローされる
        /// </exception>
        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException(
                "StringNotEmptyToVisibilityConverter does not support ConvertBack.");
        }
    }
}
