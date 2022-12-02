using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Src.Data;
using JYCalculator.Src.DB;
using System.Collections.Generic;
using System.Collections.Immutable;


namespace JYCalculator.ViewModels
{
    public class ItemDTConfigViewModel : ItemDTConfigViewModelBase
    {

        #region 成员

        #endregion

        #region 构造

        public ItemDTConfigViewModel(ItemDTDB db)
        {
            ItemDTSlotDict = db.TypeData.ToImmutableDictionary(
                kvp => kvp.Key,
                kvp => new ItemDTSlotViewModel(kvp.Value));
            Data = ItemDTSlotDict.Values.ToImmutableArray();
            ValidItemDts = new List<ItemDT>(8);

            ItemsSource = ItemDTSlotDict.Values.ToImmutableArray();

            ExtendInputNames(nameof(IsChecked));
            AttachDependVMsOutputChanged();
            PostConstructor();
            PropertyChanged += OnIsCheckedChanged;
            _Update();
        }

        public ItemDTConfigViewModel() : this(StaticJYData.DB.ItemDT)
        {

        }

        #endregion

        #region 方法

        #endregion


        #region 导入导出

        #endregion
    }
}