using ezFFmpeg.Common;
using ezFFmpeg.Helpers;
using ezFFmpeg.Models;
using ezFFmpeg.Services;
using ezFFmpeg.Services.Dialog;
using ezFFmpeg.Services.Interfaces;
using ezFFmpeg.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ezFFmpeg.Views
{ 

    public partial class MainWindow : Window
    {
        // ------------------------
        // フィールド
        // ------------------------
        public MainWindowViewModel ViewModel;

        // ------------------------
        // コンストラクター
        // ------------------------
        public MainWindow(AppSettings settings)
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel(settings, new DialogService(this));
            DataContext = ViewModel;
        }

        // ------------------------
        // プレビューエリアのコンテキストメニューオープンイベント
        // ------------------------
        private void Preview_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;

            if (vm?.Preview?.MediaSource == null)
            {
                e.Handled = true; // ← ContextMenu を開かせない
            }
        }

        // ------------------------
        // 閉じるイベント
        // ------------------------
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!ViewModel.ConfirmExit())
                e.Cancel = true;
        }

    }
}
