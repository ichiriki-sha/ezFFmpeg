using ezFFmpeg.Models.Common;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ezFFmpeg.Behaviors
{
    /// <summary>
    /// TextBox のキャレット位置を ViewModel と双方向に同期するためのビヘイビア。
    /// 
    /// - ユーザー操作によるキャレット移動は ViewModel に保存
    /// - Text の更新（バインディング反映）時はキャレット位置を復元
    /// 
    /// TextChanged と SelectionChanged が相互に影響し合うことで
    /// 無限ループが発生するのを防ぐため、内部フラグを使用して制御している。
    /// </summary>
    public static class TextBoxCaretBehavior
    {
        // =========================================================
        // 内部フラグ：Text 更新中かどうか
        // =========================================================

        /// <summary>
        /// TextChanged 処理中であることを示す内部用添付プロパティ。
        /// 
        /// Text 更新に伴う SelectionChanged を「ユーザー操作」と
        /// 誤認しないためのガードとして使用する。
        /// </summary>
        private static readonly DependencyProperty IsTextUpdatingProperty =
            DependencyProperty.RegisterAttached(
                "IsTextUpdating",
                typeof(bool),
                typeof(TextBoxCaretBehavior),
                new PropertyMetadata(false));

        /// <summary>
        /// IsTextUpdating 添付プロパティの setter。
        /// </summary>
        private static void SetIsTextUpdating(DependencyObject obj, bool value)
            => obj.SetValue(IsTextUpdatingProperty, value);

        /// <summary>
        /// IsTextUpdating 添付プロパティの getter。
        /// </summary>
        private static bool GetIsTextUpdating(DependencyObject obj)
            => (bool)obj.GetValue(IsTextUpdatingProperty);

        // =========================================================
        // 有効化スイッチ
        // =========================================================

        /// <summary>
        /// キャレット追従機能を有効／無効にするための添付プロパティ。
        /// true の場合、TextChanged / SelectionChanged を監視する。
        /// </summary>
        public static readonly DependencyProperty EnableCaretTrackingProperty =
            DependencyProperty.RegisterAttached(
                "EnableCaretTracking",
                typeof(bool),
                typeof(TextBoxCaretBehavior),
                new PropertyMetadata(false, OnEnableCaretTrackingChanged));

        /// <summary>
        /// EnableCaretTracking 添付プロパティの setter。
        /// </summary>
        public static void SetEnableCaretTracking(DependencyObject obj, bool value)
            => obj.SetValue(EnableCaretTrackingProperty, value);

        /// <summary>
        /// EnableCaretTracking 添付プロパティの getter。
        /// </summary>
        public static bool GetEnableCaretTracking(DependencyObject obj)
            => (bool)obj.GetValue(EnableCaretTrackingProperty);

        /// <summary>
        /// EnableCaretTracking の値が変更された際に呼ばれるコールバック。
        /// 
        /// 有効化時にイベントを登録し、
        /// 無効化時に必ず解除することでリークを防ぐ。
        /// </summary>
        private static void OnEnableCaretTrackingChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox tb)
                return;

            if ((bool)e.NewValue)
            {
                tb.TextChanged += OnTextChanged;
                tb.SelectionChanged += OnSelectionChanged;
            }
            else
            {
                tb.TextChanged -= OnTextChanged;
                tb.SelectionChanged -= OnSelectionChanged;
            }
        }

        // =========================================================
        // Text が変更されたとき
        // =========================================================

        /// <summary>
        /// TextBox の Text が変更された際に呼び出される。
        /// 
        /// バインディングによる Text 更新後、
        /// ViewModel に保存されているキャレット位置を復元する。
        /// </summary>
        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox tb)
                return;

            // Text変更中フラグON
            SetIsTextUpdating(tb, true);

            tb.Dispatcher.BeginInvoke(
                DispatcherPriority.Render,
                new Action(() =>
                {
                    // TextChanged 後の Caret 移動を無効化
                    if (tb.DataContext is FileItem item)
                    {
                        int caret = item.StartPositionCaretIndex;
                        if (caret <= tb.Text.Length)
                        {
                            tb.CaretIndex = caret;
                        }
                    }

                    // フラグ解除
                    SetIsTextUpdating(tb, false);
                }));
        }

        // =========================================================
        // Caret / Selection が変更されたとき
        // =========================================================

        /// <summary>
        /// キャレット位置や選択範囲が変更された際に呼び出される。
        /// 
        /// ユーザー操作による変更のみを検出し、
        /// ViewModel にキャレット位置を保存する。
        /// </summary>
        private static void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBox tb)
                return;

            if (tb.DataContext is not FileItem item)
                return;

            // Text変更由来なら保存しない
            if (GetIsTextUpdating(tb))
                return;

            // ユーザー操作 → 保存
            item.StartPositionCaretIndex = tb.CaretIndex;
        }
    }
}
