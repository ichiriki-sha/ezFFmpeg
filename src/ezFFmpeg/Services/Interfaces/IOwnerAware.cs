using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace ezFFmpeg.Services.Interfaces
{
    /// <summary>
    /// 所有者ウィンドウを持つことができるクラス向けのインターフェース。
    /// ダイアログや子ウィンドウが親ウィンドウを参照できるようにします。
    /// </summary>
    public interface IOwnerAware
    {
        /// <summary>
        /// このオブジェクトの親ウィンドウ (Owner)。
        /// ダイアログや子ウィンドウを表示する際に親ウィンドウを指定するのに使用します。
        /// </summary>
        Window? Owner { get; set; }
    }
}
