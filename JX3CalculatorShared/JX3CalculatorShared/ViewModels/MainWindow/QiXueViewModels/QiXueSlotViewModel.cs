using JX3CalculatorShared.Class;
using PropertyChanged;
using System.Collections.Generic;

namespace JX3CalculatorShared.ViewModels
{
    /// <summary>
    /// 用于描述奇穴槽的类
    /// </summary>
    public class QiXueSlotViewModel : ComboBoxViewModel<QiXueSkill>
    {
        #region 成员

        public readonly int Position; // 奇穴重数，1~12

        [DoNotNotify]
        public int Order // 奇穴层数，1~j
        {
            get => SelectedIndex + 1;
            set => SelectedIndex = value - 1;
        }

        [DoNotNotify]
        public string ShortName => SelectedItem.ShortName;

        [DoNotNotify]
        public string ItemName => SelectedItem.ItemName;

        #endregion

        #region 构造

        public QiXueSlotViewModel(IEnumerable<QiXueSkill> data) : base(data)
        {
            Position = ItemsSource[0].Position;
        }


        #endregion
        public void LoadOrder(int order)
        {
            Order = order;
        }

    }
}