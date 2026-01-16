using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace ezFFmpeg.Converters
{
    /// <summary>
    /// Slider の現在値に応じた「進捗バーの横幅」を計算するコンバーター。
    /// 
    /// Slider の Track 幅から Thumb（つまみ）の幅を差し引いた有効幅を基準に、
    /// Value / Maximum の割合で進捗幅を算出する。
    /// 
    /// 主に Slider テンプレート内で、
    /// ・再生位置バー
    /// ・トリミング範囲表示
    /// などの可視化に使用する。
    /// </summary>
    public class SliderProgressWidthConverter : IMultiValueConverter
    {
        /// <summary>
        /// Slider の状態から進捗幅（double）を計算する。
        /// </summary>
        /// <param name="values">
        /// 以下の順で値を受け取る想定：
        /// [0] Value       : 現在値
        /// [1] Maximum    : 最大値
        /// [2] TrackWidth : Slider 全体の幅
        /// [3] ThumbWidth : Thumb（つまみ）の幅
        /// </param>
        /// <param name="targetType">バインド先の型（未使用）</param>
        /// <param name="parameter">追加パラメータ（未使用）</param>
        /// <param name="culture">カルチャ情報</param>
        /// <returns>
        /// 現在値に対応する進捗バーの横幅。
        /// 不正な値の場合は 0.0 を返す。
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // 想定外のバインド数の場合は進捗なし
            if (values.Length != 4) return 0.0;

            // バインドされた値を取得
            double value = (double)values[0];
            double max = (double)values[1];
            double trackWidth = (double)values[2];
            double thumbWidth = (double)values[3];

            if (max <= 0 || trackWidth <= 0) return 0.0;

            // Thumb 分を除いた、実際に進捗表示に使える幅
            double usableWidth = trackWidth - thumbWidth;
            if (usableWidth < 0) usableWidth = 0;

            return usableWidth * (value / max);
        }

        /// <summary>
        /// 逆変換は未対応（OneWay バインディング専用）
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
