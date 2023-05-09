using PropertyChanged;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Syncfusion.Data.Extensions;

namespace JX3CalculatorShared.ViewModels
{
    public abstract class CollectionViewModel<TViewModel> : AbsViewModel where TViewModel : AbsViewModel
    {
        // 用于描述成员不变的集合VM
        #region 成语

        [DoNotNotify]
        public ImmutableArray<TViewModel> Data { get; protected set; }

        #endregion

        #region 构造

        protected CollectionViewModel(IEnumerable<TViewModel> vMs)
        {
            Data = vMs.ToImmutableArray();
            AttachDependVMsOutputChanged();
        }

        protected CollectionViewModel()
        {
        }

        /// <summary>
        /// 捕捉data中每一个输出元素变化事件
        /// </summary>
        /// <param name="data">集合</param>
        protected void AttachDependVMsOutputChanged(IEnumerable<AbsViewModel> data) // 捕捉集合中每一个对象的属性改动
        {
            foreach (var item in data)
            {
                item.OutputChanged += RaiseInputChanged;
            }
        }

        /// <summary>
        /// 捕捉data中每一个输出元素变化事件
        /// </summary>
        /// <param name="data">集合</param>
        protected void AttachDependVMsOutputChanged() // 捕捉集合中每一个对象的属性改动
        {
            AttachDependVMsOutputChanged(Data);
        }

        #endregion

        #region 方法

        protected override void _Update()
        {
        }

        protected override void _RefreshCommands()
        {
        }

        #endregion
    }

}