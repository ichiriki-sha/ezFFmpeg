using ezFFmpeg.Common;
using ezFFmpeg.ViewModels;
using System.Windows;

namespace ezFFmpeg.Views
{
    public partial class AboutDialog : Window
    {

        public AboutDialogViewModel ViewModel;

        public AboutDialog(AppSettings settings)
        {
            InitializeComponent();
            ViewModel = new AboutDialogViewModel(settings);
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
