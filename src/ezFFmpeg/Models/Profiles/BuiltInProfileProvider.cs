using ezFFmpeg.Models.Presets;
using ezFFmpeg.Models.Quality;
using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Profiles
{
    /// <summary>
    /// アプリケーションに標準で用意される
    /// 「組み込みプロファイル（Built-in Profile）」を生成するクラス。
    /// 
    /// ・初回起動時
    /// ・設定リセット時
    /// ・新規環境構築時
    /// に使用されることを想定している。
    /// </summary>
    public class BuiltInProfileProvider
    {
        /// <summary>
        /// すべての既定プロファイルを生成する。
        /// </summary>
        /// <param name="useGpu">
        /// GPU エンコーダを使用するかどうか。
        /// true の場合、GPU 対応エンコーダを優先した Profile が作成される。
        /// </param>
        /// <returns>
        /// 既定プロファイルの一覧。
        /// 並び順は UI 表示順を想定している。
        /// </returns>
        public static List<Profile> CreateDefaults(bool useGpu)
        {
            return new List<Profile>
                    {
                        CreateLastUsed(useGpu),
                        CreateStandard(useGpu),
                        CreateHighQuality(useGpu),
                        CreateSns30Fps(useGpu)
                    };
        }

        /// <summary>
        /// 「前回使用」プロファイルを作成する。
        /// 
        /// アプリ起動時に自動選択されることを想定したプロファイルで、
        /// IsDefault / IsLastUsed フラグを有効にする。
        /// </summary>
        private static Profile CreateLastUsed(bool useGpu)
        {
            var profile = new Profile(useGpu)
            {
                IsDefault = true,
                IsLastUsed = true
            };

            profile.ProfileName = $"前回使用({profile.BuildProfileName()})";

            return profile;
        }

        /// <summary>
        /// 標準品質のプロファイルを作成する。
        /// 
        /// 特別な調整を行わず、
        /// もっとも汎用的なエンコード設定を想定したプロファイル。
        /// </summary>
        private static Profile CreateStandard(bool useGpu)
        {
            var profile = new Profile(useGpu);

            profile.ProfileName = $"標準({profile.BuildProfileName()})";

            return profile;
        }

        /// <summary>
        /// 高画質設定のプロファイルを作成する。
        /// 
        /// ビデオ品質を High に設定し、
        /// 画質優先のエンコードを行いたいユーザー向け。
        /// </summary>
        private static Profile CreateHighQuality(bool useGpu)
        {
            var profile = new Profile(useGpu)
            {
                VideoQualityTier = Models.Quality.QualityTier.High
            };

            profile.ProfileName = $"高画質({profile.BuildProfileName()})";

            return profile;
        }

        /// <summary>
        /// SNS 投稿向け（30fps 固定）のプロファイルを作成する。
        /// 
        /// ・フレームレートを 30fps に固定（CFR）
        /// ・SNS や動画共有サービス向けの一般的な設定
        /// を想定している。
        /// </summary>
        private static Profile CreateSns30Fps(bool useGpu)
        {
            var profile = new Profile(useGpu)
            {
                VideoFrameRateMode = VideoFrameRateModes.Cfr.FrameRateMode,
                VideoFrameRate = VideoFrameRates.Fps30.FrameRate
            };

            profile.ProfileName = $"SNS用({profile.BuildProfileName()})";

            return profile;
        }
    }
}
