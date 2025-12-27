using ezFFmpeg.Common;
using ezFFmpeg.Helpers;
using ezFFmpeg.Models.Codec;
using ezFFmpeg.Models.Common;
using ezFFmpeg.Models.DialogResults;
using ezFFmpeg.Models.Encoder;
using ezFFmpeg.Models.Interfaces;
using ezFFmpeg.Models.Media;
using ezFFmpeg.Models.Output;
using ezFFmpeg.Models.Presets;
using ezFFmpeg.Models.Quality;
using ezFFmpeg.Services.Interfaces;
using ezFFmpeg.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace ezFFmpeg.ViewModels
{
    /// <summary>
    /// パラメータダイアログ用 ViewModel。
    /// 
    /// ・プロファイルの追加／編集／削除
    /// ・変換実行用パラメータの入力・検証
    /// ・ダイアログ結果（ParameterDialogResult）の生成
    /// 
    /// View とは ICommand とバインディングのみで連携し、
    /// ダイアログの終了は RequestClose イベントで通知する。
    /// </summary>
    /// <summary>
    public class ParameterDialogViewModel : BindableBase , IProfile
    {
        /// <summary>
        /// ダイアログ終了要求イベント。
        /// View 側でこのイベントを購読し、
        /// true: 適用 / false: キャンセル としてダイアログを閉じる。
        /// </summary>
        public event Action<bool?>? RequestClose;

        #region プロパティ

        // ============================
        // 画面表示
        // ============================
        private string _windowTitle = string.Empty;
        /// <summary>ウィンドウのタイトル</summary>
        public string WindowTitle
        {
            get => _windowTitle;
            set => SetProperty(ref _windowTitle, value);
        }

        private string _tabItemTitle = string.Empty;
        /// <summary>タブアイテムのタイトル</summary>
        public string TabItemTitle
        {
            get => _tabItemTitle;
            set => SetProperty(ref _tabItemTitle, value);
        }

        // ============================
        // 実行モード
        // ============================
        private bool _isProfileEditMode;
        /// <summary>プロファイル編集モード</summary>
        public bool IsProfileEditMode
        {
            get => _isProfileEditMode;
            set => SetProperty(ref _isProfileEditMode, value);
        }

        private bool _isProfileRemoveMode;
        /// <summary>プロファイル削除モード</summary>
        public bool IsProfileRemoveMode
        {
            get => _isProfileRemoveMode;
            set => SetProperty(ref _isProfileRemoveMode, value);
        }

        private bool _isProfileReadOnly;
        /// <summary>プロファイル読取専用</summary>
        public bool IsProfileReadOnly
        {
            get => _isProfileReadOnly;
            set => SetProperty(ref _isProfileReadOnly, value);
        }

        private bool _isRunParameterMode;
        /// <summary>パラメータ実行モード</summary>
        public bool IsRunParameterMode
        {
            get => _isRunParameterMode;
            set => SetProperty(ref _isRunParameterMode, value);
        }

        #region Profile 

        // ============================
        // プロファイル関連
        // ============================
        // プロファイルID
        private Guid _profileId;
        /// <summary>プロファイルID</summary>
        public Guid ProfileId
        {
            get => _profileId;
            set
            {
                if (SetProperty(ref _profileId, value))
                {
                    UpdateViewFromProfile(value);
                }
            }
        }

        // プロファイル名
        private string _profileName = string.Empty;
        /// <summary>プロファイル名</summary>
        public string ProfileName
        {
            get => _profileName;
            set => SetProperty(ref _profileName, value);
        }

        // ============================
        // 出力関連
        // ============================
        public string _outputFormat = string.Empty;
        /// <summary>出力形式 (例: mp4, mkv)</summary>
        public string OutputFormat
        {
            get => _outputFormat;
            set
            {
                if (SetProperty(ref _outputFormat, value))
                {

                    var format = OutputFormats.GetOutputFormat(_outputFormat);

                    // ビデオ
                    ComboBoxHelper.SetVideoEncoders(VideoEncoderList,
                                                    out string defaultVideoEncoder,
                                                    VideoEncoders.All,
                                                    format,
                                                    _settings.UseGpu);
                    VideoEncoder = defaultVideoEncoder;

                    // オーディオ
                    ComboBoxHelper.SetAudioEncoders(AudioEncoderList,
                                                    out string defaultAudioEncoder,
                                                    AudioEncoders.All,
                                                    format);
                    AudioEncoder = defaultAudioEncoder;

                    UpdateMediaCheckBoxes();
                }
            }
        }

        private string _outputFolderPath = string.Empty;
        /// <summary>出力フォルダ</summary>
        public string OutputFolderPath
        {
            get => _outputFolderPath;
            set => SetProperty(ref _outputFolderPath, value);
        }

        private string _outputFileFormat = string.Empty;
        /// <summary>出力ファイル</summary>
        public string OutputFileFormat
        {
            get => _outputFileFormat;
            set => SetProperty(ref _outputFileFormat, value);
        }

        // 上書き
        private bool _isOutputOverwrite;
        /// <summary>出力ファイル上書き</summary>
        public bool IsOutputOverwrite
        {
            get => _isOutputOverwrite;
            set => SetProperty(ref _isOutputOverwrite, value);
        }

        // ============================
        // ビデオ関連
        // ============================
        private bool _isVideoEnabled;
        /// <summary>ビデオ有効化</summary>
        public bool IsVideoEnabled
        {
            get => _isVideoEnabled;
            set => SetProperty(ref _isVideoEnabled, value);
        }

        private string _videoEncoder = string.Empty;
        /// <summary>ビデオエンコーダ</summary>
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

        private string _videoResolution = string.Empty;
        public string VideoResolution
        {
            get => _videoResolution;
            set => SetProperty(ref _videoResolution, value);
        }

        private string _videoFrameRateMode = string.Empty;
        public string VideoFrameRateMode
        {
            get => _videoFrameRateMode;
            set => SetProperty(ref _videoFrameRateMode, value);
        }

        private string _videoFrameRate = string.Empty;
        public string VideoFrameRate
        {
            get => _videoFrameRate;
            set => SetProperty(ref _videoFrameRate, value);
        }

        // ============================
        // オーディオ関連
        // ============================
        private bool _isAudioEnabled;
        public bool IsAudioEnabled
        {
            get => _isAudioEnabled;
            set => SetProperty(ref _isAudioEnabled, value);
        }

        private string _audioEncoder = string.Empty;
        public string AudioEncoder
        {
            get => _audioEncoder;
            set => SetProperty(ref _audioEncoder, value);
        }

        private string _audioBitRate = string.Empty;
        public string AudioBitRate
        {
            get => _audioBitRate;
            set => SetProperty(ref _audioBitRate, value);
        }

        // ============================
        // 管理用フラグ
        // ============================
        /// <summary>ユーザー定義プロファイルかどうか</summary>
        public bool IsUserDefined { get; set; }

        /// <summary>デフォルトプロファイルかどうか</summary>
        public bool IsDefault { get; set; }

        /// <summary>最後に使用されたプロファイルかどうか</summary>
        public bool IsLastUsed { get; set; }

        #endregion

        // ============================
        // その他
        // ============================
        private string _insertTag = string.Empty;
        /// <summary>挿入用タグ</summary>
        public string InsertTag
        {
            get => _insertTag;
            set => SetProperty(ref _insertTag, value);
        }

        #endregion

        #region コンボボックス／リスト

        // プロファイルリスト
        public ObservableCollection<SelectionItem> ProfileList { get; } = [];

        // 出力形式リスト
        public ObservableCollection<SelectionItem> OutputFormatList { get; } = [];
        public ObservableCollection<SelectionItem> OutputFileTagList { get; } = [];

        // ビデオ設定リスト
        private ObservableCollection<SelectionItem> _videoEncoderList = [];
        public ObservableCollection<SelectionItem> VideoEncoderList  
        {
            get => _videoEncoderList;
            private set => SetProperty(ref _videoEncoderList, value);
        }

        public ObservableCollection<SelectionItem> VideoQualityTierList { get; } = [];
        public ObservableCollection<SelectionItem> VideoResolutionList { get; } = [];
        public ObservableCollection<SelectionItem> VideoFrameRateModeList { get; } = [];
        public ObservableCollection<SelectionItem> VideoFrameRateList { get; } = [];

        // オーディオ設定リスト
        //public ObservableCollection<SelectionItem> AudioEncoderList { get; } = [];
        private ObservableCollection<SelectionItem> _audioEncoderList = [];
        public ObservableCollection<SelectionItem> AudioEncoderList
        {
            get => _audioEncoderList;
            private set => SetProperty(ref _audioEncoderList, value);
        }


        public ObservableCollection<SelectionItem> AudioBitRateList { get; } = [];

        #endregion

        #region コマンド

        public ICommand BrowseOutputCommand { get; }
        public ICommand ApplyCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand InsertTagCommand { get; }

        #endregion

        private readonly AppSettings            _settings;
        private readonly IDialogService         _dialogService;
        private readonly ParameterDialogMode    _mode;

        /// <summary>
        /// ダイアログ結果格納用
        /// </summary>
        public readonly ParameterDialogResult   Result;

        /// <summary>
        /// コンストラクタ。
        /// 
        /// ・各種コンボボックス用データの初期化
        /// ・実行モードに応じた画面状態設定
        /// ・指定された Profile の内容を画面に反映
        /// ・コマンドおよび戻り値（Result）の初期化
        /// </summary>
        public ParameterDialogViewModel(AppSettings settings, IDialogService dialogService, ParameterDialogMode mode , Profile profile)
        {
            _settings           = settings;
            _dialogService      = dialogService;
            _mode               = mode;

            var format = OutputFormats.GetOutputFormat(_settings.CurrentProfile.OutputFormat);

            // コンボボックスのリスト
            ComboBoxHelper.SetFromProfiles(ProfileList, _settings.Profiles);

            // 出力
            ComboBoxHelper.SetFromCollection(OutputFormatList, OutputFormats.All, x => x.Extension, x => x.Name, x => x.MediaType);
            ComboBoxHelper.SetFromOutputFileTags(OutputFileTagList, OutputFileTags.All);

            // ビデオ
            ComboBoxHelper.SetVideoEncoders(VideoEncoderList,
                                            out string defaultEncoder,
                                            VideoEncoders.All,
                                            format,
                                            _settings.UseGpu);

            ComboBoxHelper.SetFromEnum<QualityTier>(VideoQualityTierList);
            ComboBoxHelper.SetFromCollection(VideoResolutionList, VideoResolutions.All, x => x.Resolution, x => x.Name);
            ComboBoxHelper.SetFromCollection(VideoFrameRateModeList, VideoFrameRateModes.All, x => x.FrameRateMode, x => x.Name);
            ComboBoxHelper.SetFromCollection(VideoFrameRateList, VideoFrameRates.All, x => x.FrameRate, x => x.Name);

            // オーディオ
            ComboBoxHelper.SetAudioEncoders(AudioEncoderList,
                                            out defaultEncoder,
                                            AudioEncoders.All,
                                            format);

            ComboBoxHelper.SetFromCollection(AudioBitRateList, AudioBitrates.All, x => x.BitRate, x => x.Name);

            // 初期値の設定
            SetModeDefaults(mode);

            // プロファイルの設定
            //CopyFrom(profile, true);
            ProfileMapper.CopyTo(profile, this, true);


            // コマンドの設定
            BrowseOutputCommand     = new RelayCommand(ShowFolderDialog);
            ApplyCommand            = new RelayCommand(Apply);
            CancelCommand           = new RelayCommand(Cancel);
            InsertTagCommand        = new RelayCommand<SelectionItem>(InsertTagItem);

            // 戻り値の初期化
            Result = new ParameterDialogResult();
            //Result.CopyFrom(profile, true);
            ProfileMapper.CopyTo(profile, Result, true);
        }

        /// <summary>
        /// ダイアログの起動モードに応じて、
        /// ウィンドウタイトル・表示状態・編集可否を初期化する。
        /// </summary>
        /// <param name="mode">ダイアログ起動モード</param>
        private void SetModeDefaults(ParameterDialogMode mode)
        {
            _insertTag = "";

            switch (mode)
            {
                case ParameterDialogMode.ProfileAdd:
                    _windowTitle = "プロファイル(登録)";
                    _tabItemTitle = "プロファイル";
                    _isProfileEditMode = true;
                    _isProfileRemoveMode = false;
                    _isRunParameterMode = false;
                    _isProfileReadOnly = false;
                    break;
                case ParameterDialogMode.ProfileEdit:
                    _windowTitle = "プロファイル(修正)";
                    _tabItemTitle = "プロファイル";
                    _isProfileEditMode = true;
                    _isProfileRemoveMode = false;
                    _isRunParameterMode = false;
                    _isProfileReadOnly = false;
                    break;
                case ParameterDialogMode.ProfileRemove:
                    _windowTitle = "プロファイル(削除)";
                    _tabItemTitle = "プロファイル";
                    _isProfileEditMode = true;
                    _isProfileRemoveMode = true;
                    _isRunParameterMode = false;
                    _isProfileReadOnly = false;
                    break;
                case ParameterDialogMode.RunParameter:
                    _windowTitle = "パラメータ";
                    _tabItemTitle = "パラメータ";
                    _isProfileEditMode = false;
                    _isProfileRemoveMode = false;
                    _isRunParameterMode = true;
                    _isProfileReadOnly = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        /// <summary>
        /// 選択されたプロファイルの値を画面に反映
        /// </summary>
        private void UpdateViewFromProfile(Guid profileId)
        {
            var profile = _settings.Profiles
                       .FirstOrDefault(p => p.ProfileId == profileId);

            if (profile == null)
                return;

            // ---- 画面に反映 ----
            //this.CopyFrom(profile, true);
            ProfileMapper.CopyTo(profile, this, true);
        }

        /// <summary>
        /// 出力形式に応じて動画・音声の有効チェックボックスを更新
        /// </summary>
        private void UpdateMediaCheckBoxes()
        {
            var selected = OutputFormatList.FirstOrDefault(f => (string)f.Key == OutputFormat);
            if (selected != null && selected.Tag is MediaType type)
            {
                if(type == MediaType.Video)
                {
                    IsVideoEnabled = true;
                    IsAudioEnabled = true;
                }
                else
                {
                    IsVideoEnabled = false;
                    IsAudioEnabled = true;
                }
            }
            else
            {
                // 不明な場合は両方OFF
                IsVideoEnabled = false;
                IsAudioEnabled = false;
            }
        }

        /// <summary>
        /// フォルダ選択ダイアログを表示し、選択結果を OutputFolderPath に設定
        /// </summary>
        private void ShowFolderDialog()
        {
            string? ret = _dialogService.ShowFolderDialog();

            if (ret != null)
            {
                OutputFolderPath = ret;
            }
        }

        /// <summary>
        /// 確認ダイアログを表示
        /// </summary>
        private MessageBoxResult ShowQuestion(string message)
        {
            return _dialogService.ShowMessageBox(
                message,
                "確認",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
        }

        /// <summary>
        /// 入力内容の妥当性を検証する。
        /// 
        /// ・必須項目の未入力チェック
        /// ・ビデオ／オーディオ有効時の整合性チェック
        /// 
        /// UI 表示用のエラーメッセージはここでは表示せず、
        /// ValidationResult として返却する。
        /// </summary>
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
                    if (VideoFrameRate != VideoFrameRates.Source.FrameRate) // ユーザーが指定した場合
                    {
                        return ValidationResult.Warning("Source / Passthrough の場合、フレームレート指定は無視されます。");
                    }
                }
                else if (VideoFrameRateMode == VideoFrameRateModes.CFR.FrameRateMode)
                {
                    if (VideoFrameRate == VideoFrameRates.Source.FrameRate)
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

            return ValidationResult.Success();
        }

        /// <summary>
        /// 入力内容の妥当性を検証します。
        /// </summary>
        /// <returns>
        /// 入力がすべて正しい場合は true、
        /// いずれかに問題がある場合は false を返します。
        /// </returns>
        private bool ValidateInputs()
        {
            var result = Validate();


            // 検証結果が無効（エラーあり）の場合
            if (!result.IsValid)
            {
                // エラーメッセージをダイアログで表示
                _dialogService.ShowMessageBox(
                    result.Message!,
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);

                // 入力が不正なため false を返す
                return false;
            }

            if (result.IsWarning)
            {
                // 警告メッセージ
                var warningResult = _dialogService.ShowMessageBox(
                    result.Message!,
                    "警告",
                    MessageBoxButton.OKCancel,  // OK: 続行、Cancel: 中止
                    MessageBoxImage.Warning);

                // Cancel を選択した場合は false を返す
                if (warningResult ==  MessageBoxResult.Cancel)
                    return false;
            }

            // すべての入力が正しい場合
            return true;
        }

        /// <summary>
        /// 適用ボタン押下時の処理。
        /// 
        /// ・モードに応じた入力検証または削除確認
        /// ・入力内容を Result に反映
        /// ・RequestClose イベントを発行してダイアログを閉じる
        /// </summary>
        private void Apply()
        {

            switch (_mode)
            {
                case ParameterDialogMode.ProfileAdd:
                case ParameterDialogMode.ProfileEdit:
                case ParameterDialogMode.RunParameter:
                    // 入力チェック
                    if (!ValidateInputs())
                        return;
                    break;
                case ParameterDialogMode.ProfileRemove:
                    // 入力チェックなし
                    if (ShowQuestion("削除してもよろしいですか？") == MessageBoxResult.No)
                        return;
                    break;
            }

            // ===== 結果反映 =====
            Result.IsAccepted = true;
            ProfileMapper.CopyTo(this, Result, false);

            if (_mode ==  ParameterDialogMode.RunParameter)
            {
                Result.IsLastUsed = true;
            }

            // ✅ ダイアログ終了要求
            RequestClose?.Invoke(true);
        }

        /// <summary>
        /// キャンセルボタン押下処理
        /// ダイアログを閉じる
        /// </summary>
        private void Cancel()
        {

            // ✅ ダイアログ終了要求
            RequestClose?.Invoke(false);
        }

        /// <summary>
        /// 出力ファイル名用のタグを挿入する。
        /// 選択されたタグを {TagName} 形式で InsertTag に設定する。
        /// </summary>
        private void InsertTagItem(SelectionItem? item)
        {
            if (item == null) return;
            InsertTag = $"{{{item.Key}}}";
        }
    }
}
