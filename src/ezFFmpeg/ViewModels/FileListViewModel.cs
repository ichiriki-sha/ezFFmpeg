using ezFFmpeg.Common;
using ezFFmpeg.Models.Common;
using ezFFmpeg.Models.Media;
using ezFFmpeg.Services.FFmpeg;
using System.Collections.ObjectModel;
using System.Windows;

namespace ezFFmpeg.ViewModels
{
    /// <summary>
    /// ファイル一覧（FileItem）の管理を担当する ViewModel
    /// 追加・削除・クリア・メディア情報ロードを集約する
    /// </summary>
    public sealed class FileListViewModel : BindableBase
    {
        public ObservableCollection<FileItem> Items { get; } = [];

        private FileItem? _selectedItem;
        public FileItem? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        private static async Task LoadThumbnailsAsync(IEnumerable<FileItem> items)
        {
            var semaphore = new SemaphoreSlim(InternalParallel.Default);

            var tasks = items
                .Select(async item =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var thumbnail = await Task.Run(() =>
                            item.MediaInfo.CreateThumbnail());

                        if (thumbnail != null)
                        {
                            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                item.VideoThumbnail = thumbnail;
                            });
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// ファイルを追加する
        /// </summary>
        public async Task<int> AddAsync(
            IEnumerable<string> files,
            IFFmpegService ffmpegService)
        {
            List<FileItem> items = [];
            var semaphore = new SemaphoreSlim(InternalParallel.Default);
            var tasks = files.Select(async path =>
            {
                await semaphore.WaitAsync();
                try
                {
                    return await Task.Run(() =>
                        MediaInfo.TryCreate(ffmpegService, path));
                }
                finally
                {
                    semaphore.Release();
                }
            });

            var infos = (await Task.WhenAll(tasks))
                        .Where(i => i != null)!;

            int addedCount = 0;

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                FileItem? firstItem = null;
                foreach (var info in infos)
                {
                    if (info != null)
                    {
                        if (Items.Any(i =>
                            i.FilePath.Equals(info.FilePath,
                                StringComparison.OrdinalIgnoreCase)))
                            continue;

                        var item = new FileItem(info!);

                        firstItem ??= item;
                        items.Add(item);
                        addedCount++;
                    }
                }

                firstItem?.IsSelected = true;

                foreach (var item in items)
                    Items.Add(item);

                if (firstItem != null)
                    SelectedItem = firstItem;

                // サムネイル読み込み
                _ = LoadThumbnailsAsync(items);
            });

            return addedCount;
        }

        /// <summary>
        /// 選択されているアイテムを削除する
        /// </summary>
        public int RemoveSelected()
        {
            int removed = 0;

            for (int i = Items.Count - 1; i >= 0; i--)
            {
                if (Items[i].IsSelected)
                {
                    Items.RemoveAt(i);
                    removed++;
                }
            }
            return removed;
        }

        /// <summary>
        /// すべてのアイテムをクリアする
        /// </summary>
        public void Clear()
        {
            Items.Clear();
            SelectedItem = null;
        }

        /// <summary>
        /// チェックされているアイテムを取得する
        /// </summary>
        public IReadOnlyList<FileItem> GetCheckedItems()
            => Items.Where(i => i.IsChecked).ToList();

        /// <summary>
        /// 選択されているアイテムを取得する
        /// </summary>
        public IReadOnlyList<FileItem> GetSelectedItems()
            => Items.Where(i => i.IsSelected).ToList();
    }
}
