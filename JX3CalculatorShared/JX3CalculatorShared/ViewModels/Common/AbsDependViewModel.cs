using PropertyChanged;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace JX3CalculatorShared.ViewModels
{
    public abstract class AbsDependViewModel<TViewModel> : AbsViewModel where TViewModel : AbsViewModel
    {
        #region 成语

        [DoNotNotify]
        public ImmutableArray<TViewModel> _DependVMs { get; protected set; } // 存储了依赖项的VM

        protected bool _AllDependInitialized = false;
        protected bool _FullInitialized = false;

        #endregion

        #region 构造

        protected AbsDependViewModel(IEnumerable<string> inputPropertyNames, IEnumerable<TViewModel> dependVMs) :
            base(inputPropertyNames)
        {
            SetDependVMs(dependVMs);
        }

        protected AbsDependViewModel(InputPropertyNameType nameType, IEnumerable<TViewModel> dependVMs) :
            base(nameType)
        {
            SetDependVMs(dependVMs);
        }

        protected AbsDependViewModel(IEnumerable<string> inputPropertyNames, params TViewModel[] vms) :
            this(inputPropertyNames, dependVMs: vms)
        {
        }

        protected AbsDependViewModel(InputPropertyNameType nameType, params TViewModel[] vms) :
            this(nameType, dependVMs: vms)
        {
        }

        /// <summary>
        /// 设定这些VM为依赖项
        /// </summary>
        /// <param name="dependVMs"></param>
        protected void SetDependVMs(IEnumerable<TViewModel> dependVMs)
        {
            _DependVMs = dependVMs.ToImmutableArray();
            AttachDependVMsOutputChanged();
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
            AttachDependVMsOutputChanged(_DependVMs);
        }

        #endregion

        #region 方法

        #endregion
    }

}