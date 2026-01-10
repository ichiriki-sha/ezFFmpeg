using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ezFFmpeg.Behaviors
{
    /// <summary>
    /// ListBox の選択が変更された際に、
    /// 指定した別の UIElement にフォーカスを移動させるためのビヘイビア。
    /// 
    /// 主に以下の用途を想定している：
    /// - ListBox で項目選択後、TextBox や Button に即座に入力フォーカスを移したい場合
    /// - キーボード操作時にフォーカスが ListBox に残り続けるのを防ぐ場合
    /// 
    /// XAML から FocusTarget を指定することで、
    /// コードビハインドを書かずにフォーカス制御が可能になる。
    /// </summary>
    public class ListBoxFocusOnSelectionBehavior
    {
        // =========================================================
        // フォーカス移動先を指定する添付プロパティ
        // =========================================================

        /// <summary>
        /// ListBox の選択変更時にフォーカスを移動させる対象 UIElement。
        /// </summary>
        public static readonly DependencyProperty FocusTargetProperty =
            DependencyProperty.RegisterAttached(
                "FocusTarget",
                typeof(UIElement),
                typeof(ListBoxFocusOnSelectionBehavior),
                new PropertyMetadata(null, OnChanged));

        /// <summary>
        /// FocusTarget 添付プロパティの setter。
        /// </summary>
        public static void SetFocusTarget(DependencyObject obj, UIElement value)
            => obj.SetValue(FocusTargetProperty, value);

        /// <summary>
        /// FocusTarget 添付プロパティの getter。
        /// </summary>
        public static UIElement GetFocusTarget(DependencyObject obj)
            => (UIElement)obj.GetValue(FocusTargetProperty);

        // =========================================================
        // 添付プロパティ変更時の処理
        // =========================================================

        /// <summary>
        /// FocusTarget が設定・変更された際に呼び出される。
        /// 
        /// 対象が ListBox の場合、SelectionChanged イベントを監視し、
        /// 選択変更のたびに指定された UIElement にフォーカスを移す。
        /// </summary>
        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ListBox lb)
                return;

            lb.SelectionChanged += (_, _) =>
            {
                if (GetFocusTarget(lb) is UIElement target)
                {
                    target.Focus();
                }
            };
        }
    }
}
