using ezFFmpeg.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ezFFmpeg.Behaviors
{
    /// <summary>
    /// Button 内に配置された Image を対象に、
    /// Button の IsEnabled 状態に応じて
    /// 画像をカラー / グレースケールで自動切り替えするための Attached Behavior。
    ///
    /// XAML から EnableGrayImage="True" を指定するだけで有効化できる。
    /// </summary>
    public static class GrayImageBehavior
    {
        /// <summary>
        /// EnableGrayImage 添付プロパティの Getter。
        /// </summary>
        public static bool GetEnableGrayImage(DependencyObject obj)
            => (bool)obj.GetValue(EnableGrayImageProperty);

        /// <summary>
        /// EnableGrayImage 添付プロパティの Setter。
        /// </summary>
        public static void SetEnableGrayImage(DependencyObject obj, bool value)
            => obj.SetValue(EnableGrayImageProperty, value);

        /// <summary>
        /// Button に対してグレースケール画像切り替え機能を有効化するかどうかを表す添付プロパティ。
        ///
        /// True に設定された場合、Button の Loaded タイミングで
        /// 内部の Image を探索し、状態切り替え処理を登録する。
        /// </summary>
        public static readonly DependencyProperty EnableGrayImageProperty =
            DependencyProperty.RegisterAttached(
                "EnableGrayImage",
                typeof(bool),
                typeof(GrayImageBehavior),
                new PropertyMetadata(false, OnEnableGrayImageChanged));

        /// <summary>
        /// EnableGrayImage プロパティが変更されたときに呼ばれるコールバック。
        ///
        /// 対象が Button の場合のみ処理を行い、
        /// Loaded イベントにハンドラを登録する。
        /// </summary>
        private static void OnEnableGrayImageChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not Button button)
                return;

            if ((bool)e.NewValue)
            {
                // Loaded イベントの多重登録を防止
                button.Loaded -= Button_Loaded;
                button.Loaded += Button_Loaded;
            }
        }

        /// <summary>
        /// Button の Loaded イベントハンドラ。
        ///
        /// ・Button 配下の Image を探索
        /// ・BitmapSource であることを確認
        /// ・有効 / 無効に応じたグレースケール切替処理を登録
        /// </summary>
        private static void Button_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            // Button 内から Image を探索
            var image = FindChildImage(button);

            // Image または BitmapSource が見つからない場合は処理しない
            if (image?.Source is not BitmapSource bmp)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[GrayBehavior] Image not found or not BitmapSource: {button.Name}");
                return;
            }

            // ★ Button の IsEnabled に応じて
            //   カラー / グレースケールを切り替える処理を登録
            ButtonImageHelper.SetupGrayImageSwitch(button, image, bmp);

            System.Diagnostics.Debug.WriteLine(
                $"[GrayBehavior] OK Registered: {button.Name}");
        }

        /// <summary>
        /// 指定した DependencyObject の VisualTree を再帰的に探索し、
        /// 最初に見つかった Image 要素を返す。
        /// </summary>
        /// <param name="parent">探索開始ノード</param>
        /// <returns>見つかった Image。存在しない場合は null。</returns>
        private static Image? FindChildImage(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is Image img)
                    return img;

                var descendant = FindChildImage(child);
                if (descendant != null)
                    return descendant;
            }
            return null;
        }
    }
}
