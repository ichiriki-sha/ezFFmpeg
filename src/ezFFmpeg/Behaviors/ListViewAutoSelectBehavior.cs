using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ezFFmpeg.Behaviors
{
    public static class ListViewAutoSelectBehavior
    {
        public static readonly DependencyProperty EnableAutoSelectProperty =
            DependencyProperty.RegisterAttached(
                "EnableAutoSelect",
                typeof(bool),
                typeof(ListViewAutoSelectBehavior),
                new PropertyMetadata(false, OnEnableAutoSelectChanged));

        public static void SetEnableAutoSelect(DependencyObject obj, bool value)
            => obj.SetValue(EnableAutoSelectProperty, value);

        public static bool GetEnableAutoSelect(DependencyObject obj)
            => (bool)obj.GetValue(EnableAutoSelectProperty);

        private static void OnEnableAutoSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement fe)
                return;

            fe.GotFocus -= OnGotFocus;

            if ((bool)e.NewValue)
                fe.GotFocus += OnGotFocus;
        }

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