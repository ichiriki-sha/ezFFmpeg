using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ezFFmpeg.Behaviors
{
    /// <summary>
    /// RepeatButton がクリックされた際に、
    /// 指定した UIElement にフォーカスを移動させるためのビヘイビア。
    /// 
    /// 主な用途：
    /// - スピンボタン（↑↓）操作後も TextBox にフォーカスを保持したい場合
    /// - ボタン操作によってフォーカスが奪われるのを防ぎたい場合
    /// 
    /// XAML から FocusTarget を指定することで、
    /// コードビハインド不要でフォーカス制御が可能。
    /// </summary>
    public static class FocusTargetBehavior
    {

        // =========================================================
        // フォーカス移動先を指定する添付プロパティ
        // =========================================================

        /// <summary>
        /// RepeatButton クリック時にフォーカスを移動させる対象 UIElement。
        /// </summary>
        public static readonly DependencyProperty FocusTargetProperty =
            DependencyProperty.RegisterAttached(
                "FocusTarget",
                typeof(UIElement),
                typeof(FocusTargetBehavior),
                new PropertyMetadata(null, OnFocusTargetChanged));

        /// <summary>
        /// FocusTarget 添付プロパティの setter。
        /// </summary>
        /// <param name="element">対象となる DependencyObject（通常は RepeatButton）</param>
        /// <param name="value">フォーカス移動先の UIElement</param>
        public static void SetFocusTarget(DependencyObject element, UIElement value)
        {
            element.SetValue(FocusTargetProperty, value);
        }

        /// <summary>
        /// FocusTarget 添付プロパティの getter。
        /// </summary>
        /// <param name="element">対象となる DependencyObject</param>
        /// <returns>設定されているフォーカス移動先 UIElement</returns>
        public static UIElement GetFocusTarget(DependencyObject element)
        {
            return (UIElement)element.GetValue(FocusTargetProperty);
        }

        // =========================================================
        // 添付プロパティ変更時の処理
        // =========================================================

        /// <summary>
        /// FocusTarget 添付プロパティが設定・変更された際に呼び出される。
        /// 
        /// 対象が RepeatButton の場合、Click イベントをフックし、
        /// クリック時に指定された UIElement へフォーカスを移動する。
        /// </summary>
        private static void OnFocusTargetChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is RepeatButton button && e.NewValue is UIElement target)
            {
                button.Click += (_, _) =>
                {
                    if (target.Focusable)
                        target.Focus();
                };
            }
        }
    }
}
