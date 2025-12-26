using ezFFmpeg.ViewModels;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ezFFmpeg.Behaviors
{
    /// <summary>
    /// Slider と PreviewViewModel を橋渡しし、
    /// ・Thumbドラッグ中の一時停止
    /// ・ドラッグ確定時のシーク
    /// ・バークリック時の即時シーク
    /// を実現する Behavior。
    /// 
    /// MediaElement 等の再生制御は PreviewViewModel 側に委譲し、
    /// View にはロジックを持たせない。
    /// </summary>
    public class SliderSeekBehavior : Behavior<Slider>
    {
        #region DependencyProperty

        /// <summary>
        /// 再生制御を行う PreviewViewModel
        /// </summary>
        public PreviewViewModel? Preview
        {
            get => (PreviewViewModel?)GetValue(PreviewProperty);
            set => SetValue(PreviewProperty, value);
        }

        public static readonly DependencyProperty PreviewProperty =
            DependencyProperty.Register(
                nameof(Preview),
                typeof(PreviewViewModel),
                typeof(SliderSeekBehavior),
                new PropertyMetadata(null));

        #endregion

        private Thumb? _thumb;

        #region Behavior Lifecycle

        protected override void OnAttached()
        {
            AssociatedObject.Loaded += OnLoaded;
            AssociatedObject.ValueChanged += OnValueChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnLoaded;
            AssociatedObject.ValueChanged -= OnValueChanged;

            if (_thumb != null)
            {
                _thumb.DragStarted -= OnDragStarted;
                _thumb.DragCompleted -= OnDragCompleted;
            }
        }

        #endregion

        #region Slider / Thumb Events

        /// <summary>
        /// Slider の Template から Thumb を取得し、
        /// ドラッグイベントを購読する。
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _thumb = AssociatedObject.Template
                .FindName("PART_Track", AssociatedObject) is Track track
                ? track.Thumb
                : null;

            if (_thumb != null)
            {
                _thumb.DragStarted += OnDragStarted;
                _thumb.DragCompleted += OnDragCompleted;
            }
        }

        /// <summary>
        /// Thumb ドラッグ開始時：
        /// 再生を一時停止し、シーク中フラグを立てる。
        /// </summary>
        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            if (Preview == null) return;

            Preview.IsSeeking = true;
            Preview.RequestPause();
        }

        /// <summary>
        /// Thumb ドラッグ確定時：
        /// シークを実行し、再生状態を復帰する。
        /// </summary>
        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (Preview == null) return;

            Preview.IsSeeking = false;

            // 確定位置へシーク
            Preview.RequestSeek(Preview.CurrentSeconds);

            if (Preview.IsPlaying)
            {
                Preview.RequestPlay();
            }
        }

        /// <summary>
        /// バークリック等で Value が変更された場合の処理。
        /// ドラッグ中は DragCompleted 側で処理するため無視する。
        /// </summary>
        private void OnValueChanged(
            object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if (Preview == null) return;

            // ドラッグ中は確定シークを行わない
            if (Preview.IsSeeking) return;

            // バークリックによる即時シーク
            Preview.RequestSeek(e.NewValue);
        }

        #endregion
    }
}
