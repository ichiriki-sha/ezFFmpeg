using ezFFmpeg.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace ezFFmpeg.Services.FFmpeg.EncoderCapabilityProvider
{
    /// <summary>
    /// FFmpeg のエンコーダー情報を取得・キャッシュするクラス。
    /// 起動時に一度 FFmpeg を呼び出してエンコーダーの一覧を取得し、その後はキャッシュを参照する。
    /// </summary>
    public sealed class EncoderCapabilities
    {
        private readonly IFFmpegService _ffmpegService;

        /// <summary>
        /// エンコーダー情報のキャッシュ。キーはエンコーダ名、値は EncoderInfo。
        /// </summary>
        private IReadOnlyDictionary<string, EncoderInfo>? _cache;

        /// <summary>
        /// EncoderCapabilities の新しいインスタンスを初期化する。
        /// </summary>
        /// <param name="ffmpegService">FFmpeg 実行サービス</param>
        public EncoderCapabilities(IFFmpegService ffmpegService)
        {
            _ffmpegService = ffmpegService;
        }

        /// <summary>
        /// FFmpeg からエンコーダー情報を取得してキャッシュする。
        /// 起動時に一度だけ呼ぶこと。
        /// </summary>
        public void Load()
        {
            if (_cache != null)
                return;

            var result = new Dictionary<string, EncoderInfo>(StringComparer.OrdinalIgnoreCase);

            string args = FFmpegArgumentBuilder.BuildEncodersArguments();
            var ret = _ffmpegService.RunFFmpeg(args);

            if (ret.Success)
            {
                foreach (var line in ret.StdOut!.Split(Environment.NewLine))
                {
                    // エンコーダー情報を正規表現で解析
                    var match = Regex.Match(line, "([VASFXBD\\.]{6})\\s+(\\w+)\\s+(.+)");
                    if (!match.Success)
                        continue;

                    string flags = match.Groups[1].Value;
                    string encoder = match.Groups[2].Value;
                    string description = match.Groups[3].Value;

                    result.Add(encoder, new EncoderInfo(flags, encoder, description));
                }

                _cache = result;
            }
        }

        // ===== 公開 API =====

        /// <summary>
        /// 全エンコーダー情報を取得する。
        /// Load() が呼ばれていない場合は例外をスローする。
        /// </summary>
        /// <returns>全 EncoderInfo のコレクション</returns>
        public IReadOnlyCollection<EncoderInfo> GetAll()
            => (IReadOnlyCollection<EncoderInfo>)(_cache?.Values
               ?? throw new InvalidOperationException("EncoderCapabilityProvider not loaded."));

        /// <summary>
        /// 指定したエンコーダーの情報を取得する。
        /// </summary>
        /// <param name="encoder">エンコーダ名</param>
        /// <returns>EncoderInfo または存在しない場合は null</returns>
        public EncoderInfo? Get(string encoder)
        {
            if (_cache == null)
                throw new InvalidOperationException("EncoderCapabilityProvider not loaded.");

            _cache.TryGetValue(encoder, out var info);
            return info;
        }

        /// <summary>
        /// 指定したエンコーダーが使用可能かどうかを判定する。
        /// </summary>
        /// <param name="encoder">エンコーダ名</param>
        /// <returns>使用可能なら true、それ以外は false</returns>
        public bool CanUse(string encoder)
            => Get(encoder) != null;
    }
}
