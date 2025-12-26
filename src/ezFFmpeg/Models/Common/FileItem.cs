using ezFFmpeg.Common;
using ezFFmpeg.Helpers;
using ezFFmpeg.Models.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace ezFFmpeg.Models.Common
{
    /// <summary>
    /// ファイル情報とメディア情報を保持するクラス。
    /// UIバインディング用に INotifyPropertyChanged を実装している。
    /// </summary>
    public class FileItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

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
            set
            {
                if (_videoThumbnail == value) return;
                _videoThumbnail = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VideoThumbnail)));
            }
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
            get => $"{UiIcons.Time}{VideoDuration:hh\\:mm\\:ss}";
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

        /// <summary>
        /// プロパティ変更通知を発行
        /// </summary>
        protected void RaisePropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private ProcessingStatus _status;

        /// <summary>
        /// 処理状況
        /// </summary>
        public ProcessingStatus Status
        {
            get => _status;
            set
            {
                if (_status == value) return;
                _status = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusText)));
            }
        }

        private bool _isSelected;

        /// <summary>
        /// UIで選択されているかどうか
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        private FileItem? _selectedFileItem;

        /// <summary>
        /// 現在選択されている FileItem
        /// </summary>
        public FileItem? SelectedFileItem
        {
            get => _selectedFileItem;
            set
            {
                if (_selectedFileItem == value) return;
                _selectedFileItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedFileItem)));
            }
        }

        private bool _isChecked;

        /// <summary>
        /// チェック状態（UI用）
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }
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
                if (_progress != value)
                {
                    _progress = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusText)));
                }
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
        }
    }
}