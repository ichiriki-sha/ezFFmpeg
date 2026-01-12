using ezFFmpeg.Common;
using ezFFmpeg.Helpers;
using ezFFmpeg.Models.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ezFFmpeg.Models.Common
{
    /// <summary>
    /// ファイル情報とメディア情報を保持するクラス。
    /// UIバインディング用に INotifyPropertyChanged を実装している。
    /// </summary>
    public class FileItem : BindableBase
    {

        // --------------------
        // ファイル情報
        // --------------------

        /// <summary>ファイル名（拡張子付き）</summary>
        public string FileName { get; set; }

        /// <summary>ファイルパス</summary>
        public string FilePath { get; set; }

        /// <summary>最終更新日時</summary>
        public DateTime FileLastWriteTime { get; set; }

        /// <summary>最終更新日時を文字列で取得</summary>
        public string FileLastWriteTimeInfo => FileLastWriteTime.ToString("yyyy/MM/dd HH:mm:ss");

        /// <summary>ファイル種別の表示名</summary>
        public string FileType { get; set; }

        /// <summary>ファイルサイズの表示用文字列</summary>
        public string FileSize { get; set; }

        /// <summary>フォルダ名（グループ化用）</summary>
        public string? FolderName { get; set; }

        /// <summary>ファイルアイコン</summary>
        public ImageSource FileIcon { get; set; }

        // --------------------
        // ビデオ情報
        // --------------------

        public string VideoCodec { get; set; }
        public TimeSpan VideoDuration { get; set; }
        public string VideoDurationText { get => VideoDuration.ToString(@"hh\:mm\:ss\.fff"); }

        public string VideoResolution { get; set; }
        public Size VideoResolutionSize { get; set; }
        public string VideoAspectRatio { get; set; }
        public string VideoBitRate { get; set; }
        public string VideoFrameRate { get; set; }

        private ImageSource? _videoThumbnail;

        /// <summary>
        /// ビデオサムネイル画像
        /// </summary>
        public ImageSource? VideoThumbnail
        {
            get => _videoThumbnail;
            set => SetProperty(ref _videoThumbnail, value);
        }

        // --------------------
        // オーディオ情報
        // --------------------

        public string? AudioCodec { get; set; }
        public string? AudioSampleRate { get; set; }
        public string? AudioBitRate { get; set; }
        public string? AudioChannels { get; set; }

        // --------------------
        // 表示用情報
        // --------------------

        public string DurationInfo 
        {
            get => $"{UiIcons.Time}{VideoDurationText}";
        }

        public string VideoInfo
        {
            get => $"{UiIcons.Video}{VideoCodec} {VideoResolution} {VideoAspectRatio} {VideoBitRate} {VideoFrameRate}";
        }

        public string AudioInfo
        {
            get {
                if (!string.IsNullOrEmpty(AudioCodec))
                {
                    return $"{UiIcons.Audio}{AudioCodec} {AudioSampleRate} {AudioBitRate} {AudioChannels}";
                }
                else
                {
                    return "";
                }
            }
        }

        // --------------------
        // 処理状況
        // --------------------

        /// <summary>処理ログの行コレクション</summary>
        public ObservableCollection<string> ProcessingLogLines { get; } = [];

        private ProcessingStatus _status;
        /// <summary>
        /// 処理状況
        /// </summary>
        public ProcessingStatus Status
        {
            get => _status;
            set
            {
                if (SetProperty(ref _status, value))
                    RaisePropertyChanged(nameof(StatusText));
            }
        }

        private bool _isSelected;

        /// <summary>
        /// UIで選択されているかどうか
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value); 
        }

        private FileItem? _selectedFileItem;

        /// <summary>
        /// 現在選択されている FileItem
        /// </summary>
        public FileItem? SelectedFileItem
        {
            get => _selectedFileItem;
            set => SetProperty(ref _selectedFileItem, value);
        }

        private bool _isChecked;

        /// <summary>
        /// チェック状態（UI用）
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }

        private double _progress;

        /// <summary>
        /// 処理進捗（0～100）
        /// </summary>
        public double Progress
        {
            get => _progress;
            set
            {
                if (SetProperty(ref _progress, value))
                    RaisePropertyChanged(nameof(StatusText));
            }
        }

        /// <summary>
        /// ステータス表示用文字列
        /// </summary>
        public string StatusText => Status switch
        {
            ProcessingStatus.Pending => "未処理",
            ProcessingStatus.Processing => $"{Progress:F1}%",
            ProcessingStatus.Completed => "処理済",
            ProcessingStatus.Canceled => "キャンセル",
            ProcessingStatus.Error => "エラー",
            _ => ""
        };

        private TimeSpan _startPosition = TimeSpan.Zero;
        public TimeSpan StartPosition
        {
            get => _startPosition;
            set 
            {
                value = value < TimeSpan.Zero ? TimeSpan.Zero : value;
                value = value > EndPosition ? EndPosition : value;
                if (SetProperty(ref _startPosition, value))
                    RaisePropertyChanged(nameof(StartPositionText));
            }
        }

        /// <summary>
        /// TextBox 表示用 (hh:mm:ss)
        /// </summary>
        public string StartPositionText
        {
            get => StartPosition.ToString(@"hh\:mm\:ss\.fff");
            set
            {
                if (TimeSpan.TryParse(value, out var ts))
                {
                    StartPosition = ts;
                }
                // 失敗時は何もしない
            }
        }

        private int _startPositionCaretIndex;
        public int StartPositionCaretIndex
        {
            get => _startPositionCaretIndex;
            set => SetProperty(ref _startPositionCaretIndex, value);
        }

        private TimeSpan _endPosition ;
        public TimeSpan EndPosition
        {
            get => _endPosition;
            set
            {
                value = value < StartPosition ? StartPosition : value;
                value = value > VideoDuration ? VideoDuration : value;
                if (SetProperty(ref _endPosition, value))
                    RaisePropertyChanged(nameof(EndPositionText));
            }
        }

        /// <summary>
        /// TextBox 表示用 (hh:mm:ss)
        /// </summary>
        public string EndPositionText
        {
            get => EndPosition.ToString(@"hh\:mm\:ss\.fff");
            set
            {
                if (TimeSpan.TryParse(value, out var ts))
                {
                    EndPosition = ts;
                }
                // 失敗時は何もしない
            }
        }

        private int _endPositionCaretIndex ;
        public int EndPositionCaretIndex
        {
            get => _endPositionCaretIndex;
            set => SetProperty(ref _endPositionCaretIndex, value);
        }

        /// <summary>
        /// 元の MediaInfo を保持
        /// </summary>
        public MediaInfo MediaInfo { get; }

        /// <summary>
        /// コンストラクタ
        /// FileItem を初期化する
        /// </summary>
        public FileItem()
        {
            FileName = string.Empty;
            FilePath = string.Empty;
            FileType = string.Empty;
            FileSize = string.Empty;
            FileIcon = BitmapFrame.Create(new Uri("pack://application:,,,/Resources/Images/File.png"));
            VideoCodec = string.Empty;
            VideoResolution = string.Empty;
            VideoAspectRatio = string.Empty;
            VideoBitRate = string.Empty;
            VideoFrameRate = string.Empty;
            MediaInfo = null!;
            StartPosition = TimeSpan.Zero;
            EndPosition   = TimeSpan.Zero;
        }

        /// <summary>
        /// コンストラクタ
        /// MediaInfo から FileItem を初期化する
        /// </summary>
        /// <param name="info">初期化に使用する MediaInfo オブジェクト</param>
        public FileItem(MediaInfo info)
        {
            if (info.Video == null)
                throw new ArgumentException("Video情報がありません", nameof(info));

            var fi = new FileInfo(info.FilePath);
            var ext = Path.GetExtension(info.FilePath).ToLowerInvariant();

            MediaInfo = info;

            // ---- ファイル情報 ----
            FilePath = info.FilePath;
            FolderName = Path.GetDirectoryName(info.FilePath);
            FileName = Path.GetFileName(info.FilePath);
            FileLastWriteTime = File.GetLastWriteTime(info.FilePath);
            FileType = FileTypeHelper.GetFileTypeDescription(ext);
            FileSize = FileSizeHelper.GetReadableFileSize(fi.Length);
            FileIcon = FileIconHelper.GetIcon(info.FilePath, false);

            // ---- ビデオ情報 ----
            VideoCodec = info.Video.Codec!;
            VideoDuration = info.Video.Duration;
            VideoResolution = info.Video.SizeString;
            VideoResolutionSize = new Size(info.Video.Width, info.Video.Height);
            VideoBitRate = info.Video.BitRateString;
            VideoFrameRate = info.Video.FrameRateString;
            VideoAspectRatio = info.Video.DisplayAspectRatio!;

            // 仮サムネイル
            VideoThumbnail = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Movie.png"));

            // ---- オーディオ情報 ----
            if (info.Audio != null)
            {
                AudioCodec = info.Audio.Codec;
                AudioSampleRate = info.Audio.SampleRateString;
                AudioBitRate = info.Audio.BitRateString;
                AudioChannels = info.Audio.ChannelsString;
            }

            // ---- 状態初期化 ----
            IsChecked = true;
            Progress = 0;
            Status = ProcessingStatus.Pending;

            StartPosition = TimeSpan.Zero;
            StartPositionCaretIndex　= 12;
            EndPosition = info.Video.Duration;
            EndPositionCaretIndex = 12;
        }
    }
}