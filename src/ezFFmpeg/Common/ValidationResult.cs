
namespace ezFFmpeg.Common
{
    /// <summary>
    /// 入力チェックの結果を表すクラス
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; private set; }
        public bool IsWarning { get; private set; }
        public string? Message { get; private set; }

        private ValidationResult(ValidationType validationType, string? message)
        {
            switch (validationType)
            {
                case ValidationType.Success:
                    IsValid = true;
                    IsWarning = false;
                    Message = message;
                    break;
                case ValidationType.Warning:
                    IsValid = true;
                    IsWarning = true;
                    Message = message;
                    break;
                case ValidationType.Error:
                    IsValid = false;
                    IsWarning = false;
                    Message = message;
                    break;
            }
        }

        /// <summary>
        /// 成功（エラーなし）
        /// </summary>
        public static ValidationResult Success() 
            => new(ValidationType.Success, null);

        /// <summary>
        /// 警告
        /// </summary>
        public static ValidationResult Warning(string message) 
            => new(ValidationType.Warning, message);

        /// <summary>
        /// エラー
        /// </summary>
        public static ValidationResult Error(string message) 
            => new(ValidationType.Error, message);
    }
}
