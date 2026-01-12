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

        public enum CaretTarget
        {
            Start,
            End
        }


        public static readonly DependencyProperty CaretTargetProperty =
                    DependencyProperty.RegisterAttached(
                        "CaretTarget",
                        typeof(CaretTarget),
                        typeof(TextBoxCaretBehavior),
                        new PropertyMetadata(CaretTarget.Start));

        public static void SetCaretTarget(DependencyObject obj, CaretTarget value)
            => obj.SetValue(CaretTargetProperty, value);

        public static CaretTarget GetCaretTarget(DependencyObject obj)
            => (CaretTarget)obj.GetValue(CaretTargetProperty);


        private static int GetCaretIndex(FileItem item, CaretTarget target)
        {
            return target switch
            {
                CaretTarget.Start => item.StartPositionCaretIndex,
                CaretTarget.End => item.EndPositionCaretIndex,
                _ => 0
            };
        }

        private static void SetCaretIndex(FileItem item, CaretTarget target, int value)
        {
            switch (target)
            {
                case CaretTarget.Start:
                    item.StartPositionCaretIndex = value;
                    break;
                case CaretTarget.End:
                    item.EndPositionCaretIndex = value;
                    break;
            }
        }

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
                tb.Loaded += OnTextBoxLoaded;
            }
            else
            {
                tb.TextChanged -= OnTextChanged;
                tb.SelectionChanged -= OnSelectionChanged;
                tb.Loaded -= OnTextBoxLoaded;
            }
        }

        private static void OnTextBoxLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBox tb)
                return;

            RestoreInitialCaret(tb);
        }

        private static void RestoreInitialCaret(TextBox tb)
        {
            tb.Dispatcher.BeginInvoke(
                DispatcherPriority.Render,
                new Action(() =>
                {
                    if (tb.DataContext is not FileItem item)
                        return;

                    var target = GetCaretTarget(tb);
                    int caret = GetCaretIndex(item, target);

                    if (caret <= tb.Text.Length)
                    {
                        tb.CaretIndex = caret;
                    }
                }));
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

            SetIsTextUpdating(tb, true);

            tb.Dispatcher.BeginInvoke(
                DispatcherPriority.Render,
                new Action(() =>
                {
                    if (tb.DataContext is FileItem item)
                    {
                        var target = GetCaretTarget(tb);
                        int caret = GetCaretIndex(item, target);

                        if (caret <= tb.Text.Length)
                        {
                            tb.CaretIndex = caret;
                        }
                    }

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

            if (GetIsTextUpdating(tb))
                return;

            var target = GetCaretTarget(tb);
            SetCaretIndex(item, target, tb.CaretIndex);
        }


    }
}
