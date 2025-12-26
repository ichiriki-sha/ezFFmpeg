
namespace ezFFmpeg.Common
{
    /// <summary>
    /// バリデーション結果を表す軽量な結果クラス
    /// </summary>
    public sealed record ValidationResult(bool IsValid, string? Message)
    {
        /// <summary>
        /// バリデーション成功
        /// </summary>
        public static ValidationResult Success
            => new(true, null);

        /// <summary>
        /// バリデーション失敗
        /// </summary>
        public static ValidationResult Error(string message)
            => new(false, message);
    }
}
