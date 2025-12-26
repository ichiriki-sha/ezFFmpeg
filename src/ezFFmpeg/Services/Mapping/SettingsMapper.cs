using ezFFmpeg.Models.Interfaces;

namespace ezFFmpeg.Services.Mapping
{
    /// <summary>
    /// ISettings を実装するオブジェクト間で
    /// 共通設定値をコピーするマッパークラス。
    /// 
    /// ViewModel / Model の区別なく使用でき、
    /// 設定値同期処理を一箇所に集約する。
    /// </summary>
    public static class SettingsMapper
    {
        /// <summary>
        /// 設定値を source から target へコピーする。
        /// </summary>
        /// <param name="source">コピー元設定</param>
        /// <param name="target">コピー先設定</param>
        public static void CopyTo(ISettings source,ISettings target)
        {

            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(target);

            target.FFmpegFolderPath     = source.FFmpegFolderPath;
            target.IsFFmpegEnableLog    = source.IsFFmpegEnableLog;
            target.UseGpu               = source.UseGpu;
            target.ParallelCount        = source.ParallelCount;
            target.Profiles             = source.Profiles;
        }
    }
}
