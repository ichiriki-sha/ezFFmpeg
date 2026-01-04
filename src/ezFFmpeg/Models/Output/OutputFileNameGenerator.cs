using ezFFmpeg.Common;
using ezFFmpeg.Models.Common;
using ezFFmpeg.Models.Encoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ezFFmpeg.Models.Output
{

    /// <summary>
    /// 出力ファイル名を生成するユーティリティクラス。
    /// プロファイル設定とテンプレート文字列を元に、
    /// タグ置換を行い最終的な出力ファイル名を生成する。
    /// </summary>
    public static class OutputFileNameGenerator
    {

        /// <summary>
        /// ビデオエンコーダー用のタグ文字列を取得する。
        /// Copy の場合は入力ファイルのコーデック名を使用する。
        /// </summary>
        /// <param name="file">入力ファイル情報</param>
        /// <param name="profile">現在のプロファイル</param>
        /// <returns>ビデオエンコーダーを表す文字列</returns>
        private static string GetVideoEncoderTag(FileItem file, Profile profile)
        {
            if (profile.IsVideoEnabled)
            {

                var videoEncoder = VideoEncoders.GetCodec(profile.VideoEncoder);

                if (videoEncoder.IsCopy)
                {
                    return file.VideoCodec.ToLower() ?? "none";
                }
                else
                {
                    return videoEncoder.Name.Replace(".", "").ToLower();
                }
            }
            else
            {
                return "none";
            }
        }

        /// <summary>
        /// オーディオエンコーダー用のタグ文字列を取得する。
        /// Copy の場合は入力ファイルのコーデック名を使用する。
        /// </summary>
        /// <param name="file">入力ファイル情報</param>
        /// <param name="profile">現在のプロファイル</param>
        /// <returns>オーディオエンコーダーを表す文字列</returns>
        private static string GetAudioEncoderTag(FileItem file, Profile profile)
        {
            if (profile.IsAudioEnabled)
            {

                var audioEncoder = AudioEncoders.GetCodec(profile.AudioEncoder);

                if (audioEncoder.IsCopy)
                {
                    return file.AudioCodec.ToLower() ?? "none";
                }
                else
                {
                    return audioEncoder.Name.Replace(".", "").ToLower();
                }
            }
            else
            {
                return "none";
            }
        }

        /// <summary>
        /// ビデオ解像度用のタグ文字列を取得する。
        /// </summary>
        /// <param name="profile">現在のプロファイル</param>
        /// <returns>解像度タグ文字列</returns>
        private static string GetVideoResolutionTag(Profile profile) => profile.IsVideoEnabled ? profile.VideoResolution : "none";

        /// <summary>
        /// 指定した入力ファイルとアプリケーション設定に基づき、
        /// 出力ファイル名を生成する。
        /// </summary>
        /// <param name="file">入力ファイル情報</param>
        /// <param name="settings">アプリケーション設定（現在のプロファイルを含む）</param>
        /// <returns>生成された出力ファイル名</returns>
        public static string Generate(FileItem file, AppSettings settings)
        {
            // 各タグに対応する値を設定
            var tags = new Dictionary<string, string>
            {
                [OutputFileTags.FileName.Tag] = Path.GetFileNameWithoutExtension(file.FilePath),
                [OutputFileTags.VideoCodec.Tag] = GetVideoEncoderTag(file,settings.CurrentProfile),
                [OutputFileTags.VideoResolution.Tag] = GetVideoResolutionTag(settings.CurrentProfile),
                [OutputFileTags.AudioCodec.Tag] = GetAudioEncoderTag(file, settings.CurrentProfile),
                [OutputFileTags.TimeStamp.Tag] = settings.ProcessStartTime.ToString("yyyyMMddHHmmss")
            };

            // プロファイルで指定された出力ファイル名テンプレート
            string ret = $"{settings.CurrentProfile.OutputFileFormat}{settings.CurrentProfile.OutputFormat}";

            // タグをテンプレート内に置換
            foreach (var tag in tags)
            {
                string pattern = $@"\{{{Regex.Escape(tag.Key)}\}}"; // {tag} の形式にマッチs
                ret = Regex.Replace(ret, pattern, tag.Value, RegexOptions.IgnoreCase);
            }

            return ret;
        }
    }
}
