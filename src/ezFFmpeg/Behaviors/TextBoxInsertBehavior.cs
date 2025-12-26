using System.Windows;
using System.Windows.Controls;

namespace ezFFmpeg.Behaviors
{
    /// <summary>
    /// TextBox に対して、指定した文字列を
    /// 「現在のキャレット位置」または「選択範囲」に挿入するための
    /// 添付ビヘイビア。
    /// 
    /// 主に以下の用途を想定：
    /// ・ボタン押下で定型文字列を挿入
    /// ・ショートカットや UI 操作から TextBox を編集
    /// 
    /// MVVM パターンでコードビハインドを使わずに
    /// TextBox の文字挿入を行うための仕組み。
    /// </summary>
    public static class TextBoxInsertBehavior
    {
        /// <summary>
        /// 挿入する文字列を取得する。
        /// </summary>
        public static string GetInsertText(DependencyObject obj)
            => (string)obj.GetValue(InsertTextProperty);

        /// <summary>
        /// 挿入する文字列を設定する。
        /// この値が変更されたタイミングで、
        /// TextBox への文字挿入処理が実行される。
        /// </summary>
        public static void SetInsertText(DependencyObject obj, string value)
            => obj.SetValue(InsertTextProperty, value);

        /// <summary>
        /// TextBox に挿入する文字列を表す添付 DependencyProperty。
        /// 
        /// 値が更新されると OnInsertTextChanged が呼び出され、
        /// 文字挿入処理が行われる。
        /// </summary>
        public static readonly DependencyProperty InsertTextProperty =
            DependencyProperty.RegisterAttached(
                "InsertText",
                typeof(string),
                typeof(TextBoxInsertBehavior),
                new PropertyMetadata(null, OnInsertTextChanged));

        /// <summary>
        /// InsertText プロパティが変更された際に呼ばれるコールバック。
        /// 
        /// ・選択範囲がある場合：選択文字列を置き換える
        /// ・選択範囲がない場合：キャレット位置に文字を挿入する
        /// 
        /// 挿入後はキャレットを挿入文字列の末尾へ移動し、
        /// TextBox にフォーカスを戻す。
        /// </summary>
        private static void OnInsertTextChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            // TextBox 以外、または文字列以外の場合は処理しない
            if (d is not TextBox tb || e.NewValue is not string text)
                return;

            int newCaretIndex;

            // 選択範囲がある場合は、選択部分を置き換える
            if (tb.SelectionLength > 0)
            {
                int start = tb.SelectionStart;

                tb.Text = tb.Text
                    .Remove(start, tb.SelectionLength)
                    .Insert(start, text);

                newCaretIndex = start + text.Length;
            }
            else
            {
                // キャレット位置に文字列を挿入
                int caret = tb.CaretIndex;

                tb.Text = tb.Text.Insert(caret, text);
                newCaretIndex = caret + text.Length;
            }

            // Text 更新直後は CaretIndex を設定できない場合があるため、
            // Dispatcher 経由で非同期に設定する
            tb.Dispatcher.InvokeAsync(() =>
            {
                tb.CaretIndex = newCaretIndex;
            });

            // フォーカスを TextBox に戻す
            tb.Focus();
        }
    }
}
