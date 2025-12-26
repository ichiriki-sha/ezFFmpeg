using ezFFmpeg.Common;
using ezFFmpeg.Services;
using ezFFmpeg.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ezFFmpeg.Models.Common;
using ezFFmpeg.Services.Dialog;

namespace ezFFmpeg.Views
{
    /// <summary>
    /// ParameterDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ParameterDialog : Window
    {
        public ParameterDialogViewModel ViewModel;

        public ParameterDialog(AppSettings settings, ParameterDialogMode mode, Profile profile)
        {
            InitializeComponent();

            ViewModel = new ParameterDialogViewModel(settings, new DialogService(this), mode, profile);
            ViewModel.RequestClose += OnRequestClose;

            this.DataContext = ViewModel;
        }

        private void OnRequestClose(bool? result)
        {
            DialogResult = result;
            Close();
        }
    }
}
