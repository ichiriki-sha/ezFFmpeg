using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ezFFmpeg.Behaviors
{
    /// <summary>
    /// Slider 上にマウスを移動した際、
    /// その位置に対応する再生時間をツールチップで表示し、
    /// クリック時にはその時間へ即座にジャンプさせる Behavior。
    ///
    /// ・動画のシークバー用途を想定
    /// ・MVVM を崩さずに UI 挙動を追加できる
    /// </summary>
    public class SliderTimeTooltipBehavior : Behavior<Slider>
    {
        // ------------------------------------------------------------------
        // Attached Property 定義
        // ------------------------------------------------------------------

        /// <summary>
        /// スライダー全体が表す総再生時間（秒）。
        /// Slider の Value は「秒」を直接扱う想定。
        /// </summary>
        public static readonly DependencyProperty TotalSecondsProperty =
            DependencyProperty.Register(nameof(TotalSeconds),
                typeof(double),
                typeof(SliderTimeTooltipBehavior));

        /// <summary>
        /// 総再生時間（秒）。
        /// ViewModel からバインドされる想定。
        /// </summary>
        public double TotalSeconds
        {
            get => (double)GetValue(TotalSecondsProperty);
            set => SetValue(TotalSecondsProperty, value);
        }

        // ------------------------------------------------------------------
        // フィールド
        // ------------------------------------------------------------------

        /// <summary>
        /// マウス位置に応じた時間を表示するツールチップ。
        /// </summary>
        private ToolTip? _toolTip;

        // ------------------------------------------------------------------
        // Behavior ライフサイクル
        // ------------------------------------------------------------------

        /// <summary>
        /// Behavior が Slider にアタッチされたときに呼ばれます。
        /// ツールチップの生成とマウスイベントの購読を行います。
        /// </summary>
        protected override void OnAttached()
        {
            _toolTip = new ToolTip
            {
                Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse,
                StaysOpen = true
            };

            AssociatedObject.ToolTip = _toolTip;

            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseLeave += OnMouseLeave;
            AssociatedObject.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
        }

        /// <summary>
        /// Behavior がデタッチされたときに呼ばれます。
        /// イベント購読を解除します。
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.MouseLeave -= OnMouseLeave;
            AssociatedObject.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
        }

        // ------------------------------------------------------------------
        // マウスイベント処理
        // ------------------------------------------------------------------

        /// <summary>
        /// マウス移動時に呼ばれます。
        /// マウス位置から再生時間を算出し、ツールチップに表示します。
        /// </summary>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (TotalSeconds <= 0) return;

            var slider = AssociatedObject;
            var pos = e.GetPosition(slider).X;
            var ratio = pos / slider.ActualWidth;
            ratio = Math.Clamp(ratio, 0, 1);

            var seconds = TotalSeconds * ratio;
            _toolTip!.Content = TimeSpan
                .FromSeconds(seconds)
                .ToString(@"hh\:mm\:ss\.fff");

            _toolTip.IsOpen = true;
        }

        /// <summary>
        /// マウスが Slider から離れたときに呼ばれます。
        /// ツールチップを非表示にします。
        /// </summary>
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (_toolTip != null)
                _toolTip.IsOpen = false;
        }

        /// <summary>
        /// マウス左クリック時に呼ばれます。
        /// クリック位置に対応する時間へ Slider.Value を直接設定します。
        /// </summary>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (TotalSeconds <= 0) return;

            var slider = AssociatedObject;
            var pos = e.GetPosition(slider).X;
            var ratio = pos / slider.ActualWidth;
            ratio = Math.Clamp(ratio, 0, 1);

            var seconds = TotalSeconds * ratio;

            // ★ ここが重要
            slider.Value = seconds;

            e.Handled = true; // Track の標準処理を殺す
        }
    }
}
