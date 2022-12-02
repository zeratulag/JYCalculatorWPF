using System.Collections.Generic;
using System.Linq;

namespace JX3CalculatorShared.ViewModels
{

    public class ComboBoxViewModel<TItem> : AbsViewModel
    {
        /// <summary>
        /// 通用下拉框ViewModel
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        #region 成员

        public int Length => ItemsSource.Length;
        public virtual TItem SelectedItem => ItemsSource[SelectedIndex];
        public TItem[] ItemsSource { get; protected set; }
        public int SelectedIndex { get; set; }

        #endregion

        #region 构造

        protected ComboBoxViewModel(IEnumerable<TItem> data) : base(nameof(SelectedIndex))
        {
            ItemsSource = data.ToArray();
            SelectedIndex = 0;
        }

        protected ComboBoxViewModel() : base(nameof(SelectedIndex))
        {
            SelectedIndex = 0;
        }

        #endregion

        #region 方法

        #endregion

        protected override void _Update()
        {
        }

        protected override void _Load<TSave>(TSave sav)
        {
        }

        protected override void _RefreshCommands()
        {
        }
    }
}