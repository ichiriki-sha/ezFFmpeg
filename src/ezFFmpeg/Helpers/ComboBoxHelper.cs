using ezFFmpeg.Extensions;
using ezFFmpeg.Models.Codec;
using ezFFmpeg.Models.Common;
using ezFFmpeg.Models.Encoder;
using ezFFmpeg.Models.Interfaces;
using ezFFmpeg.Models.Media;
using ezFFmpeg.Models.Output;
using ezFFmpeg.Models.Profiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using static System.Windows.Forms.DataFormats;

namespace ezFFmpeg.Helpers
{
    /// <summary>
    /// ComboBox や ListBox にバインドするための
    /// <see cref="SelectionItem"/> コレクションを構築するヘルパークラス。
    /// 
    /// ・プロファイル
    /// ・エンコーダ
    /// ・Enum / 定数
    /// 
    /// など、UI 表示用の選択肢生成ロジックを共通化することを目的とする。
    /// </summary>
    public static class ComboBoxHelper
    {
        /// <summary>
        /// プロファイル一覧から SelectionItem を生成して設定する。
        /// </summary>
        /// <param name="list">出力先の SelectionItem コレクション</param>
        /// <param name="profiles">プロファイル一覧</param>
        public static void SetFromProfiles(ObservableCollection<SelectionItem> list, IEnumerable<Profile> profiles)
        {
            list.Clear();
            foreach (var profile in profiles)
            {
                list.Add(new SelectionItem
                {
                    Key = profile.ProfileId,
                    Name = profile.ProfileName
                });
            }
        }

        /// <summary>
        /// 出力ファイル名タグ一覧から SelectionItem を生成して設定する。
        /// </summary>
        /// <param name="list">出力先の SelectionItem コレクション</param>
        /// <param name="tags">タグ名と表示名のタプル</param>
        public static void SetFromOutputFileTags(ObservableCollection<SelectionItem> list, IEnumerable<(string Tag, string Name)> tags)
        {
            list.Clear();
            foreach (var (Tag, Name) in tags)
            {
                list.Add(new SelectionItem
                {
                    Key = Tag,
                    Name = $"{{{Tag}}} = {Name}"
                });
            }
        }

