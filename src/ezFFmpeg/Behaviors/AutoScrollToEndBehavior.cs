using System.Collections.Specialized;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace ezFFmpeg.Behaviors
{
    /// <summary>
    /// ListBox の Items に要素が追加された際、
    /// 常に最後のアイテムが表示されるよう自動スクロールする Behavior。
    ///
    /// 主にログ表示や進捗メッセージ一覧など、
    /// 新しい項目を下に追記していく UI で使用する。
    /// </summary>
    public class AutoScrollToEndBehavior : Behavior<ListBox>
    {
        /// <summary>
        /// Behavior が ListBox にアタッチされたタイミングで呼ばれる。
        ///
        /// Items が INotifyCollectionChanged を実装している場合、
        /// CollectionChanged イベントを購読し、
        /// アイテム追加時に自動スクロールできるようにする。
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject.Items is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged += OnCollectionChanged;
            }
        }

        /// <summary>
        /// Behavior が ListBox からデタッチされるタイミングで呼ばれる。
        ///
        /// OnAttached で登録した CollectionChanged イベントを解除し、
        /// メモリリークを防止する。
        /// </summary>
        protected override void OnDetaching()
        {
            if (AssociatedObject.Items is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged -= OnCollectionChanged;
            }

            base.OnDetaching();
        }

        /// <summary>
        /// Items コレクションの内容が変更されたときに呼ばれる。
        ///
        /// 新しいアイテムが追加された場合、
        /// ListBox の最後のアイテムが表示されるようにスクロールする。
        /// </summary>
        /// <param name="sender">イベントの送信元</param>
        /// <param name="e">コレクション変更内容</param>
        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // アイテムが存在しない場合は何もしない
            if (AssociatedObject.Items.Count == 0)
                return;

            // 最後のアイテムを取得
            var lastItem = AssociatedObject.Items[^1];

            // UI スレッド上で安全にスクロール処理を実行
            AssociatedObject.Dispatcher.InvokeAsync(() =>
            {
                AssociatedObject.ScrollIntoView(lastItem);
            });
        }
    }
}
