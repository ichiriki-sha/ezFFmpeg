using ezFFmpeg.Common;
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
    /// AppSettings のプロファイル情報とテンプレートを使用して、最終的な出力ファイル名を作成する。
    /// </summary>
    public static class OutputFileNameGenerator
    {
        /// <summary>
        /// 指定した入力ファイル名と設定情報に基づき、出力ファイル名を生成する。
        /// </summary>
        /// <param name="fileName">入力ファイル名（フルパス可）</param>
        /// <param name="settings">アプリケーション設定および現在のプロファイル情報</param>
        /// <returns>生成された出力ファイル名</returns>
        public static string Generate(string fileName, AppSettings settings)
        {
            // 各タグに対応する値を設定
            var tags = new Dictionary<string, string>
            {
                [OutputFileTags.FileName.Tag] = Path.GetFileNameWithoutExtension(fileName),
                [OutputFileTags.VideoCodec.Tag] = VideoEncoders.GetCodec(settings.CurrentProfile.VideoEncoder).Name.Replace(".", ""),
                [OutputFileTags.AudioCodec.Tag] = AudioEncoders.GetCodec(settings.CurrentProfile.AudioEncoder).Name.Replace(".", ""),
                [OutputFileTags.VideoResolution.Tag] = settings.CurrentProfile.VideoResolution,
                [OutputFileTags.Extension.Tag] = settings.CurrentProfile.OutputFormat,
                [OutputFileTags.TimeStamp.Tag] = settings.ProcessStartTime.ToString("yyyyMMddHHmmss")
            };

            // プロファイルで指定された出力ファイル名テンプレート
            string ret = settings.CurrentProfile.OutputFileFormat;

            // タグをテンプレート内に置換
            foreach (var tag in tags)
            {
                string pattern = $@"\{{{Regex.Escape(tag.Key)}\}}"; // {tag} の形式にマッチ
                ret = Regex.Replace(ret, pattern, tag.Value, RegexOptions.IgnoreCase);
            }

            return ret;
        }
    }
}
