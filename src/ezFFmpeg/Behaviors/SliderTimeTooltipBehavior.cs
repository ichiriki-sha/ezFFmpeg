using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ezFFmpeg.Behaviors
{
    public class SliderTimeTooltipBehavior : Behavior<Slider>
    {
        public static readonly DependencyProperty TotalSecondsProperty =
            DependencyProperty.Register(nameof(TotalSeconds),
                typeof(double),
                typeof(SliderTimeTooltipBehavior));

        public double TotalSeconds
        {
            get => (double)GetValue(TotalSecondsProperty);
            set => SetValue(TotalSecondsProperty, value);
        }

        private ToolTip? _toolTip;

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

        protected override void OnDetaching()
        {
            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.MouseLeave -= OnMouseLeave;
            AssociatedObject.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
        }

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

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (_toolTip != null)
                _toolTip.IsOpen = false;
        }

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
