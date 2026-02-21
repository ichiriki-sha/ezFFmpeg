using ezFFmpeg.Common;
using ezFFmpeg.Helpers;
using ezFFmpeg.Models.Common;
using ezFFmpeg.Models.DialogResults;
using ezFFmpeg.Models.Encoder;
using ezFFmpeg.Models.Profiles;
using ezFFmpeg.Services.FFmpeg;
using ezFFmpeg.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace ezFFmpeg.ViewModels
{
    /// <summary>
    /// オプションダイアログ用の ViewModel
    /// FFmpegフォルダパス、並列処理数、GPU使用設定、プロファイル管理などを提供する
    /// </summary>
    public class OptionDialogViewModel : BindableBase 
    {
        /// <summary>
        /// ダイアログ終了要求イベント
        /// true: 適用、false: キャンセル
        /// </summary>
        public event Action<bool?>? RequestClose;

        /// <summary>
        /// FFmpegフォルダパス
        /// </summary>
        private string? _ffmpegFolderPath = "";
        public string? FFmpegFolderPath
        {
            get => _ffmpegFolderPath;
            set => SetProperty(ref _ffmpegFolderPath, value);
        }

        /// <summary>
        /// FFmpegログ有効フラグ
        /// </summary>
        private bool _isFFmpegEnableLog;
        public bool IsFFmpegEnableLog
        {
            get => _isFFmpegEnableLog;
            set => SetProperty(ref _isFFmpegEnableLog, value);
        }

        /// <summary>
        /// 並列処理数
        /// </summary>
        private int _parallelCount;
        public int ParallelCount
        {
            get => _parallelCount;
            set {
                value = Math.Max(value, ParallelMin);
                value = Math.Min(value, ParallelMax);
                if(SetProperty(ref _parallelCount, value))
                    RaisePropertyChanged(nameof(ParallelCountText));
            }
        }

        /// <summary>
        /// 並列処理数
        /// </summary>
        public string ParallelCountText
        {
            get => _parallelCount.ToString();
            set
            {
                if (int.TryParse(value, out var n))
                {
                    ParallelCount = n;
                }
                // 失敗時は何もしない
            }
        }

        /// <summary>
        /// GPU使用フラグ
        /// </summary>
        private bool _useGpu;
        public bool UseGpu
        {
            get => _useGpu;
            set => SetProperty(ref _useGpu, value);
        }

        /// <summary>
        /// 並列処理最小値
        /// </summary>
        public static int ParallelMin => ConversionParallelLimits.Min;
        /// <summary>
        /// 並列処理最大値
        /// </summary>
        public static int ParallelMax => ConversionParallelLimits.Max;

        /// <summary>
        /// 並列数の範囲表示
        /// </summary>
        public static string ParallelRangeText => $"({ParallelMin}～{ParallelMax})";

        public ObservableCollection<Profile> ProfileItems => Profiles.Profiles;

        /// <summary>
        /// プロファイル一覧(UI用)
        /// </summary>
        public ProfileManager Profiles { get; set; } = [];

        /// <summary>
        /// 選択中のプロファイル
        /// 選択時にボタンの状態を更新
        /// </summary>
        private Profile? _selectedProfile;
        public Profile? SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                if (SetProperty(ref _selectedProfile, value))
                {
                    UpdateProfileActionButton();
                }
            }
        }

        // プロファイル操作ボタンの有効/無効フラグ
        private bool _isDefaultProfileEnabled;
        public bool IsDefaultProfileEnabled 
        { 
            get => _isDefaultProfileEnabled; 
            set => SetProperty(ref _isDefaultProfileEnabled, value); 
        }

        private bool _isAddProfileEnabled;
        public bool IsAddProfileEnabled 
        { 
            get => _isAddProfileEnabled; 
            set => SetProperty(ref _isAddProfileEnabled, value); 
        }

        private bool _isCopyProfileEnabled;
        public bool IsCopyProfileEnabled 
        { 
            get => _isCopyProfileEnabled; 
            set => SetProperty(ref _isCopyProfileEnabled, value); 
        }

        private bool _isEditProfileEnabled;
        public bool IsEditProfileEnabled 
        { 
            get => _isEditProfileEnabled; 
            set => SetProperty(ref _isEditProfileEnabled, value); 
        }

        private bool _isRemoveProfileEnabled;
        public bool IsRemoveProfileEnabled 
        { 
            get => _isRemoveProfileEnabled; 
            set => SetProperty(ref _isRemoveProfileEnabled, value); 
        }

        // ===== コマンド =====
        public ICommand ShownCommand { get; }

        public ICommand ParallelCountUpCommand { get; }
        public ICommand ParallelCountDownCommand { get; }

        public ICommand BrowseCommand { get; }

        public ICommand AddProfileCommand { get; }
        public ICommand CopyProfileCommand { get; }
        public ICommand EditProfileCommand { get; }
        public ICommand RemoveProfileCommand { get; }
        public ICommand DefaultProfileCommand { get; }

        public ICommand ApplyCommand { get; }
        public ICommand CancelCommand { get; }

        private readonly AppSettings _settings;
        private readonly IDialogService _dialogService;

        /// <summary>
        /// ダイアログ結果
        /// </summary>
        public readonly OptionDialogResult Result;

        /// <summary>
        /// コンストラクタ
        /// 設定とダイアログサービスを受け取り、コマンドとプロファイル一覧を初期化
        /// </summary>
        public OptionDialogViewModel(AppSettings settings, IDialogService dialogService)
        {
            _settings                   = settings;
            _dialogService              = dialogService;


            _isDefaultProfileEnabled    = false;
            _isAddProfileEnabled        = true;
            _isCopyProfileEnabled       = false;
            _isEditProfileEnabled       = false;
            _isRemoveProfileEnabled     = false;
            _selectedProfile            = null;

            FFmpegFolderPath            = _settings.FFmpegFolderPath;
            IsFFmpegEnableLog           = _settings.IsFFmpegEnableLog;
            ParallelCount               = _settings.ParallelCount;
            UseGpu                      = _settings.UseGpu;
            Profiles                    = [.. _settings.Profiles];
            SelectedProfile             = Profiles.FirstOrDefault();

            Result                      = new OptionDialogResult();

            ShownCommand                = new RelayCommand(OnShown);

            ParallelCountUpCommand      = new RelayCommand(() => ChangeParallelCount(1));
            ParallelCountDownCommand    = new RelayCommand(() => ChangeParallelCount(-1));

            BrowseCommand               = new RelayCommand(ShowFolderDialog);
            ApplyCommand                = new RelayCommand(Apply);
            CancelCommand               = new RelayCommand(Cancel);
            DefaultProfileCommand       = new RelayCommand(() => SetDefaultProfile(SelectedProfile!));

            // 追加用のプロファイルを準備
            Profile profile             = new(settings.UseGpu);
            profile.ProfileName         = "- 新規 -";
            profile.IsUserDefined       = true;
            AddProfileCommand           = new RelayCommand(
                                                () => ShowParameterDialog(ParameterDialogMode.ProfileAdd, profile));
            CopyProfileCommand　　　    = new RelayCommand(
                                                () => ShowParameterDialog(ParameterDialogMode.ProfileAdd, SelectedProfile!.Clone(false)));
            EditProfileCommand          = new RelayCommand(
                                                () => ShowParameterDialog(ParameterDialogMode.ProfileEdit, SelectedProfile!));
            RemoveProfileCommand        = new RelayCommand(
                                                () => ShowParameterDialog(ParameterDialogMode.ProfileRemove, SelectedProfile!));
        }

        /// <summary>
        /// ダイアログ表示時に呼ばれる処理
        /// FFmpegフォルダ未設定の場合は警告表示
        /// </summary>
        private void OnShown()
        {
            if (string.IsNullOrWhiteSpace(FFmpegFolderPath))
            {
                _dialogService.ShowMessageBox("FFmpegのインストールフォルダを入力してください。", "注意", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        /// <summary>
        /// 並列数変更処理
        /// direction: "Up"で減少、"Down"で増加
        /// </summary>
        private void ChangeParallelCount(int direction)
        {
            ParallelCount = ParallelCount + direction;
        }

        /// <summary>
        /// FFmpegフォルダ選択ダイアログを表示
        /// </summary>
        private void ShowFolderDialog()
        {
            string? ret = _dialogService.ShowFolderDialog();
            if (ret != null) FFmpegFolderPath = ret;
        }

        /// <summary>
        /// プロファイル追加/編集/削除用ダイアログ表示
        /// </summary>
        private void ShowParameterDialog(ParameterDialogMode mode, Profile profile)
        {
            var ret = _dialogService.ShowParameterDialog(_settings, mode, profile);

            if (!ret.IsAccepted) return;

            Debug.WriteLine(ReferenceEquals(
                        Profiles.Profiles,
                        this.Profiles));

            switch (mode)
            {
                case ParameterDialogMode.ProfileAdd:
                    Profiles.Add((Profile)ret);
                    break;
                case ParameterDialogMode.ProfileEdit:
                    Profiles.Update(profile.ProfileId, (Profile)ret);
                    break;
                case ParameterDialogMode.ProfileRemove:
                    Profiles.Remove(profile.ProfileId);
                    break;
            }
        }

        /// <summary>
        /// 選択プロファイルをデフォルトに設定
        /// </summary>
        private void SetDefaultProfile(Profile profile) => Profiles.SetDefault(profile.ProfileId);

        /// <summary>
        /// プロファイル操作ボタンの状態更新
        /// </summary>
        private void UpdateProfileActionButton()
        {
            if (SelectedProfile == null)
            {
                IsDefaultProfileEnabled = false;
                IsAddProfileEnabled     = true;
                IsCopyProfileEnabled    = false;
                IsEditProfileEnabled    = false;
                IsRemoveProfileEnabled  = false;
            }
            else
            {
                IsDefaultProfileEnabled = true;
                IsAddProfileEnabled     = true;
                IsCopyProfileEnabled    = true;
                IsEditProfileEnabled    = SelectedProfile.IsUserDefined;
                IsRemoveProfileEnabled  = SelectedProfile.IsUserDefined;
            }
        }

        public ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(FFmpegFolderPath))
                return ValidationResult.Error("FFmpegフォルダが未設定です。");

            if (!FFmpegPathService.IsFFmpegFolder(FFmpegFolderPath))
                return ValidationResult.Error("FFmpegフォルダではありません。");

            if (!ConversionParallelLimits.IsValid(ParallelCount))
                return ValidationResult.Error("並列数が不正です。");

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
            // AppSettingsBase に定義されている Validate() を呼び出して検証を行う
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

            // すべての入力が正しい場合
            return true;
        }

        /// <summary>
        /// 設定適用
        /// 入力チェック後、Resultに値を設定しダイアログ終了を通知
        /// </summary>
        private void Apply()
        {

            if (!ValidateInputs())
                return;

            Result.IsAccepted           = true;
            Result.FFmpegFolderPath     = FFmpegFolderPath;
            Result.IsFFmpegEnableLog    = IsFFmpegEnableLog;
            Result.ParallelCount        = ParallelCount;
            Result.UseGpu               = UseGpu;
            Result.Profiles             = [.. Profiles];

            // ダイアログ終了要求
            RequestClose?.Invoke(true);
        }

        /// <summary>
        /// キャンセル処理
        /// </summary>
        private void Cancel() => RequestClose?.Invoke(false);
    }
}
