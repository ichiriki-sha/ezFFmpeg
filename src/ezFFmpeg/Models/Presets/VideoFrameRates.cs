using System;

namespace ezFFmpeg.Models.Presets
{
    /// <summary>
    /// ビデオフレームレート（FPS）のプリセットを定義する静的クラス。
    /// 
    /// 各プリセットは <see cref="VideoFrameRate"/> として定義され、
    /// UI の選択肢、プロファイル設定、FFmpeg 引数生成などで共通的に使用される。
    /// </summary>
    public static class VideoFrameRates
    {
        /// <summary>
        /// 入力動画のフレームレートをそのまま使用する設定。
        /// </summary>
        public static readonly VideoFrameRate Source = new("source", "変更しない", true);

        /// <summary>24fps プリセット</summary>
        public static readonly VideoFrameRate Fps24 = new("24", "24fps", false);

        /// <summary>25fps プリセット</summary>
        public static readonly VideoFrameRate Fps25 = new("25", "25fps", false);

        /// <summary>29.97fps プリセット（NTSC）</summary>
        public static readonly VideoFrameRate Fps29_97 = new("29.97", "29.97fps", false);

        /// <summary>30fps プリセット</summary>
        public static readonly VideoFrameRate Fps30 = new("30", "30fps", false);

        /// <summary>50fps プリセット</summary>
        public static readonly VideoFrameRate Fps50 = new("50", "50fps", false);

        /// <summary>59.94fps プリセット（NTSC）</summary>
        public static readonly VideoFrameRate Fps59_94 = new("59.94", "59.94fps", false);

        /// <summary>60fps プリセット</summary>
        public static readonly VideoFrameRate Fps60 = new("60", "60fps", false);

        /// <summary>120fps プリセット</summary>
        public static readonly VideoFrameRate Fps120 = new("120", "120fps", false);

        /// <summary>
        /// 定義されているすべてのフレームレートプリセット。
        /// ComboBox の ItemsSource やバリデーション処理で使用される。
        /// </summary>
        public static readonly VideoFrameRate[] All =
        [
            Source, Fps24, Fps25, Fps29_97, Fps30, Fps50, Fps59_94, Fps60, Fps120
        ];

        /// <summary>
        /// 指定された識別値に対応するフレームレートプリセットを取得する。
        /// </summary>
        /// <param name="frameRate">フレームレート識別値（例: "30", "59.94", "source"）</param>
        /// <returns>対応する <see cref="VideoFrameRate"/> インスタンス</returns>
        /// <exception cref="ArgumentException">指定された識別値が未対応の場合</exception>
        public static VideoFrameRate GetFrameRate(string frameRate)
        {
            var videoFrameRate = Array.Find(All, f => f.FrameRate == frameRate);
            if (videoFrameRate == null)
                throw new ArgumentException($"未対応の frameRate: {frameRate}",nameof(frameRate));

            return videoFrameRate;
        }
    }
}
