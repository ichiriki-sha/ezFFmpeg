using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ezFFmpeg.Behaviors
{
    /// <summary>
    /// ListView 内のコントロールがフォーカスを取得した際に、
    /// 自動的にその ListViewItem を選択状態にするための Attached Behavior。
    ///
    /// ・TextBox や Button などをクリックしたとき
    /// ・フォーカス移動のみでも行選択を同期したい場合
    ///
    /// MVVM を崩さずに ListView の選択制御を補助します。
    /// </summary>
    public static class ListViewAutoSelectBehavior
    {
        // ------------------------------------------------------------------
        // Attached Property 定義
        // ------------------------------------------------------------------

        /// <summary>
        /// 自動選択機能を有効／無効にするための Attached Property。
        /// true の場合、GotFocus イベントを監視します。
        /// </summary>
        public static readonly DependencyProperty EnableAutoSelectProperty =
            DependencyProperty.RegisterAttached(
                "EnableAutoSelect",
                typeof(bool),
                typeof(ListViewAutoSelectBehavior),
                new PropertyMetadata(false, OnEnableAutoSelectChanged));

        /// <summary>
        /// EnableAutoSelect プロパティの setter。
        /// </summary>
        public static void SetEnableAutoSelect(DependencyObject obj, bool value)
            => obj.SetValue(EnableAutoSelectProperty, value);

        /// <summary>
        /// EnableAutoSelect プロパティの getter。
        /// </summary>
        public static bool GetEnableAutoSelect(DependencyObject obj)
            => (bool)obj.GetValue(EnableAutoSelectProperty);

        // ------------------------------------------------------------------
        // Attached Property 変更時の処理
        // ------------------------------------------------------------------

        /// <summary>
        /// EnableAutoSelect の値が変更されたときに呼ばれます。
        /// GotFocus イベントの購読／解除を行います。
        /// </summary>
        private static void OnEnableAutoSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement fe)
                return;

            fe.GotFocus -= OnGotFocus;

            if ((bool)e.NewValue)
                fe.GotFocus += OnGotFocus;
        }

        // ------------------------------------------------------------------
        // フォーカス取得時の処理
        // ------------------------------------------------------------------

        /// <summary>
        /// 対象要素がフォーカスを取得したときに呼ばれます。
        /// 親の ListViewItem を特定し、対応する項目を選択状態にします。
        /// </summary>
        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is not DependencyObject d)
                return;

            var lvi = FindParent<ListViewItem>(d);
            var lv = FindParent<ListView>(d);

            if (lvi == null || lv == null)
                return;

            lv.SelectedItem = lvi.DataContext;
        }

        // ------------------------------------------------------------------
        // VisualTree ヘルパー
        // ------------------------------------------------------------------

        /// <summary>
        /// VisualTree を親方向にたどり、指定した型の親要素を探します。
        /// </summary>
        /// <typeparam name="T">探索する親要素の型</typeparam>
        /// <param name="child">探索開始点となる要素</param>
        /// <returns>見つかった親要素。存在しない場合は null。</returns>
        private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = child;

            while (parent != null)
            {
                if (parent is T t)
                    return t;

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
    }
}