using JX3CalculatorShared.Class;
using JX3CalculatorShared.Utils;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Data;
using JYCalculator.DB;
using JYCalculator.Models;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;


namespace JYCalculator.ViewModels
{
    public class EquipOptionConfigViewModel : CollectionViewModel<EquipOptionSlotViewModel>
    {
        #region 成员

        public EquipOptionSlotViewModel WPViewModel { get; }
        public EquipOptionSlotViewModel YZViewModel { get; }

        public bool JN { get; set; } // 套装技能增强
        public bool SL { get; set; } // 套装神力效果

        public string JNToolTip { get; set; } = "“夺魄箭”伤害提高10%,“暴雨梨花针”伤害提高10%";
        public string SLToolTip { get; set; } = "施展外功伤害招式，一定几率提高自身外功会心几率4%，会心效果4%，持续6秒。";

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

        public EquipOptionConfigViewModel() : this(StaticXFData.DB.EquipOption)
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