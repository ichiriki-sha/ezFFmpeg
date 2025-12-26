using ezFFmpeg.Common;
using ezFFmpeg.Models.Common;

namespace ezFFmpeg.Models.Conversion
{
    /// <summary>
    /// 変換処理に必要な情報をまとめたコンテキスト
    /// </summary>
    public sealed class ConversionContext
    {
        public IReadOnlyList<FileItem> Files { get; init; } = [];
        public AppSettings Settings { get; init; } = default!;
        public CancellationToken Token { get; init; }
    }
}
