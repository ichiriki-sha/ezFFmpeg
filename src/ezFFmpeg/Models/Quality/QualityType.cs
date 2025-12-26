using System;
using System.Collections.Generic;
using System.Text;

namespace ezFFmpeg.Models.Quality
{
    /// <summary>
    /// エンコード時に使用する品質指定の方式を表す列挙型。
    /// 映像や音声の圧縮・画質制御におけるパラメータ種別を示す。
    /// </summary>
    public enum QualityType
    {
        /// <summary>
        /// 品質指定なし（デフォルト）。
        /// </summary>
        None,

        /// <summary>
        /// CRF（Constant Rate Factor）方式。
        /// -crf オプションで使用され、可変ビットレートで画質を一定に保つ。
        /// </summary>
        Crf,

        /// <summary>
        /// CQ（Constant Quantizer）方式。
        /// -cq オプションで使用され、品質を固定してエンコード。
        /// </summary>
        Cq,

        /// <summary>
        /// QP（Quantization Parameter）方式。
        /// -qp オプションで使用される将来向けの方式。画質とビットレートを直接制御。
        /// </summary>
        Qp
    }
}
