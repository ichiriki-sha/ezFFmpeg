using Microsoft.Xaml.Behaviors;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ezFFmpeg.Behaviors
{
    /// <summary>
    /// ListView の項目をドラッグ＆ドロップで並び替える Behavior
    /// ItemsSource は IList を実装している必要がある
    /// </summary>
    public class ListViewDragDropBehavior : Behavior<ListView>
    {
        private object? _dragItem;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewMouseLeftButtonDown += OnMouseDown;
            AssociatedObject.PreviewMouseMove += OnMouseMove;
            AssociatedObject.Drop += OnDrop;
            AssociatedObject.AllowDrop = true;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseLeftButtonDown -= OnMouseDown;
            AssociatedObject.PreviewMouseMove -= OnMouseMove;
            AssociatedObject.Drop -= OnDrop;

            base.OnDetaching();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _dragItem = GetItemUnderMouse(e.GetPosition(AssociatedObject));
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || _dragItem == null)
                return;

            DragDrop.DoDragDrop(
                AssociatedObject,
                _dragItem,
                DragDropEffects.Move);
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (_dragItem == null)
                return;

            var targetItem = GetItemUnderMouse(e.GetPosition(AssociatedObject));
            if (targetItem == null || ReferenceEquals(_dragItem, targetItem))
                return;

            if (AssociatedObject.ItemsSource is not IList list)
                return;

            int oldIndex = list.IndexOf(_dragItem);
            int newIndex = list.IndexOf(targetItem);

            if (oldIndex < 0 || newIndex < 0)
                return;

            list.RemoveAt(oldIndex);
            list.Insert(newIndex, _dragItem);

            AssociatedObject.SelectedItem = _dragItem;
            _dragItem = null;
        }

        private object? GetItemUnderMouse(Point point)
        {
            var element = AssociatedObject.InputHitTest(point) as DependencyObject;

            while (element != null && element is not ListViewItem)
                element = System.Windows.Media.VisualTreeHelper.GetParent(element);

            return (element as ListViewItem)?.DataContext;
        }
    }
}
