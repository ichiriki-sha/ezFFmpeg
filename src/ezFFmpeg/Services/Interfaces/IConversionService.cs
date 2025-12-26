using ezFFmpeg.Models.Conversion;

namespace ezFFmpeg.Services.Conversion
{
    public interface IConversionService
    {
        Task ExecuteAsync(
            ConversionContext context,
            IProgress<ConversionProgress> progress);
    }
}
