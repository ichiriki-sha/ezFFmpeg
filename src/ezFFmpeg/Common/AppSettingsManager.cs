using ezFFmpeg.Models.Common;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;

namespace ezFFmpeg.Common
{
    /// <summary>
    /// アプリケーション設定（<see cref="AppSettings"/>）の
    /// 読み込みおよび保存を担当するユーティリティクラス。
    /// 
    /// 設定ファイルは JSON 形式で保存され、
    /// ファイルが存在しない、または読み込みに失敗した場合は
    /// 既定値を持つ <see cref="AppSettings"/> を返す。
    /// </summary>
    public static class AppSettingsManager
    {

        // =====================================================
        // JsonSerializerOptions のキャッシュ
        // =====================================================
        private static readonly JsonSerializerOptions s_jsonOptions = new() { WriteIndented = true };

        // =====================================================
        // 設定の読み込み
        // =====================================================

        /// <summary>
        /// 設定ファイルを読み込み、<see cref="AppSettings"/> を生成する。
        /// </summary>
        /// <returns>
        /// 読み込みに成功した場合は設定ファイルの内容を反映した <see cref="AppSettings"/>。
        /// ファイルが存在しない、またはエラーが発生した場合は
        /// 既定値で初期化された <see cref="AppSettings"/>。
        /// </returns>
        public static AppSettings LoadSettings()
        {
            // 設定ファイルのパスを取得
            var filePath = AppPath.GetAppSettingPath();

            try
            {
                // ファイルが存在する場合のみ読み込みを行う
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    // UTF-8 として JSON を読み込み
                    var json = File.ReadAllText(filePath, Encoding.UTF8);

                    // JSON から AppSettings をデシリアライズ
                    var loaded = JsonSerializer.Deserialize<AppSettings>(json, s_jsonOptions);

                    // 正常に読み込めた場合はそのまま返す
                    if (loaded != null)
                    {
                        Profile? profile;
                        
                        // デフォルトがあれば設定する
                        profile = loaded.Profiles.FirstOrDefault(p => p.IsDefault);
                        if (profile != null)
                        {
                            loaded.CurrentProfile = profile;
                        }

                        // 最終に使用したのもがあれば設定する
                        profile = loaded.Profiles.FirstOrDefault(p => p.IsLastUsed);
                        if (profile != null)
                        {
                            loaded.CurrentProfile = profile;
                        }

                        return loaded;
                    }
                }
            }
            catch (Exception ex) {

                Debug.WriteLine(ex.Message);

                // 設定読み込み失敗時は例外を外へ投げず、
                // 安全に既定設定へフォールバックする
            }

            // 読み込み失敗・ファイル未存在時は新規設定を返す
            return new AppSettings();
        }

        // =====================================================
        // 設定の保存
        // =====================================================

        /// <summary>
        /// 指定された <see cref="AppSettings"/> を JSON 形式で保存する。
        /// </summary>
        /// <param name="settings">
        /// 保存対象のアプリケーション設定。
        /// </param>
        public static void SaveSettings(AppSettings settings)
        {
            // 設定ファイルの保存先パスを取得
            var filePath = AppPath.GetAppSettingPath();

            try
            {
                // インデント付きで JSON を生成（可読性向上）
                var json = JsonSerializer.Serialize(
                    settings,
                    s_jsonOptions);

                // UTF-8 でファイルに書き込み
                File.WriteAllText(filePath, json, Encoding.UTF8);
            }
            catch
            {
                // 設定保存失敗は致命的ではないため例外は無視
                // （アプリ終了時などでも安全に呼び出せるようにするため）
            }
        }
    }
}
