using JX3CalculatorShared.Class;
using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Utils;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Models;
using JYCalculator.Src.Data;
using JYCalculator.Src.DB;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;


namespace JYCalculator.ViewModels
{
    public class EquipOptionConfigViewModel: CollectionViewModel<EquipOptionSlotViewModel>
    {
        #region 成员
        
        public readonly EquipOptionSlotViewModel WPViewModel;
        public readonly EquipOptionSlotViewModel YZViewModel;

        public bool JN { get; set; } // 套装技能增强
        public bool SL { get; set; } // 套装神力效果

        public WPOption SelectedWP => (WPOption)WPViewModel.SelectedItem;
        public YZOption SelectedYZ => (YZOption)YZViewModel.SelectedItem; // 当前腰坠选项

        public readonly EquipOptionConfigModel Model;

        public readonly List<Recipe> EquipRecipes;

        public NamedAttrs AttrsDesc;
        #endregion

        #region 构造

        public EquipOptionConfigViewModel(EquipOptionDB db)
        {
            WPViewModel = new EquipOptionSlotViewModel(db.WP);
            YZViewModel = new EquipOptionSlotViewModel(db.YZ);

            var data = new EquipOptionSlotViewModel[2] { WPViewModel, YZViewModel };
            Data = data.ToImmutableArray();
            ExtendInputNames(nameof(JN), nameof(SL));

            Model = new EquipOptionConfigModel();
            EquipRecipes = Model.OtherRecipes;

            AttachDependVMsOutputChanged();
            PostConstructor();
            _Update();
        }

        public EquipOptionConfigViewModel() : this(StaticJYData.DB.EquipOption)
        {
        }

        #endregion


        #region 方法

        protected override void _Update()
        {
            Model.UpdateInput(this);
            AttrsDesc = SelectedWP.SCharAttr.ToNamed(SelectedWP.ItemName);
            AttrsDesc.ParcelName("武器：");
        }

        protected override void _DEBUG()
        {
            var recipenames = from _ in EquipRecipes select _.RecipeName;
            Trace.WriteLine("装备提供的秘籍包括：");
            recipenames.TraceCat();
        }

        public EquipOptionConfigSav Export()
        {
            var res = new EquipOptionConfigSav(JN, SL, SelectedWP.Name, SelectedYZ.Name);
            return res;
        }

        protected void _Load(EquipOptionConfigSav sav)
        {
            JN = sav.JN;
            SL = sav.SL;
            WPViewModel.Load(sav.WPName);
            YZViewModel.Load(sav.YZName);
        }

        public void Load(EquipOptionConfigSav sav)
        {
            ActionUpdateOnce(_Load, sav);
        }

        #endregion
    }
}