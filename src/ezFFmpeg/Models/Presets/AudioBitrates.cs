using ezFFmpeg.Models.Encoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Presets
{
    /// <summary>
    /// オーディオビットレートのプリセットを定義する静的クラス。
    /// 各プリセットは (BitRate, Name) のタプルで保持され、ユーザー選択やプロファイル設定に利用される。
    /// </summary>
    public static class AudioBitRates
    {
        /// <summary>
        /// ソースのまま変更しない設定
        /// </summary>
        public static readonly AudioBitRate Source = new("source", "変更しない", true);

        /// <summary>
        /// 64Kbps のプリセット
        /// </summary>
        public static readonly AudioBitRate Kbps64 = new("64", "64Kbps", false);

        /// <summary>
        /// 96Kbps のプリセット
        /// </summary>
        public static readonly AudioBitRate Kbps96 = new("96", "96Kbps", false);

        /// <summary>
        /// 128Kbps のプリセット
        /// </summary>
        public static readonly AudioBitRate Kbps128 = new("128", "128Kbps", false);

        /// <summary>
        /// 192Kbps のプリセット
        /// </summary>
        public static readonly AudioBitRate Kbps192 = new("192", "192Kbps", false);

        /// <summary>
        /// 256Kbps のプリセット
        /// </summary>
        public static readonly AudioBitRate Kbps256 = new("256", "256Kbps", false);

        /// <summary>
        /// 320Kbps のプリセット
        /// </summary>
        public static readonly AudioBitRate Kbps320 = new("320", "320Kbps", false);

        /// <summary>
        /// すべてのビットレートプリセットをまとめた配列
        /// </summary>
        public static readonly AudioBitRate[] All =
        [
            Source, Kbps64, Kbps96, Kbps128, Kbps192, Kbps256, Kbps320
        ];

        /// <summary>
        /// 指定された名前のオーディオビットレートを取得する
        /// </summary>
        /// <param name="bitRate">ビットレート識別名</param>
        /// <returns>対応する AudioBitRate インスタンス</returns>
        /// <exception cref="ArgumentException">未対応のビットレート名の場合</exception>
        public static AudioBitRate GetBitRate(string bitRate)
        {
            var audioBitRate = Array.Find(All, c => c.BitRate == bitRate);
            if (audioBitRate!.BitRate == null) 
                throw new ArgumentException($"未対応の bitRate: {bitRate}");

            return audioBitRate;
        }
    }
}
