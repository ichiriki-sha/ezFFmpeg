using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ezFFmpeg.Behaviors
{
    /// <summary>
    /// TextBox の Loaded 時にキャレット（カーソル）を
    /// テキスト末尾へ自動的に移動させるためのビヘイビアクラス。
    /// 
    /// XAML から Attached Property として指定することで、
    /// コードビハインドを書かずに動作を適用できる。
    /// </summary>
    public static class TextBoxCaretInitializer
    {
        /// <summary>
        /// TextBox の Loaded 時にキャレットを末尾へ移動するかどうかを指定する
        /// 添付プロパティ。
        /// </summary>
        public static readonly DependencyProperty CaretToEndOnLoadProperty =
            DependencyProperty.RegisterAttached(
                "CaretToEndOnLoad",
                typeof(bool),
                typeof(TextBoxCaretInitializer),
                new PropertyMetadata(false, OnChanged));

        /// <summary>
        /// CaretToEndOnLoad 添付プロパティの setter。
        /// </summary>
        /// <param name="obj">対象となる DependencyObject（通常は TextBox）</param>
        /// <param name="value">true の場合、Loaded 時にキャレットを末尾へ移動する</param>
        public static void SetCaretToEndOnLoad(DependencyObject obj, bool value)
            => obj.SetValue(CaretToEndOnLoadProperty, value);

        /// <summary>
        /// CaretToEndOnLoad 添付プロパティの getter。
        /// </summary>
        /// <param name="obj">対象となる DependencyObject</param>
        /// <returns>設定されている値</returns>
        public static bool GetCaretToEndOnLoad(DependencyObject obj)
            => (bool)obj.GetValue(CaretToEndOnLoadProperty);

        /// <summary>
        /// CaretToEndOnLoad 添付プロパティが変更された際に呼び出されるコールバック。
        /// </summary>
        /// <param name="d">プロパティが設定されたオブジェクト</param>
        /// <param name="e">変更内容</param>
        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // TextBox 以外、または false 指定時は何もしない
            if (d is not TextBox tb || !(bool)e.NewValue)
                return;

            // Loaded 後にキャレット移動を行う
            tb.Loaded += (_, _) =>
            {
                // レイアウト・描画完了後に処理するため Dispatcher を使用
                tb.Dispatcher.BeginInvoke(
                    DispatcherPriority.Render,
                    new Action(() =>
                    {
                        // キャレットをテキスト末尾へ移動
                        tb.CaretIndex = tb.Text.Length;
                    }));
            };
        }
    }

}
