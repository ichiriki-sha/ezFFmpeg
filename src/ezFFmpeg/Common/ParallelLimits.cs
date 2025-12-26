using System;

namespace ezFFmpeg.Common
{
    /// <summary>
    /// 変換処理（主に FFmpeg 実行）における
    /// 並列実行数の上限・下限および既定値を定義するクラス。
    /// 
    /// CPU 使用率の暴走を防ぎつつ、
    /// ユーザー操作や UI 応答性を維持することを目的としている。
    /// </summary>
    public static class ConversionParallelLimits
    {
        /// <summary>
        /// 並列実行数の既定値。
        /// 
        /// CPU コア数の 1/3 を使用することで、
        /// システム負荷と処理速度のバランスを取る。
        /// 最低でも 1 スレッドは確保される。
        /// </summary>
        public static readonly int Default =
            Math.Max(1, Environment.ProcessorCount / 3);

        /// <summary>
        /// 許可される並列実行数の最小値。
        /// </summary>
        public static readonly int Min = 1;

        /// <summary>
        /// 許可される並列実行数の最大値。
        /// 
        /// CPU コア数の 1/2 を上限とし、
        /// OS や他アプリケーションの動作余地を残す。
        /// </summary>
        public static readonly int Max =
            Math.Max(1, Environment.ProcessorCount / 2);

        /// <summary>
        /// 指定された並列実行数が、
        /// 許可範囲内であるかを判定する。
        /// </summary>
        /// <param name="parallel">検証対象の並列数</param>
        /// <returns>
        /// 有効な範囲内であれば true、それ以外は false
        /// </returns>
        public static bool IsValid(int parallel)
        {
            return parallel >= Min && parallel <= Max;
        }
    }

    /// <summary>
    /// 内部処理用の並列数定義クラス。
    /// 
    /// UI 設定やユーザー指定とは切り離し、
    /// 内部処理（解析・一時処理・補助タスクなど）で
    /// 最大限の CPU リソースを使用したい場合に利用する。
    /// </summary>
    public static class InternalParallel
    {
        /// <summary>
        /// 内部処理用の既定並列数。
        /// 
        /// CPU の論理コア数をそのまま使用する。
        /// </summary>
        public static int Default => Environment.ProcessorCount;
    }
}
