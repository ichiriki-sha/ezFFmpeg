using System;

namespace ezFFmpeg.Models.Common
{
    /// <summary>
    /// 選択リスト用のアイテムを表すクラス
    /// ComboBox や ListBox などの選択コントロールに使用可能
    /// </summary>
    public class SelectionItem
    {
        /// <summary>
        /// アイテムのキー（内部識別用）
        /// </summary>
        public object Key { get; init; } = "";

        /// <summary>
        /// アイテムの表示名（UI に表示される文字列）
        /// </summary>
        public string Name { get; init; } = "";

        /// <summary>
        /// 任意の付加情報（タグ）
        /// </summary>
        public object? Tag { get; init; } = null;

        /// <summary>
        /// デバッグや UI バインド時に Name を文字列として返す
        /// </summary>
        /// <returns>表示名</returns>
        public override string ToString() => Name; // 保険として Name を返す
    }
}
