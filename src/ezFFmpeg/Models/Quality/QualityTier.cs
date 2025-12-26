using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ezFFmpeg.Models.Quality
{
    /// <summary>
    /// エンコード品質のティア（ランク）を表す列挙型。
    /// VeryHigh から Low までの 4 段階で、映像/音声の圧縮・画質の優先度を示す。
    /// </summary>
    public enum QualityTier
    {
        /// <summary>
        /// 最高品質。圧縮率を下げ、画質/音質を最大に優先。
        /// Display 属性は「最高品質」。
        /// </summary>
        [Display(Name = "最高品質")]
        VeryHigh,

        /// <summary>
        /// 高品質。画質/音質を高く保ちつつ、圧縮効率も考慮。
        /// Display 属性は「高品質」。
        /// </summary>
        [Display(Name = "高品質")]
        High,

        /// <summary>
        /// 標準品質。画質と圧縮のバランスを重視。
        /// Display 属性は「標準」。
        /// </summary>
        [Display(Name = "標準")]
        Medium,

        /// <summary>
        /// 高圧縮。ファイルサイズ削減を優先し、画質/音質は低め。
        /// Display 属性は「高圧縮」。
        /// </summary>
        [Display(Name = "高圧縮")]
        Low
    }
}
