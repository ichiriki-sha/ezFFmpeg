using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ezFFmpeg.Models.Common
{
    /// <summary>
    /// ViewModelの基底クラス。
    /// INotifyPropertyChanged を実装し、プロパティ変更通知を提供します。
    /// MVVMパターンで利用されることを想定。
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティ変更通知イベント。
        /// プロパティの値が変更されたときにUIに通知されます。
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// プロパティが変更されたことを通知します。
        /// CallerMemberName属性により呼び出し元プロパティ名を自動取得可能。
        /// </summary>
        /// <param name="propertyName">変更されたプロパティ名</param>
        protected void RaisePropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// プロパティ値を設定し、変更があれば通知します。
        /// 変更がなければUI通知を行わず、パフォーマンス向上に寄与します。
        /// </summary>
        /// <typeparam name="T">プロパティの型</typeparam>
        /// <param name="field">バックフィールドへの参照</param>
        /// <param name="value">設定する新しい値</param>
        /// <param name="propertyName">プロパティ名（省略可能）</param>
        /// <returns>値が変更された場合は true、変更なしの場合は false</returns>
        protected bool SetProperty<T>(
            ref T field,
            T value,
            [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
                return false;

            field = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}
