using System;
using System.Windows.Input;

namespace ezFFmpeg.Helpers
{
    /// <summary>
    /// パラメータなしの ICommand 実装。
    /// WPF のボタンやメニューのコマンドにバインド可能。
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="execute">コマンド実行時の処理</param>
        /// <param name="canExecute">コマンド実行可否判定関数（省略可）</param>
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// コマンドが実行可能か判定
        /// </summary>
        /// <param name="parameter">パラメータ（使用しない）</param>
        /// <returns>実行可能なら true</returns>
        public bool CanExecute(object? parameter)
            => _canExecute?.Invoke() ?? true;

        /// <summary>
        /// コマンド実行
        /// </summary>
        /// <param name="parameter">パラメータ（使用しない）</param>
        public void Execute(object? parameter)
            => _execute();

        /// <summary>
        /// CanExecute の変更を通知するイベント
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// CanExecuteChanged イベントを手動で発火
        /// </summary>
        public void RaiseCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 型パラメータ付きの ICommand 実装。
    /// コマンドに引数を渡して実行可能。
    /// </summary>
    /// <typeparam name="T">コマンド引数の型</typeparam>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T>? _canExecute;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="execute">コマンド実行時の処理</param>
        /// <param name="canExecute">コマンド実行可否判定関数（省略可）</param>
        public RelayCommand(Action<T> execute, Predicate<T>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// コマンドが実行可能か判定
        /// </summary>
        /// <param name="parameter">コマンド引数</param>
        /// <returns>実行可能なら true</returns>
        public bool CanExecute(object? parameter)
        {
            if (_canExecute == null)
                return true;

            if (parameter is T value)
                return _canExecute(value);

            return false;
        }

        /// <summary>
        /// コマンド実行
        /// </summary>
        /// <param name="parameter">コマンド引数</param>
        public void Execute(object? parameter)
        {
            if (parameter is T value)
            {
                _execute(value);
            }
        }

        /// <summary>
        /// CanExecute の変更を通知するイベント
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// CanExecuteChanged イベントを手動で発火
        /// </summary>
        public void RaiseCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
