using ezFFmpeg.Models.Encoder;
using ezFFmpeg.Models.Media;

namespace ezFFmpeg.Services.FFmpeg.EncoderCapabilityProvider
{
    /// <summary>
    /// FFmpeg のエンコーダー情報を表すクラス。
    /// エンコーダー名、説明、メディアタイプ、GPU/CPU 種類、フラグ情報を保持します。
    /// </summary>
    public sealed class EncoderInfo
    {
        /// <summary>
        /// エンコーダーの識別名 (例: "libx264", "h264_nvenc")。
        /// </summary>
        public string Encoder { get; }

        /// <summary>
        /// エンコーダーの説明文字列。
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// エンコーダーが対応するメディアタイプ (Video/Audio/Subtitle)。
        /// </summary>
        public MediaType MediaType { get; }

        /// <summary>
        /// エンコーダーの種類 (CPU / GPU)。
        /// </summary>
        public EncoderType EncoderType { get; }

        /// <summary>
        /// エンコーダーフラグ情報。
        /// スレッディング、実験的機能対応などの詳細を保持。
        /// </summary>
        public EncoderFlags Flags { get; }

        /// <summary>
        /// GPU 対応エンコーダーかどうか。
        /// </summary>
        public bool CanUseGpu => EncoderType == EncoderType.Gpu;

        /// <summary>
        /// 実験的な機能を持つエンコーダーかどうか。
        /// </summary>
        public bool IsExperimental => Flags.IsExperimental;

        /// <summary>
        /// EncoderInfo の新しいインスタンスを初期化します。
        /// エンコーダーフラグを解析し、メディアタイプと GPU/CPU 種類を設定します。
        /// </summary>
        /// <param name="flags">FFmpeg エンコーダーフラグ文字列</param>
        /// <param name="encoder">エンコーダー名</param>
        /// <param name="description">エンコーダー説明</param>
        public EncoderInfo(string flags, string encoder, string description)
        {
            Encoder = encoder;
            Description = description;

            Flags = EncoderFlags.Parse(flags);

            MediaType = Flags.MediaType;
            EncoderType = EncoderGpuDetector.IsGpu(encoder)
                ? EncoderType.Gpu
                : EncoderType.Cpu;
        }

        /// <summary>
        /// デバッグやログ用にエンコーダー情報を文字列化します。
        /// 形式: "エンコーダー名 (メディアタイプ, エンコーダー種類)"
        /// </summary>
        /// <returns>エンコーダー情報の文字列表現</returns>
        public override string ToString()
            => $"{Encoder} ({MediaType}, {EncoderType})";
    }
}
