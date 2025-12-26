using System;
using System.Globalization;
using System.Windows.Data;

namespace ezFFmpeg.Converters
{
    /// <summary>
    /// <see cref="TimeSpan"/> を「hh:mm:ss」形式の文字列に変換するコンバーター。
    /// 主に WPF の Binding で再生時間などを表示するために使用する。
    /// </summary>
    public class TimeSpanToStringConverter : IValueConverter
    {
        /// <summary>
        /// <see cref="TimeSpan"/> を文字列に変換する。
        /// 秒未満（ミリ秒以下）は切り捨て、「hh:mm:ss」形式で返す。
        /// </summary>
        /// <param name="value">バインド元の値（TimeSpan を想定）</param>
        /// <param name="targetType">バインド先の型</param>
        /// <param name="parameter">未使用</param>
        /// <param name="culture">カルチャ情報</param>
        /// <returns>
        /// TimeSpan の場合は「hh:mm:ss」形式の文字列。
        /// それ以外の場合は "00:00:00" を返す。
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 値が TimeSpan の場合のみ変換処理を行う
            if (value is TimeSpan ts)
            {
                // 総秒数を整数に変換し、1秒未満を切り捨てる
                var totalSeconds = (int)ts.TotalSeconds;

                // 切り捨て後の秒数から TimeSpan を再生成
                var truncated = TimeSpan.FromSeconds(totalSeconds);

                // hh:mm:ss 形式で文字列化
                return truncated.ToString(@"hh\:mm\:ss");
            }

            // 想定外の型の場合はデフォルト値を返す
            return "00:00:00";
        }

        /// <summary>
        /// 逆変換はサポートしない。
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// ConvertBack は使用しないため常に例外をスローする。
        /// </exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
