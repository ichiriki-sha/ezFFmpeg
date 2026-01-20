using ezFFmpeg.Common;
using ezFFmpeg.Helpers;
using ezFFmpeg.Models.Common;
using ezFFmpeg.Models.Conversion;
using ezFFmpeg.Models.Encoder;
using ezFFmpeg.Models.Output;
using ezFFmpeg.Models.Profiles;
using ezFFmpeg.Services.Conversion;
using ezFFmpeg.Services.FFmpeg;
using ezFFmpeg.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace ezFFmpeg.ViewModels
{
    /// <summary>
    /// メインウィンドウ用 ViewModel。
    /// 
    /// ・ファイル一覧管理（追加 / 削除 / 選択 / チェック）  
    /// ・FFmpeg 変換処理の開始 / 停止  
    /// ・進捗・経過時間・ステータスメッセージ管理  
    /// ・プレビュー制御（PreviewViewModel との連携）  
    /// ・UI 表示状態（ツールバー / ログ / プレビュー / ステータスバー）の保持  
    /// 
    /// アプリケーション全体の状態とユーザー操作を統括する中核 ViewModel。
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        // ------------------------
        // バインド用プロパティ
        // ------------------------
        #region プロパティ

        /// <summary>
        /// 全体の変換進捗率（0～100）。
        /// ConversionService から通知される値をそのまま反映する。
        /// </summary>
        private double _totalProgress;
        public double TotalProgress
        {
            get => _totalProgress;
            set => SetProperty(ref _totalProgress, value);
        }

        /// <summary>
        /// 現在エンコード処理中かどうか。
        /// true の間は UI 操作を制限する。
        /// </summary>
        private bool _isProcessing;
        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }

        /// <summary>
        /// ファイル追加可否
        /// </summary>
        private bool _canAddItem = true;
        public bool CanAddItem
        {
            get => _canAddItem;
            set => SetProperty(ref _canAddItem, value);
        }

        /// <summary>
        /// ファイル削除可否
        /// </summary>
        private bool _canRemoveItem = false;
        public bool CanRemoveItem
        {
            get => _canRemoveItem;
            set => SetProperty(ref _canRemoveItem, value);
        }

        /// <summary>
        /// 全削除可否
        /// </summary>
        private bool _canClearItem = false;
        public bool CanClearItem
        {
            get => _canClearItem;
            set => SetProperty(ref _canClearItem, value);
        }

        /// <summary>
        /// 変換開始可否
        /// </summary>
        private bool _canStart = false;
        public bool CanStart
        {
            get => _canStart;
            set => SetProperty(ref _canStart, value);
        }

        /// <summary>
        /// 停止可否
        /// </summary>
        private bool _canStop = false;
        public bool CanStop
        {
            get => _canStop;
            set => SetProperty(ref _canStop, value);
        }

        /// <summary>
        /// 設定画面表示可否
        /// </summary>
        private bool _canSetting = true;
        public bool CanSetting
        {
            get => _canSetting;
            set => SetProperty(ref _canSetting, value);
        }

        /// <summary>
        /// UI を読み取り専用にするか
        /// </summary>
        private bool _isReadOnly = false;
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => SetProperty(ref _isReadOnly, value);
        }

        /// <summary>
        /// ステータスバー表示メッセージ
        /// </summary>
        private string _statusMessage = "アイテムを追加してください。";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        /// <summary>
        /// 経過時間表示文字列
        /// </summary>
        private string _elapsedTime = "--:--:--";
        public string ElapsedTime
        {
            get => _elapsedTime;
            set => SetProperty(ref _elapsedTime, value);
        }

        /// <summary>
        /// 現在選択中のファイル。
        /// 変更時に FileList と Preview の状態も同期される。
        /// </summary>
        private FileItem? _selectedFileItem;
        public FileItem? SelectedFileItem
        {
            get => _selectedFileItem;
            set {
                if (SetProperty(ref _selectedFileItem, value))
                {
                    FileList.SelectedItem = value;
                    Preview.SetSource(value);
                }
            }
        }

        private bool _isToolbarVisible = false;
        public bool IsToolbarVisible
        {
            get => _isToolbarVisible;
            set => SetProperty(ref _isToolbarVisible, value);
        }

        private GridLength _fileListRowHeight = new(280); 
        public GridLength FileListRowHeight
        {
            get => _fileListRowHeight;
            set => SetProperty(ref _fileListRowHeight, value);
        }

        private GridLength _logRowHeight;
        public GridLength LogRowHeight
        {
            get => _logRowHeight;
            set => SetProperty(ref _logRowHeight, value);
        }

        private bool _isLogVisible = false;
        private GridLength _lastFileListRowHeigh ;

        public bool IsLogVisible
        {
            get => _isLogVisible;
            set
            {
                if (SetProperty(ref _isLogVisible, value))
                {
                    if (value)
                    {
                        // ON
                        LogRowHeight = new GridLength(1, GridUnitType.Star);
                        FileListRowHeight = _lastFileListRowHeigh;
                    }
                    else
                    {
                        // OFF
                        _lastFileListRowHeigh = FileListRowHeight;
                        LogRowHeight = new GridLength(0);
                        FileListRowHeight = new GridLength(1, GridUnitType.Star);
                    }
                }
            }
        }

        private GridLength _previewColumnWidth;
        public GridLength PreviewColumnWidth
        {
            get => _previewColumnWidth;
            set => SetProperty(ref _previewColumnWidth, value);
        }

        private bool _isPreviewVisible;
        private GridLength _lastPreviewWidth;
        public bool IsPreviewVisible
        {
            get => _isPreviewVisible;
            set
            {
                if (SetProperty(ref _isPreviewVisible, value))
                {
                    if (value)
                    {
                        // ON
                        PreviewColumnWidth = _lastPreviewWidth;
                    }
                    else
                    {
                        // OFF
                        _lastPreviewWidth = PreviewColumnWidth;
                        PreviewColumnWidth = new GridLength(0);
                    }
                }
            }
        }

        private bool _isStatusbarVisible = false;
        public bool IsStatusbarVisible
        {
            get => _isStatusbarVisible;
            set => SetProperty(ref _isStatusbarVisible, value);
        }

        public FileListViewModel FileList { get; }

        /// <summary>
        /// ファイルログ一覧
        /// </summary>
        public ObservableCollection<FileItem> ActiveLogItems { get; } = [];

        /// <summary>
        /// 選択中のファイルログアイテム
        /// </summary>
        private FileItem? _selectedLogItem;
        public FileItem? SelectedLogItem
        {
            get => _selectedLogItem;
            set => SetProperty(ref _selectedLogItem, value);
        }

        private string _videoEncoderStatus = "";
        public string VideoEncoderStatus
        {
            get => _videoEncoderStatus;
            set => SetProperty(ref _videoEncoderStatus, value);
        }

        private string _audioEncoderStatus = "";
        public string AudioEncoderStatus
        {
            get => _audioEncoderStatus;
            set => SetProperty(ref _audioEncoderStatus, value);
        }

        #endregion

        // ------------------------
        // 内部制御
        // ------------------------

        /// <summary>
        /// プレビュー表示専用 ViewModel。
        /// MediaElement 制御は Behavior 経由で行う。
        /// </summary>
        public PreviewViewModel Preview { get; }

        /// <summary>
        /// 経過時間表示更新用タイマー。
        /// 変換開始時に起動し、終了時に停止する。
        /// </summary>
        private DispatcherTimer? _elapsedTimer;

        /// <summary>
        /// 変換キャンセル用トークン。
        /// 停止ボタンや終了確認からキャンセル要求を伝える。
        /// </summary>
        private CancellationTokenSource? _cts;

        #region コマンド

        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand ClearItemCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand SettingCommand { get; }

        // FileListView
        public ICommand FileListViewSelectionChangedCommand { get; }
        public ICommand FileListViewMouseLeftButtonDownCommand { get; }
        public ICommand FileListViewDropFilesCommand { get; }

        // StartPotion 
        public ICommand FileItemStartPositionUpCommand { get; }
        public ICommand FileItemStartPositionDownCommand { get; }
        public ICommand FileItemEndPositionUpCommand { get; }
        public ICommand FileItemEndPositionDownCommand { get; }

        // FileListViewItem
        public ICommand FileItemCheckedCommand { get; }
        public ICommand FileItemUncheckedCommand { get; }

        public ICommand AboutCommand { get; }
        public ICommand ExitCommand { get; }

        public ICommand PreviewTogglePlayPauseCommand { get; }

        // View
        public ICommand ToggleToolbarCommand { get; }
        public ICommand ToggleLogCommand { get; }
        public ICommand TogglePreviewCommand { get; }
        public ICommand ToggleStatusbarCommand { get; }

        #endregion

        private readonly AppSettings        _settings;
        private readonly IDialogService     _dialogService;
        private readonly ConversionService  _conversionService;

        /// <summary>
        /// MainWindowViewModel のコンストラクタ。
        /// 
        /// ・設定情報
        /// ・ダイアログサービス
        /// ・変換サービス
        /// を受け取り、各 ViewModel・Command を初期化する。
        /// </summary>
        public MainWindowViewModel(AppSettings settings, IDialogService dialogService)
        {
            _settings           = settings;
            _dialogService      = dialogService;
            _conversionService  = new ConversionService();

            // PreviewViewModel 初期化
            Preview                                 = new PreviewViewModel();

            // FileListViewModel 初期化
            FileList                                = new FileListViewModel();

            // Menu & ToolBox
            AddItemCommand                          = new RelayCommand(AddItemFromFileDialog);
            RemoveItemCommand                       = new RelayCommand(RemoveItem);
            ClearItemCommand                        = new RelayCommand(ClearItem);
            StartCommand                            = new RelayCommand(StartConversion);
            StopCommand                             = new RelayCommand(CancelConversion);
            SettingCommand                          = new RelayCommand(ShowOptionDialog); 

            // View
            ToggleToolbarCommand                    = new RelayCommand(ToggleVisibility);
            ToggleLogCommand                        = new RelayCommand(ToggleVisibility);
            TogglePreviewCommand                    = new RelayCommand(ToggleVisibility);
            ToggleStatusbarCommand                  = new RelayCommand(ToggleVisibility);

            // ListView
            FileListViewSelectionChangedCommand     = new RelayCommand(UpdateCanRemoveItem);
            FileListViewMouseLeftButtonDownCommand  = new RelayCommand(UpdateCanRemoveItem);
            FileListViewDropFilesCommand            = new RelayCommand<string[]>(OnFilesDropped);

            // ListViewItem
            FileItemCheckedCommand                  = new RelayCommand<FileItem>(OnFileItemChecked);
            FileItemUncheckedCommand                = new RelayCommand<FileItem>(OnFileItemUnchecked);

            FileItemStartPositionUpCommand          = new RelayCommand(() => ChangeFileItemStartPosition(1));
            FileItemStartPositionDownCommand        = new RelayCommand(() => ChangeFileItemStartPosition(-1));

            FileItemEndPositionUpCommand            = new RelayCommand(() => ChangeFileItemEndPosition(1));
            FileItemEndPositionDownCommand          = new RelayCommand(() => ChangeFileItemEndPosition(-1));

            // Help & Exit
            AboutCommand                            = new RelayCommand(ShowAbout);
            ExitCommand                             = new RelayCommand(Exit);

            // Preview
            PreviewTogglePlayPauseCommand           = new RelayCommand(() => Preview.TogglePlayPause());

            // View
            IsToolbarVisible                        = _settings.ToolbarVisible;
            IsLogVisible                            = _settings.LogVisible;
            IsPreviewVisible                        = _settings.PreviewVisible;
            IsStatusbarVisible                      = _settings.StatusbarVisible;

            FileListRowHeight                       = _settings.FileListRowHeight;
            LogRowHeight                            = _settings.LogRowHeight;
            PreviewColumnWidth                      = _settings.PreviewColumnWidth;

            // エンコーダの状態を表示
            UpdateEncoderStatus();

            // ボタン状態初期化
            UpdateToolboxButtonsState();
        }


        /// <summary>
        /// 現在のプロファイル設定をもとに、
        /// ビデオ／オーディオエンコーダの状態表示文字列を更新する。
        /// 
        /// ・有効／無効
        /// ・Copy（ストリームコピー）か再エンコードか
        /// ・選択されているエンコーダ名
        /// 
        /// などを判定し、UI 表示用のステータス文字列を構築する。
        /// </summary>
        private void UpdateEncoderStatus()
        {
            // ビデオエンコーダの状態更新
            VideoEncoderStatus = _settings.CurrentProfile!.BuildVideoEncoderStstus();
            // オーディオエンコーダの状態更新
            AudioEncoderStatus = _settings.CurrentProfile!.BuildAudioEncoderStatus();
        }

        /// <summary>
        /// ツールバーおよびメニューのボタン状態を更新する。
        /// 処理中かどうか、選択・チェック状態に応じて有効 / 無効を切り替える。
        /// </summary>
        private void UpdateToolboxButtonsState()
        {
            if (IsProcessing)
            {
                CanAddItem = false;
                CanRemoveItem = false;
                CanClearItem = false;
                CanStart = false;
                CanStop = true;
                CanSetting = false;
            }
            else 
            {
                
                CanAddItem = true;
                CanRemoveItem = FileList.GetSelectedItems().Count > 0;
                CanClearItem = FileList.Items.Count > 0;
                CanStart = FileList.GetCheckedItems().Count > 0;
                CanStop = false;
                CanSetting = true;
            }
        }

        /// <summary>
        /// 経過時間表示用タイマーを開始する。
        /// 1 秒ごとにステータスバーの時間表示を更新する。
        /// </summary>
        private void StartElapsedTimer()
        {
            _settings.ProcessStartTime = DateTime.Now;

            _elapsedTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _elapsedTimer.Tick += (s, e) =>
            {
                var elapsed = DateTime.Now - _settings.ProcessStartTime;
                StatusMessage = $"実行中...";
                ElapsedTime = $"{elapsed:hh\\:mm\\:ss}";
            };

            _elapsedTimer.Start();
        }

        /// <summary>
        /// 経過時間表示用タイマーを停止する。
        /// </summary>
        private void StopElapsedTimer()
        {
            _elapsedTimer?.Stop();
            _elapsedTimer = null;
        }


        private void UpdateResultStatus()
        {
            var elapsed = DateTime.Now - _settings.ProcessStartTime;
            var elapsedFormatted = $"{elapsed:hh\\:mm\\:ss}";

            ElapsedTime = elapsedFormatted;

            if (_cts?.IsCancellationRequested == true)
            {
                StatusMessage = "処理はキャンセルされました。";
            }
            else
            {
                StatusMessage = "処理が完了しました。";
            }
        }

        /// <summary> 
        /// 変換処理の実行本体。 
        /// 
        /// ・進捗更新 
        /// ・ログ収集 
        /// ・キャンセル対応 
        /// ・完了／中断時の UI 復帰 
        /// を一括して管理する。 
        /// </summary>
        private async void StartProcess()
        {
            var selectedFiles = FileList.GetCheckedItems();

            TotalProgress = 0;
            IsProcessing = true;
            IsReadOnly = true;
            UpdateToolboxButtonsState();

            // アイテム初期化
            foreach (var item in selectedFiles)
            {
                item.Progress = 0;
                item.Status = ProcessingStatus.Pending ;
                item.ProcessingLogLines.Clear();
            }

            StartElapsedTimer();
            StatusMessage = "実行中...";

            _cts = new CancellationTokenSource();

            var context = new ConversionContext
            {
                Files = selectedFiles,
                Settings = _settings,
                Token = _cts.Token
            };

            var progress = new Progress<ConversionProgress>(p =>
            {
                TotalProgress = p.TotalPercent;
            });

            try
            {
                await _conversionService.ExecuteAsync(context, progress);

                // 実行ログをまとめる
                if (_settings.IsFFmpegEnableLog)
                {
                    var logFileName = $"{AppInfo.AppName}_{_settings.ProcessStartTime:yyyyMMdd_HHmmssfff}.log";
                    var logFilePath = Path.Combine(_settings.CurrentProfile!.OutputFolderPath, logFileName);
                    using var sw = new StreamWriter(logFilePath, false, Encoding.UTF8);

                    foreach (var item in selectedFiles)
                    {
                        try
                        {
                            sw.WriteLine($"===== {item.FilePath} =====");
                            foreach (var line in item.ProcessingLogLines)
                            {
                                sw.WriteLine(line);
                            }
                        }
                        catch { }
                    }
                }
            }
            finally
            {
                StopElapsedTimer();
                IsProcessing = false;
                IsReadOnly = false;
                UpdateToolboxButtonsState();
            }

            UpdateResultStatus();
        }

        /// <summary>
        /// FFmpeg フォルダ設定が正しいかを確認する。
        /// </summary>
        private bool IsConversionReady()
        {
            return FFmpegPathService.IsFFmpegFolder(_settings.FFmpegFolderPath);
        }

        /// <summary>
        /// オプションダイアログを表示する。
        /// FFmpeg フォルダが変更された場合は
        /// エンコーダ情報を再初期化する。
        /// </summary>
        private void ShowOptionDialog()
        {
            string? currntFFmpegFolderPath = _settings.FFmpegFolderPath;
            var ret = _dialogService.ShowOptionDialog(_settings);
            if (ret.IsAccepted)
            {
                _settings.CopyFrom(ret);

                if (currntFFmpegFolderPath != null && !currntFFmpegFolderPath.Equals(_settings.FFmpegFolderPath, StringComparison.OrdinalIgnoreCase))
                {
                    // FFmpegフォルダが変更された場合、エンコーダー情報を初期化
                    VideoEncoders.Initialize(_settings.FFmpegService!);
                    AudioEncoders.Initialize(_settings.FFmpegService!);
                }

                AppSettingsManager.SaveSettings(_settings);

                // エンコーダの状態を表示
                UpdateEncoderStatus();
            }
        }

        /// <summary>
        /// 実行パラメータ（プロファイル）選択ダイアログを表示する。
        /// 前回使用プロファイルの管理も行う。
        /// </summary>
        private bool ShowParameterDialog()
        {
            var profiles = new ProfileManager(_settings.Profiles);
            var profile = profiles.GetDefault();
            var ret = _dialogService.ShowParameterDialog(_settings, ParameterDialogMode.RunParameter, profile!);
            if (ret.IsAccepted)
            {

                Profile? lastUsed = profiles.GetLastUsed();

                if (lastUsed != null)
                {
                    // 前回使用プロファイルあり
                    bool isDefault = lastUsed.IsDefault;

                    lastUsed.CopyFrom((Profile)ret);

                    lastUsed.ProfileName = $"前回使用({lastUsed.BuildProfileName()})";
                    lastUsed.IsDefault = isDefault;
                    lastUsed.IsUserDefined = false;
                    lastUsed.IsLastUsed = true;
                }
                else
                {
                    // 前回使用プロファイルなし
                    lastUsed = new Profile ();
                    lastUsed.CopyFrom((Profile)ret);

                    lastUsed.ProfileName = $"前回使用({lastUsed.BuildProfileName()})";
                    lastUsed.IsDefault = false ;
                    lastUsed.IsUserDefined = false;
                    lastUsed.IsLastUsed = true;
                    profiles.Add(lastUsed);
                }

                _settings.CurrentProfile = lastUsed;
                _settings.Profiles = [.. profiles];

                AppSettingsManager.SaveSettings(_settings);

                // エンコーダの状態を表示
                UpdateEncoderStatus();

                return true;
            }
            return false;
        }

        /// <summary>
        /// ファイルダイアログからファイルを追加する。
        /// </summary>
        private void AddItemFromFileDialog()
        {
            var files = _dialogService.ShowFileDialog();
            if (files.Count > 0)  { AddItem(files); }
        }

        /// <summary>
        /// 指定されたファイルを一覧に追加する。
        /// FFmpeg を用いてメディア情報を非同期で取得する。
        /// </summary>
        private async void AddItem(IEnumerable<string> files)
        {
            if (IsProcessing) return;

            StatusMessage = "アイテム追加中...";

            int count = await FileList.AddAsync(files, _settings.FFmpegService!);

            UpdateToolboxButtonsState();

            if (count == 0)
            {
                StatusMessage = "追加できるアイテムがありませんでした。";
            }
            else
            {
                StatusMessage = $"アイテムを {count}個追加しました。";
            }
        }

        /// <summary>
        /// 選択中のファイルを削除する。
        /// </summary>
        private void RemoveItem()
        {
            var count = FileList.RemoveSelected();
            UpdateToolboxButtonsState();

            if (count > 0)
                StatusMessage = $"アイテムを {count}個削除しました。";

            Preview.MediaSource = null;
        }

        /// <summary>
        /// ファイル一覧をすべてクリアする。
        /// </summary>
        private void ClearItem()
        {
            FileList.Clear();
            TotalProgress = 0;

            UpdateToolboxButtonsState();
            StatusMessage = "アイテムをクリアしました。";

            Preview.MediaSource = null;
        }

        /// <summary>
        /// 変換処理開始要求を受け付ける。
        /// 
        /// ・FFmpeg フォルダ設定の妥当性確認
        /// ・実行プロファイル選択ダイアログ表示
        /// ・問題なければ変換処理を開始
        /// </summary>
        private void StartConversion()
        {
            if (!IsConversionReady())
            {
                _dialogService.ShowMessageBox("FFmpegフォルダを設定してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);

                ShowOptionDialog();
                if (!IsConversionReady()) return;
            }

            if (ShowParameterDialog())
                StartProcess();
        }

        /// <summary>
        /// 実行中の変換処理をキャンセルする。
        /// CancellationTokenSource にキャンセル要求を通知するのみで、
        /// 実際の停止処理は ConversionService 側で行われる。
        /// </summary>
        private void CancelConversion()
        {
            _cts?.Cancel();
        }

        /// <summary>
        /// ファイル選択状態変更時に、
        /// 削除・開始などのボタン有効状態を更新する。
        /// </summary>
        private void UpdateCanRemoveItem()
        {
            if (IsProcessing) return;

            // ボタン状態
            UpdateToolboxButtonsState();
        }


        private static TimeSpan CalculateDelta(int caretIndex, int duration)
        {
            return caretIndex switch
            {
                0 or 1 => TimeSpan.FromHours(10 * duration),        // 時間10の位
                2 or 3 => TimeSpan.FromHours(1 * duration),         // 時間1の位
                4 => TimeSpan.FromMinutes(10 * duration),           // 分10の位
                5 or 6 => TimeSpan.FromMinutes(1 * duration),       // 分1の位
                7 => TimeSpan.FromSeconds(10 * duration),           // 秒10の位
                8 or 9 => TimeSpan.FromSeconds(1 * duration),       // 秒1の位
                10 => TimeSpan.FromMilliseconds(100 * duration),    // ミリ秒100の位
                11 => TimeSpan.FromMilliseconds(10 * duration),     // ミリ秒10の位
                12 => TimeSpan.FromMilliseconds(1 * duration),      // ミリ秒1の位
                _ => TimeSpan.Zero
            };
        }

        /// <summary>
        /// 変換開始時間を増減する。
        /// </summary>
        private void ChangeFileItemStartPosition(int duration)
        {
            if (SelectedFileItem == null) return;

            var delta = CalculateDelta(SelectedFileItem.StartPositionCaretIndex, duration);

            TimeSpan ts = SelectedFileItem.StartPosition.Add(delta);

            SelectedFileItem.StartPosition = ts;
        }

        /// <summary>
        /// 変換終了時間を増減する。
        /// </summary>
        private void ChangeFileItemEndPosition(int duration)
        {
            if (SelectedFileItem == null) return;

            var delta = CalculateDelta(SelectedFileItem.EndPositionCaretIndex, duration);

            TimeSpan ts = SelectedFileItem.EndPosition.Add(delta);

            SelectedFileItem.EndPosition = ts;
        }

        /// <summary>
        /// ファイルがドラッグ＆ドロップされた際の処理。
        /// </summary>
        private void OnFilesDropped(string[] files)
        {

            AddItem(files);
        }

        /// <summary>
        /// ファイルがチェックされた際の処理。
        /// 変換対象としてマークし、ボタン状態を更新する。
        /// </summary>
        private void OnFileItemChecked(FileItem item)
        {
            // チェックされた
            item.IsChecked = true;

            // ボタン状態
            UpdateToolboxButtonsState();
        }

        /// <summary>
        /// ファイルのチェックが外された際の処理。
        /// </summary>
        private void OnFileItemUnchecked(FileItem item)
        {
            // チェック解除された
            item.IsChecked = false;

            // ボタン状態
            UpdateToolboxButtonsState();
        }

        /// <summary>
        /// ツールバー／ログ／プレビュー／ステータスバー表示状態を
        /// 設定に反映する。
        /// 
        /// 実際の ON/OFF 切り替えはバインディングによって行われる。
        /// </summary>
        private void ToggleVisibility()
        {
            _settings.ToolbarVisible = IsToolbarVisible;
            _settings.LogVisible = IsLogVisible;
            _settings.PreviewVisible = IsPreviewVisible;
            _settings.StatusbarVisible = IsStatusbarVisible;
            _settings.FileListRowHeight = FileListRowHeight;
            _settings.LogRowHeight = LogRowHeight;
            _settings.PreviewColumnWidth = PreviewColumnWidth;
        }

        /// <summary>
        /// About ダイアログを表示する。
        /// </summary>
        /// <remarks>
        /// メソッド名は ShowAbout が望ましい（typo）。
        /// </remarks>
        private void ShowAbout()
        {
            _dialogService.ShowAboutDialog(_settings);
        }

        /// <summary>
        /// 処理中の場合、終了確認ダイアログを表示する。
        /// </summary>
        public bool ConfirmExit()
        {
            if (!IsProcessing)
                return true;

            var result = _dialogService.ShowMessageBox(
                "処理を中断して終了しますか？",
                "確認",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
                return false;

            CancelConversion();
            return true;
        }

        /// <summary>
        /// アプリケーションを終了する。
        /// </summary>
        private void Exit()
        {
            if (!ConfirmExit())
                return;

            System.Windows.Application.Current.Shutdown();　
        }
    }
}
