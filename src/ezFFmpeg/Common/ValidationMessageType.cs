namespace ezFFmpeg.Common
{
    /// <summary>
    /// バリデーション結果の種類を表す列挙型。
    /// </summary>
    public enum ValidationType
    {
        /// <summary>
        /// 成功（処理には影響なし）
        /// </summary>
        Success,

        /// <summary>
        /// 警告（処理は続行できるが注意が必要）
        /// </summary>
        Warning,

        /// <summary>
        /// エラー（処理を中断する必要がある）
        /// </summary>
        Error
    }
}
