using ezFFmpeg.ViewModels;
using Microsoft.Xaml.Behaviors;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace ezFFmpeg.Behaviors
{
    /// <summary>
    /// MediaElement と MainWindowViewModel を仲介し、
    /// 再生・一時停止・シーク・再生状態同期を行うための Behavior。
    /// 
    /// MediaElement は MVVM と相性が悪いため、
    /// ・イベント
    /// ・再生制御
    /// ・再生状態管理
    /// をこの Behavior に集約している。
    /// </summary>
    public class MediaElementControlBehavior : Behavior<MediaElement>
    {
        private bool _mediaOpened;
        private bool _playRequestedBeforeOpen;
        private DispatcherTimer? _timer;

        public PreviewViewModel? Preview
        {
            get => (PreviewViewModel?)GetValue(PreviewProperty);
            set => SetValue(PreviewProperty, value);
        }

        public static readonly DependencyProperty PreviewProperty =
            DependencyProperty.Register(
                nameof(Preview),
                typeof(PreviewViewModel),
                typeof(MediaElementControlBehavior),
                new PropertyMetadata(null, OnPreviewChanged));

        protected override void OnAttached()
        {
            AssociatedObject.MediaOpened += OnMediaOpened;
            AssociatedObject.MediaEnded += OnMediaEnded;
            AssociatedObject.MediaFailed += OnMediaFailed;
            StartTimer();
            Attach(Preview);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MediaOpened -= OnMediaOpened;
            AssociatedObject.MediaEnded -= OnMediaEnded;
            AssociatedObject.MediaFailed -= OnMediaFailed;

            Attach(null);
        }

        private static void OnPreviewChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var b = (MediaElementControlBehavior)d;
            b.Attach(e.NewValue as PreviewViewModel);
        }

        private void Attach(PreviewViewModel? vm)
        {
            if (vm == null) return;

            vm.PlayRequested += Play;
            vm.PauseRequested += Pause;
            vm.SeekRequested += Seek;
        }

        private void Play()
        {
            if (!_mediaOpened)
            {
                _playRequestedBeforeOpen = true;
                AssociatedObject.Play();
                AssociatedObject.Stop();
                return;
            }

            _timer!.Start();
            AssociatedObject.Play();
            Preview!.IsPlaying = true;
            Preview!.IsMediaOpened = true;        // プレイのタイミングでオープン済みにする
        }

        private void Pause()
        {
            _timer!.Stop();
            AssociatedObject.Pause();
            Preview!.IsPlaying = false;
        }

        private void Seek(double seconds)
        {
            if (_mediaOpened)
            {
                AssociatedObject.Position = TimeSpan.FromSeconds(seconds);

                if (!Preview!.IsPlaying)
                {
                    Play();
                    Pause();
                }
            }
        }

        private void OnMediaOpened(object? sender, RoutedEventArgs e)
        {
            _mediaOpened = true;

            if (AssociatedObject.NaturalDuration.HasTimeSpan)
                Preview!.TotalSeconds =
                    AssociatedObject.NaturalDuration.TimeSpan.TotalSeconds;

            // ★ 明示的に再生要求があった場合のみ再生
            if (_playRequestedBeforeOpen )
            {
                _playRequestedBeforeOpen = false;
                Play();
            }
        }

        private void OnMediaEnded(object? sender, RoutedEventArgs e)
        {
            //_mediaOpened = false;
            Preview!.IsPlaying = false;
            AssociatedObject.Stop();
        }

        private void OnMediaFailed(object? sender, ExceptionRoutedEventArgs e)
        {
            _mediaOpened = false;
            Preview!.IsMediaOpened = false;
            Preview!.IsPlaying = false;
            AssociatedObject.Stop();

            Preview!.MediaErrorMessage = "この動画はプレビュー再生できません。\n" +
                                         "（コーデックまたは形式が未対応です）";
        }

        private void StartTimer()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            _timer.Tick += (_, _) =>
            {
                if (_mediaOpened && Preview != null && !Preview.IsSeeking)
                    Preview!.CurrentSeconds =
                        AssociatedObject.Position.TotalSeconds;
            };
            _timer.Start();
        }
    }
}
