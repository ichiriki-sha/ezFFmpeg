using ezFFmpeg.Common;
using ezFFmpeg.Models.Encoder;
using ezFFmpeg.Models.Interfaces;
using ezFFmpeg.Models.Output;
using ezFFmpeg.Models.Presets;
using ezFFmpeg.Models.Quality;
using ezFFmpeg.Services;
using ezFFmpeg.Services.FFmpeg;
using ezFFmpeg.Services.Interfaces;
using ezFFmpeg.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Runtime;
using System.Text;

namespace ezFFmpeg.Models.Common
{
    /// <summary>
    /// 変換プロファイルを表すクラス
    /// </summary>
    public class Profile : BindableBase, IProfile
    {
        // ============================
        // 内部キー（不変・一意）
        // ============================
        private Guid _profileId;
        /// <summary>プロファイルの一意識別子</summary>
        public Guid ProfileId
        {
            get => _profileId;
            set => SetProperty(ref _profileId, value);
        }

        // ============================
        // 表示名
        // ============================
        private string _profileName;
        /// <summary>プロファイル名</summary>
        public string ProfileName
        {
            get => _profileName;
            set => SetProperty(ref _profileName, value);
        }

        // ============================
        // 出力設定
        // ============================
        private string _outputFormat;
        /// <summary>出力形式（例: mp4, mkv）</summary>
        public string OutputFormat
        {
            get => _outputFormat;
            set => SetProperty(ref _outputFormat, value);
        }

        private string _outputFolderPath;
        /// <summary>出力先フォルダ</summary>
        public string OutputFolderPath
        {
            get => _outputFolderPath;
            set => SetProperty(ref _outputFolderPath, value);
        }

        private string _outputFileFormat;
        /// <summary>出力ファイル名フォーマット</summary>
        public string OutputFileFormat
        {
            get => _outputFileFormat;
            set => SetProperty(ref _outputFileFormat, value);
        }

        private bool _isOutputOverwrite;
        /// <summary>既存ファイルの上書きフラグ</summary>
        public bool IsOutputOverwrite
        {
            get => _isOutputOverwrite;
            set => SetProperty(ref _isOutputOverwrite, value);
        }

        // ============================
        // ビデオ設定
        // ============================
        private bool _isVideoEnabled;
        /// <summary>ビデオ出力の有効/無効</summary>
        public bool IsVideoEnabled
        {
            get => _isVideoEnabled;
            set => SetProperty(ref _isVideoEnabled, value);
        }

        private string _videoEncoder;
        /// <summary>ビデオエンコーダー名</summary>
        public string VideoEncoder
        {
            get => _videoEncoder;
            set => SetProperty(ref _videoEncoder, value);
        }

        private QualityTier _videoQualityTier;
        /// <summary>ビデオ品質レベル</summary>
        public QualityTier VideoQualityTier
        {
            get => _videoQualityTier;
            set => SetProperty(ref _videoQualityTier, value);
        }

        private string _videoResolution;
        /// <summary>ビデオ解像度</summary>
        public string VideoResolution
        {
            get => _videoResolution;
            set => SetProperty(ref _videoResolution, value);
        }

        private string _videoFrameRateMode;
        /// <summary>フレームレート制御方式</summary>
        public string VideoFrameRateMode
        {
            get => _videoFrameRateMode;
            set => SetProperty(ref _videoFrameRateMode, value);
        }

        private string _videoFrameRate;
        /// <summary>フレームレート</summary>
        public string VideoFrameRate
        {
            get => _videoFrameRate;
            set => SetProperty(ref _videoFrameRate, value);
        }

        // ============================
        // オーディオ設定
        // ============================
        private bool _isAudioEnabled;
        /// <summary>オーディオ出力の有効/無効</summary>
        public bool IsAudioEnabled
        {
            get => _isAudioEnabled;
            set => SetProperty(ref _isAudioEnabled, value);
        }

        private string _audioEncoder;
        /// <summary>オーディオエンコーダー名</summary>
        public string AudioEncoder
        {
            get => _audioEncoder;
            set => SetProperty(ref _audioEncoder, value);
        }

        private string _audioBitRate;
        /// <summary>オーディオビットレート</summary>
        public string AudioBitRate
        {
            get => _audioBitRate;
            set => SetProperty(ref _audioBitRate, value);
        }

        // ============================
        // 管理用フラグ
        // ============================
        private bool _isUserDefined;
        /// <summary>ユーザー定義プロファイルかどうか</summary>
        public bool IsUserDefined
        {
            get => _isUserDefined;
            set => SetProperty(ref _isUserDefined, value);
        }

        private bool _isDefault;
        /// <summary>デフォルトプロファイルかどうか</summary>
        public bool IsDefault
        {
            get => _isDefault;
            set => SetProperty(ref _isDefault, value);
        }

        private bool _isLastUsed;
        /// <summary>最後に使用されたプロファイルかどうか</summary>
        public bool IsLastUsed
        {
            get => _isLastUsed;
            set => SetProperty(ref _isLastUsed, value);
        }