        /// <summary>
        /// ビデオエンコーダ一覧から SelectionItem を生成し、
        /// 出力フォーマットや GPU 使用可否に応じてフィルタリングを行う。
        /// </summary>
        /// <param name="list">出力先の SelectionItem コレクション</param>
        /// <param name="defultEncoder">選択すべきデフォルトエンコーダ</param>
        /// <param name="encoders">使用可能なビデオエンコーダ一覧</param>
        /// <param name="format">出力フォーマット</param>
        /// <param name="useGpu">GPU エンコードを使用するかどうか</param>
        public static void SetVideoEncoders(
                                ObservableCollection<SelectionItem> list,
                                out string defultEncoder,
                                IEnumerable<VideoEncoder> encoders,
                                OutputFormat format,
                                bool useGpu)
        {
            list.Clear();

            foreach (var encoder in encoders)
            {
                // OutputFormat に対応していないコーデックはスキップ
                if (!format.SupportedVideoCodecs.Contains(encoder.Codec))
                    continue;

                // 使用不可や GPU 条件でフィルタ
                if (!encoder.CanUse || (!useGpu && encoder.Type == EncoderType.Gpu))
                    continue;

                list.Add(new SelectionItem
                {
                    Key = encoder.Encoder,
                    Name = encoder.Name
                });
            }

            // 推奨コーデックがあれば選択状態にする
            defultEncoder = VideoEncoders.Copy.Encoder;
            if (format.RecommendedVideo != null)
            {
                if (useGpu)
                {
                    foreach (var key in list.Select(x => x.Key.ToString()!))
                    {
                        var encoder = VideoEncoders.GetEncoder(key);
                        if (encoder.UseGpu && encoder.Codec.Codec == format.RecommendedVideo.Codec)
                        {
                            defultEncoder = encoder.Encoder;
                            break;
                        }
                    }
                }

                if(defultEncoder == VideoEncoders.Copy.Encoder)
                {
                    foreach (var key in list.Select(x => x.Key.ToString()!))
                    {
                        var encoder = VideoEncoders.GetEncoder(key);
                        if (!encoder.UseGpu && encoder.Codec.Codec == format.RecommendedVideo.Codec)
                        {
                            defultEncoder = encoder.Encoder;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// オーディオエンコーダ一覧から SelectionItem を生成し、
        /// 出力フォーマットに応じてフィルタリングを行う。
        /// </summary>
        /// <param name="list">出力先の SelectionItem コレクション</param>
        /// <param name="defultEncoder">選択すべきデフォルトエンコーダ</param>
        /// <param name="encoders">使用可能なオーディオエンコーダ一覧</param>
        /// <param name="format">出力フォーマット</param>
        public static void SetAudioEncoders(
                        ObservableCollection<SelectionItem> list,
                        out string defultEncoder,
                        IEnumerable<AudioEncoder> encoders,
                        OutputFormat format)
        {
            list.Clear();

            foreach (var encoder in encoders)
            {
                // OutputFormat に対応していないコーデックはスキップ
                if (!format.SupportedAudioCodecs.Contains(encoder.Codec))
                    continue;

                // 使用不可 でフィルタ
                if (!encoder.CanUse)
                    continue;

                list.Add(new SelectionItem
                {
                    Key = encoder.Encoder,
                    Name = encoder.Name
                });
            }

            // 推奨コーデックがあれば選択状態にする
            defultEncoder = AudioEncoders.Copy.Encoder;
            if (format.RecommendedAudio != null)
            {
                foreach (var key in list.Select(x => x.Key.ToString()!))
                {
                    var encoder = AudioEncoders.GetEncoder(key);
                    if (encoder.Codec.Codec == format.RecommendedAudio.Codec)
                    {
                        defultEncoder = encoder.Encoder;
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// 任意のコレクションから SelectionItem を生成して設定する汎用メソッド。
        /// </summary>
        public static void SetFromCollection<T>(
            ObservableCollection<SelectionItem> list,
            IEnumerable<T> source,
            Func<T, object> keySelector,
            Func<T, string> nameSelector,
            Func<T, object?>? tagSelector = null)
        {
            list.Clear();
            foreach (var item in source)
            {
                list.Add(new SelectionItem
                {
                    Key = keySelector(item).ToString() ?? "",
                    Name = nameSelector(item),
                    Tag = tagSelector?.Invoke(item)
                });
            }
        }

        /// <summary>
        /// Enum の値一覧から SelectionItem を生成して設定する。
        /// DisplayName 属性があれば表示名として使用する。
        /// </summary>
        public static void SetFromEnum<TEnum>(ObservableCollection<SelectionItem> list)
            where TEnum : Enum
        {
            list.Clear();
            foreach (var value in Enum.GetValues(typeof(TEnum)).Cast<TEnum>())
            {
                list.Add(new SelectionItem
                {
                    Key = value.ToString(),
                    Name = ((Enum)(object)value).GetDisplayName()
                });
            }
        }

        /// <summary>
        /// 定数クラス（public const string）から SelectionItem を生成して設定する。
        /// </summary>
        /// <param name="list">出力先の SelectionItem コレクション</param>
        /// <param name="constantClass">定数を定義したクラス型</param>
        public static void SetFromConstants(
            ObservableCollection<SelectionItem> list,
            Type constantClass)
        {
            list.Clear();
            var fields = constantClass.GetFields(
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (var field in fields)
            {
                if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                {
                    var value = field.GetRawConstantValue()?.ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        list.Add(new SelectionItem
                        {
                            Key = value,
                            Name = value
                        });
                    }
                }
            }
        }
    }
}
