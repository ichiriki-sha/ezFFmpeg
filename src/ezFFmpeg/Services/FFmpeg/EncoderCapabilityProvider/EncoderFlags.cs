using ezFFmpeg.Models.Media;

namespace ezFFmpeg.Services.FFmpeg.EncoderCapabilityProvider
{
    /// <summary>
    /// FFmpeg エンコーダーのフラグ情報を表すクラス。
    /// FFmpeg の "-encoders" 出力に含まれる 6 文字のフラグを解析して、
    /// 動画・音声・字幕の種類や各種機能サポートを判定する。
    /// </summary>
    public sealed class EncoderFlags
    {
        /// <summary>
        /// メディアの種類（Video / Audio / Subtitle / Unknown）
        /// </summary>
        public MediaType MediaType { get; }

        /// <summary>
        /// フレーム単位のマルチスレッドをサポートするかどうか
        /// </summary>
        public bool FrameThreading { get; }

        /// <summary>
        /// スライス単位のマルチスレッドをサポートするかどうか
        /// </summary>
        public bool SliceThreading { get; }

        /// <summary>
        /// 実験的なエンコーダーかどうか
        /// </summary>
        public bool IsExperimental { get; }

        /// <summary>
        /// 水平帯域描画をサポートするかどうか
        /// </summary>
        public bool SupportsDrawHorizBand { get; }

        /// <summary>
        /// 直接レンダリングをサポートするかどうか
        /// </summary>
        public bool SupportsDirectRendering { get; }

        /// <summary>
        /// EncoderFlags の内部コンストラクタ
        /// </summary>
        private EncoderFlags(
            MediaType mediaType,
            bool frameThreading,
            bool sliceThreading,
            bool experimental,
            bool drawBand,
            bool direct)
        {
            MediaType = mediaType;
            FrameThreading = frameThreading;
            SliceThreading = sliceThreading;
            IsExperimental = experimental;
            SupportsDrawHorizBand = drawBand;
            SupportsDirectRendering = direct;
        }

        /// <summary>
        /// FFmpeg の 6 文字フラグ文字列を解析して EncoderFlags を生成する。
        /// 例: "VFSXBD" → Video, FrameThreading=true, SliceThreading=true, IsExperimental=true, SupportsDrawHorizBand=true, SupportsDirectRendering=true
        /// </summary>
        /// <param name="flags">FFmpeg エンコーダーフラグ文字列 (6 文字)</param>
        /// <returns>解析結果を反映した EncoderFlags インスタンス</returns>
        /// <exception cref="ArgumentException">無効なフラグ文字列の場合にスロー</exception>
        public static EncoderFlags Parse(string flags)
        {
            if (string.IsNullOrWhiteSpace(flags) || flags.Length < 6)
                throw new ArgumentException("Invalid encoder flags");

            var mediaType = flags[0] switch
            {
                'V' => MediaType.Video,
                'A' => MediaType.Audio,
                'S' => MediaType.Subtitle,
                _ => MediaType.Unknown
            };

            return new EncoderFlags(
                mediaType,
                flags[1] == 'F',
                flags[2] == 'S',
                flags[3] == 'X',
                flags[4] == 'B',
                flags[5] == 'D'
            );
        }
    }
}
