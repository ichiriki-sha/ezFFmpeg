using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ezFFmpeg.Behaviors
{
    /// <summary>
    /// ListView のアイテムクリックを ICommand として ViewModel に通知するための Behavior。
    /// 
    /// ・ListViewItem がクリックされたとき
    /// ・その DataContext を引数として Command を実行する
    /// 
    /// MVVM パターンで、コードビハインドを書かずに
    /// アイテムクリック処理を実装する目的で使用する。
    /// </summary>
    public class ListViewClickBehavior : Behavior<ListView>
    {
        /// <summary>
        /// ListViewItem がクリックされた際に実行されるコマンド。
        /// 引数には、クリックされた ListViewItem の DataContext が渡される。
        /// </summary>
        public ICommand? ItemClickCommand
        {
            get => (ICommand)GetValue(ItemClickCommandProperty);
            set => SetValue(ItemClickCommandProperty, value);
        }

        /// <summary>
        /// ItemClickCommand 用の DependencyProperty。
        /// XAML からバインドできるようにするために定義。
        /// </summary>
        public static readonly DependencyProperty ItemClickCommandProperty =
            DependencyProperty.Register(
                nameof(ItemClickCommand),
                typeof(ICommand),
                typeof(ListViewClickBehavior),
                new PropertyMetadata(null));

        /// <summary>
        /// Behavior が ListView にアタッチされたときに呼ばれる。
        /// PreviewMouseLeftButtonDown を監視し、
        /// クリックされた ListViewItem を検出できるようにする。
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseLeftButtonDown += OnMouseDown;
        }

        /// <summary>
        /// Behavior が ListView からデタッチされたときに呼ばれる。
        /// イベントハンドラを解除し、メモリリークを防止する。
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseLeftButtonDown -= OnMouseDown;
            base.OnDetaching();
        }

        /// <summary>
        /// マウス左ボタン押下時の処理。
        /// クリック位置から VisualTree を遡り、
        /// ListViewItem がクリックされたかを判定する。
        /// </summary>
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            // クリックされた要素（TextBlock / Border / Image 等）を起点にする
            var original = e.OriginalSource as DependencyObject;

            // VisualTree を親方向へ辿り、ListViewItem を探す
            while (original != null && original is not ListViewItem)
                original = VisualTreeHelper.GetParent(original);

            // ListViewItem 以外がクリックされた場合は何もしない
            if (original is not ListViewItem item)
                return;

            // ListViewItem にバインドされているデータを取得
            var fileItem = item.DataContext;

            // Command が実行可能であれば実行する
            if (ItemClickCommand?.CanExecute(fileItem) == true)
            {
                ItemClickCommand.Execute(fileItem);
            }
        }
    }
}
