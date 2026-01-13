using ezFFmpeg.Helpers;
using ezFFmpeg.Models.Common;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ezFFmpeg.ViewModels
{
    /// <summary>
    /// メディアプレビュー専用 ViewModel
    /// 再生状態・再生位置・UI 表示状態のみを管理する
    /// </summary>
    public sealed class PreviewViewModel : BindableBase
    {
        //// View → MediaElement 要求
        public event Action? PlayRequested;
        public event Action? PauseRequested;
        public event Action<double>? SeekRequested;

        public void RequestPlay() => PlayRequested?.Invoke();
        public void RequestPause() => PauseRequested?.Invoke();
        public void RequestSeek(double seconds) => SeekRequested?.Invoke(seconds);

        private Uri? _mediaSource;
        public Uri? MediaSource
        {
            get => _mediaSource;
            set
            {
                SetProperty(ref _mediaSource, value);
                if (MediaSource == null)
                    MediaErrorMessage = null;
            }
        }

        private bool _isMediaOpened;
        public bool IsMediaOpened
        {
            get => _isMediaOpened;
            internal set => SetProperty(ref _isMediaOpened, value);
        }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get => _isPlaying;
            internal set
            {
                if (SetProperty(ref _isPlaying, value))
                    RaisePropertyChanged(nameof(PlayPauseImage));
            }
        }

        private double _currentSeconds;
        public double CurrentSeconds
        {
            get => _currentSeconds;
            set
            {
                if (SetProperty(ref _currentSeconds, value))
                {
                    CurrentTimeText = IsMediaOpened
                        ? TimeSpan.FromSeconds(value).ToString(@"hh\:mm\:ss\.fff")
                        : "00:00:00.000";

                    if (IsSeeking)
                    {
                        SeekRequested?.Invoke(value);
                    }
                }
            }
        }

        private string _currentTimeText = "00:00:00.000";
        public string CurrentTimeText
        {
            get => _currentTimeText;
            private set => SetProperty(ref _currentTimeText, value);
        }

        private double _totalSeconds;
        public double TotalSeconds
        {
            get => _totalSeconds;
            internal set => SetProperty(ref _totalSeconds, value);
        }

        private bool _isSeeking;
        public bool IsSeeking
        {
            get => _isSeeking;
            set => SetProperty(ref _isSeeking, value);
        }

        public ImageSource PlayPauseImage =>
            IsPlaying
                ? new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Pause.png"))
                : new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Play.png"));

        private string? _mediaErrorMessage;
        public string? MediaErrorMessage
        {
            get => _mediaErrorMessage;
            set => SetProperty(ref _mediaErrorMessage, value);
        }

        public ICommand TogglePlayPauseCommand { get; }

        public PreviewViewModel()
        {
            TogglePlayPauseCommand = new RelayCommand(TogglePlayPause);
        }

        // ------------------------
        // 操作 API
        // ------------------------

        public void SetSource(FileItem? item)
        {
            // 再生中のみ停止
            if (IsPlaying)
            {
                PauseRequested?.Invoke();
            }

            IsPlaying = false;
            IsMediaOpened = false;

            if (item == null || !File.Exists(item.FilePath))
            {
                MediaSource = null;
                return;
            }

            MediaSource = new Uri(item.FilePath);
            CurrentSeconds = 0;
            CurrentTimeText = "00:00:00.000";
            TotalSeconds = item.VideoDuration.TotalSeconds;
            MediaErrorMessage = null;
        }

        private void Play() => PlayRequested?.Invoke();
        private void Pause() => PauseRequested?.Invoke();

        public void TogglePlayPause()
        {
            if (IsPlaying) Pause();
            else Play();
        }
    }
}