        // ============================
        // コンストラクタ
        // ============================
        /// <summary>
        /// デフォルトプロファイルを作成
        /// </summary>
        public Profile()
        {
            _profileId = Guid.NewGuid();
            _profileName = "既定(H.264 / MP4)";
            _outputFormat = OutputFormats.Mp4.Extension;
            _outputFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            _outputFileFormat = $"{{{OutputFileTags.FileName.Tag}}}_{{{OutputFileTags.VideoCodec.Tag}}}{{{OutputFileTags.Extension.Tag}}}";
            _isOutputOverwrite = true;

            _isVideoEnabled = true;
            _videoEncoder = VideoEncoders.LibX264.Encoder;
            _videoQualityTier = QualityTier.Medium;
            _videoResolution = VideoResolutions.Source.Resolution;
            _videoFrameRateMode = VideoFrameRateModes.Source.FrameRateMode;
            _videoFrameRate = VideoFrameRates.Source.FrameRate;

            _isAudioEnabled = true;
            _audioEncoder = AudioEncoders.Copy.Encoder;
            _audioBitRate = AudioBitrates.Source.BitRate;

            _isUserDefined = false;
            _isDefault = false;
            _isLastUsed = false;
        }

        /// <summary>
        /// GPU エンコーダを考慮したコンストラクタ
        /// </summary>
        /// <param name="encoder">使用するビデオエンコーダ</param>
        public Profile(String encoder)
            : this()
        {
            _videoEncoder = encoder;
        }

        /// <summary>
        /// プロファイルを複製
        /// </summary>
        /// <returns>複製された Profile</returns>
        public Profile Clone(bool includeId = true)
        {
            var clone = new Profile();
            clone.CopyFrom(this);
            if (includeId)
                clone.ProfileId = this.ProfileId;
            return clone;
        }

        /// <summary>
        /// 別のプロファイルから設定をコピーする
        /// </summary>
        /// <param name="source">コピー元プロファイル</param>
        /// <param name="copyId">ProfileIdのコピー可否フラグ</param>
        public void CopyFrom(Profile source, bool includeId = false)
        {
            //ArgumentNullException.ThrowIfNull(source);

            //if (copyId)
            //{
            //    ProfileId = source.ProfileId;
            //}
            //ProfileName = source.ProfileName;
            //OutputFormat = source.OutputFormat;
            //OutputFolderPath = source.OutputFolderPath;
            //OutputFileFormat = source.OutputFileFormat;
            //IsOutputOverwrite = source.IsOutputOverwrite;

            //IsVideoEnabled = source.IsVideoEnabled;
            //VideoEncoder = source.VideoEncoder;
            //VideoQualityTier = source.VideoQualityTier;
            //VideoResolution = source.VideoResolution;
            //VideoFrameRateMode = source.VideoFrameRateMode;
            //VideoFrameRate = source.VideoFrameRate;

            //IsAudioEnabled = source.IsAudioEnabled;
            //AudioEncoder = source.AudioEncoder;
            //AudioBitRate = source.AudioBitRate;

            //IsUserDefined = source.IsUserDefined;
            //IsDefault = source.IsDefault;
            //IsLastUsed = source.IsLastUsed;

            ProfileMapper.CopyTo(source, this, includeId);
        }

        public ValidationResult Validate()
        {
            // ===== 共通チェック =====

            if (string.IsNullOrWhiteSpace(OutputFormat))
            {
                return ValidationResult.Error("出力形式を選択してください。");
            }

            if (string.IsNullOrWhiteSpace(OutputFolderPath))
            {
                return ValidationResult.Error("出力フォルダを選択してください。");
            }

            if (string.IsNullOrWhiteSpace(OutputFileFormat))
            {
                return ValidationResult.Error("出力ファイル名を入力してください。");
            }

            // ===== ビデオ =====
            if (IsVideoEnabled)
            {
                if (string.IsNullOrWhiteSpace(VideoEncoder))
                {
                    return ValidationResult.Error("ビデオコーデックを選択してください。");
                }

                if (string.IsNullOrWhiteSpace(VideoResolution))
                {
                    return ValidationResult.Error("解像度を選択してください。");
                }

                if (string.IsNullOrWhiteSpace(VideoFrameRateMode))
                {
                    return ValidationResult.Error("フレームレート制御方式を選択してください。");
                }

                if (VideoFrameRateMode == VideoFrameRateModes.Source.FrameRateMode ||
                    VideoFrameRateMode == VideoFrameRateModes.Passthrough.FrameRateMode)
                {
                    // fps 指定は不要
                }
                else if (VideoFrameRateMode == VideoFrameRateModes.CFR.FrameRateMode)
                {
                    return ValidationResult.Error("CFR の場合はフレームレートを指定してください。");
                }
                else if (VideoFrameRateMode == VideoFrameRateModes.VFR.FrameRateMode)
                {
                }
                else
                {
                    return ValidationResult.Error("不明なフレームレート制御方式です。");
                }
            }

            // ===== オーディオ =====

            if (IsAudioEnabled)
            {
                if (string.IsNullOrWhiteSpace(AudioEncoder))
                {
                    return ValidationResult.Error("オーディオコーデックを選択してください。");
                }

                if (string.IsNullOrWhiteSpace(AudioBitRate))
                {
                    return ValidationResult.Error("オーディオビットレートを選択してください。");
                }
            }

            // 出力形式と動画を出力するの関連チェック
            //string outputFormatName = OutputFormats.GetName(OutputFormat);
            //MediaType mediaType = OutputFormats.GetMediaType(OutputFormat);

            //if (mediaType == MediaType.Video && !IsVideoEnabled)
            //{
            //    return ValidationResult.Error($"出力形式({outputFormatName})を出力には『ビデオを出力する』に\nチェックをしてください。");
            //}

            return ValidationResult.Success;
        }

    }
}
