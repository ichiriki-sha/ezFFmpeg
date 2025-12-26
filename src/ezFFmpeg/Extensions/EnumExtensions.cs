using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ezFFmpeg.Extensions
{
    /// <summary>
    /// <see cref="Enum"/> に関する拡張メソッドを提供するクラス。
    /// 主に表示用の文字列取得など、UI 向けの用途を想定している。
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Enum に設定された <see cref="DisplayAttribute"/> の Name を取得する。
        /// DisplayAttribute が設定されていない場合は、Enum の名前をそのまま返す。
        /// </summary>
        /// <param name="value">対象の Enum 値</param>
        /// <returns>
        /// DisplayAttribute.Name が存在する場合はその値。
        /// 存在しない場合は <c>Enum.ToString()</c> の結果。
        /// </returns>
        public static string GetDisplayName(this Enum value)
        {
            // Enum 型から、該当するフィールド情報を取得
            var field = value.GetType().GetField(value.ToString());

            // フィールドに設定されている DisplayAttribute を取得
            var attr = field?.GetCustomAttribute<DisplayAttribute>();

            // DisplayAttribute があれば Name を、なければ Enum 名を返す
            return attr?.Name ?? value.ToString();
        }
    }
}
