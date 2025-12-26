using ezFFmpeg.Models.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Services.FFmpeg
{
    /// <summary>
    /// FFprobe の実行引数を生成するユーティリティクラス。
    /// </summary>
    public class FFprobeArgumentBuilder
    {
        /// <summary>
        /// 指定したメディアファイルの情報を取得するための FFprobe 引数を生成します。
        /// JSON 形式でストリーム情報を取得するように設定されます。
        /// </summary>
        /// <param name="filePath">メディアファイルのフルパス</param>
        /// <returns>FFprobe 実行用の引数文字列</returns>
        public static string BuildMediaInfoArguments(string filePath)
        {
            // -hide_banner: 起動バナーを非表示
            // -v error: エラーメッセージのみ表示
            // -i "filePath": 入力ファイル指定
            // -show_streams: ストリーム情報を出力
            // -of json: JSON 形式で出力
            return $"-hide_banner -v error -i \"{filePath}\" -show_streams -of json";
        }

        /// <summary>
        /// FFprobe バージョン確認用引数を生成
        /// </summary>
        public static string BuildVersionArguments() => "-hide_banner -version";

    }
}
