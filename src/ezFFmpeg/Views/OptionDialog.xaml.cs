using ezFFmpeg.Common;
using ezFFmpeg.ViewModels;
using ezFFmpeg.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ezFFmpeg.Services.Dialog;

namespace ezFFmpeg.Views
{
    /// <summary>
    /// OptionDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class OptionDialog : Window
    {

        public OptionDialogViewModel ViewModel;
        public OptionDialog(AppSettings settings)
        {
            InitializeComponent();

            ViewModel = new OptionDialogViewModel(settings, new DialogService(this));
            ViewModel.RequestClose += OnRequestClose;

            DataContext = ViewModel;
        }

        private void OnRequestClose(bool? result)
        {
            DialogResult = result;
            Close();
        }
    }
}
