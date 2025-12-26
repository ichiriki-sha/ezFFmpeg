using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace ezFFmpeg.Behaviors
{
    /// <summary>
    /// ListView にファイルのドラッグ＆ドロップを可能にし、
    /// ドロップされたファイルパス配列を ICommand 経由で ViewModel に通知する Behavior。
    ///
    /// View にコードビハインドを記述せず、
    /// MVVM パターンでファイルドロップ処理を実装するために使用する。
    /// </summary>
    public class FileDropBehavior : Behavior<ListView>
    {
        /// <summary>
        /// ファイルがドロップされた際に実行される ICommand を保持する依存関係プロパティ。
        ///
        /// ドロップされたファイルパス配列（string[]）が
        /// Command の Execute / CanExecute の引数として渡される。
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(FileDropBehavior));

        /// <summary>
        /// ファイルドロップ時に実行されるコマンド。
        ///
        /// ViewModel 側では ICommand の引数として
        /// string[]（ファイルパス配列）を受け取ることを想定する。
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Behavior が ListView にアタッチされたときに呼ばれる。
        ///
        /// ・AllowDrop を true に設定し、ドロップ操作を有効化
        /// ・Drop イベントを登録して、ファイルドロップを検知できるようにする
        /// </summary>
        protected override void OnAttached()
        {
            AssociatedObject.AllowDrop = true;
            AssociatedObject.Drop += OnDrop;
        }

        /// <summary>
        /// Behavior が ListView からデタッチされたときに呼ばれる。
        ///
        /// 登録した Drop イベントを解除し、
        /// メモリリークや不要なイベント呼び出しを防止する。
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.Drop -= OnDrop;
        }

        /// <summary>
        /// ファイルが ListView にドロップされたときに呼ばれるイベントハンドラ。
        ///
        /// ・ファイルドロップかどうかを判定
        /// ・ファイルパス配列を取得
        /// ・Command.CanExecute を確認した上で Command.Execute を呼び出す
        /// </summary>
        /// <param name="sender">イベントの送信元（ListView）</param>
        /// <param name="e">ドラッグ＆ドロップイベント情報</param>
        private void OnDrop(object sender, DragEventArgs e)
        {
            if (Command == null)
                return;

            // ドロップデータがファイルかどうかを確認
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // ファイルパス配列を取得
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // 実行可能な場合のみコマンドを実行
                if (Command.CanExecute(files))
                    Command.Execute(files);
            }
        }
    }
}
