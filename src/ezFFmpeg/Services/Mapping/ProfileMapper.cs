using ezFFmpeg.Models.Interfaces;

namespace ezFFmpeg.Services.Mapping
{
    /// <summary>
    /// IProfile を実装するオブジェクト間で
    /// 共通設定値をコピーするマッパークラス。
    /// 
    /// ViewModel / Model の区別なく使用でき、
    /// 設定値同期処理を一箇所に集約する。
    /// </summary>
    internal static class ProfileMapper
    {

        /// <summary>
        /// 設定値を source から target へコピーする。
        /// </summary>
        /// <param name="source">コピー元設定</param>
        /// <param name="target">コピー先設定</param>
        /// <param name="includeId">
        /// true の場合は ProfileId もコピーする。
        /// </param>
        public static void CopyTo(
            IProfile source,
            IProfile target,
            bool includeId = false)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(target);

            if (includeId)
                target.ProfileId        = source.ProfileId;

            target.ProfileName          = source.ProfileName;
            target.OutputFormat         = source.OutputFormat;
            target.OutputFolderPath     = source.OutputFolderPath;
            target.OutputFileFormat     = source.OutputFileFormat;
            target.IsOutputOverwrite    = source.IsOutputOverwrite;

            target.IsVideoEnabled       = source.IsVideoEnabled;
            target.VideoEncoder         = source.VideoEncoder;
            target.VideoQualityTier     = source.VideoQualityTier;
            target.VideoResolution      = source.VideoResolution;
            target.VideoFrameRateMode   = source.VideoFrameRateMode;
            target.VideoFrameRate       = source.VideoFrameRate;

            target.IsAudioEnabled       = source.IsAudioEnabled;
            target.AudioEncoder         = source.AudioEncoder;
            target.AudioBitRate         = source.AudioBitRate;
            target.IsUserDefined        = source.IsUserDefined;
            target.IsDefault            = source.IsDefault;
            target.IsLastUsed           = source.IsLastUsed;
        }
    }
}
