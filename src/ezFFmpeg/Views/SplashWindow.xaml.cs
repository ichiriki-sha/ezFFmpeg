using ezFFmpeg.ViewModels;
using System;
using System.Windows;

namespace ezFFmpeg.Views
{
    /// <summary>
    /// SplashWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SplashWindow : Window
    {

        // ------------------------
        // フィールド
        // ------------------------
        public SplashWindowViewModel ViewModel;

        public SplashWindow()
        {
            InitializeComponent();

            ViewModel = new SplashWindowViewModel();
            DataContext = ViewModel;
        }
    }
}
