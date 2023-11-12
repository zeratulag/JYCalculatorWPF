using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Src;
using JYCalculator.Class;
using JYCalculator.Src;
using JYCalculator.ViewModels;
using System.Collections.Generic;
using JYCalculator.Globals;
using JX3PZ.Models;

namespace JYCalculator.Models
{
    public class MainWindowModel : IModel
    {
        #region 成员

        // 输入成员
        public readonly MainWindowViewModels _VMs;

        public readonly AllSkillMiJiConfigViewModel SkillMiJiVM;
        public readonly QiXueConfigViewModel QiXueVM;
        public readonly FightOptionConfigViewModel FightOptionVM;
        public readonly ItemDTConfigViewModel ItemDTVM;
        public readonly AllBuffConfigViewModel BuffVM;

        public readonly InitInputViewModel InitInputVM;
        public readonly InitCharacter InitChar;
        public readonly EquipOptionConfigViewModel EquipOptionVM;

        public readonly FightTimeSummaryViewModel FightTimeSummaryVM;
        public readonly SkillDataDFViewModel SkillDataDFMiJiQiXueVM;

        // 输出成员

        public readonly Dictionary<string, NamedAttrs> AllAttrsDict;

        public CalculatorShell CalcShell;
        public CalculatorShellArg CalcShellArg;

        public BuffShellInput Shellbuffs;

        public FullCharacterGroup FullCharGroup;

        public readonly List<NamedAttrs> AllAttrs;

        public int CalcShellStatus = 0; // 表示计算是否成功

        #endregion 构建

        public MainWindowModel(MainWindowViewModels vm)
        {
            _VMs = vm;
            SkillMiJiVM = vm.SkillMiJiVM;
            QiXueVM = vm.QiXueVM;
            FightOptionVM = vm.FightOptionVM;
            ItemDTVM = vm.ItemDTVM;
            BuffVM = vm.BuffVM;
            InitInputVM = vm.InitInputVM;
            InitChar = vm.InitCharVM;
            EquipOptionVM = vm.EquipOptionVM;
            FightTimeSummaryVM = vm.FightTimeSummaryVM;
            SkillDataDFMiJiQiXueVM = vm.SkillDFMiJiQiXueVM;

            CalcShell = new CalculatorShell(this);
            CalcShellArg = new CalculatorShellArg();

            AllAttrsDict = CalcShell.AllAttrsDict;

            AllAttrs = new List<NamedAttrs>(8);
        }


        public void Calc()
        {
            var res = UpdateCalcShell();
            CalcShell = res;
            CalcAllAttrs();
            UpdateResult();
            CalcShellStatus = res.CalcStatus;
        }

        protected CalculatorShell UpdateCalcShell()
        {
            Shellbuffs = new BuffShellInput(BuffVM.BuffAttrsDesc, BuffVM.DebuffAttrsDesc, ItemDTVM.AttrsDesc,
                BuffVM.Arg);
            CalcShellArg = new CalculatorShellArg(_VMs.SkillMiJiVM.IsSupport, (int) InitChar.HS,
                _VMs.OptimizationVM.IsChecked, InitInputVM.BigFMVM.Arg, BuffVM.Arg);
            var calcShell = new CalculatorShell(this);
            calcShell.UpdateInput(this);
            var res = calcShell.Calc();
            return res;
        }

        protected void CalcAllAttrs()
        {
            AllAttrs.Clear();
            foreach (var KVP in AllAttrsDict)
            {
                if (KVP.Value != null && KVP.Value.Attr != null)
                {
                    AllAttrs.Add(KVP.Value);
                }
            }
        }

        public void UpdateResult()
        {
        }

        #region 方法

        public MainWindowSav Export()
        {
            var res = new MainWindowSav()
            {
                InitInput = InitInputVM.Export(),
                SkillMiJi = SkillMiJiVM.Export(),
                QiXue = QiXueVM.Export(),
                Buff = BuffVM.Export(),
                ItemDT = ItemDTVM.Export(),
                FightOption = FightOptionVM.Export(),
                AppMeta = Globals.XFAppStatic.CurrentAppMeta,
                SnapFinalDPS = CalcShell.CDPSKernel.FinalDPS,
                IsSync = GlobalContext.IsPZSyncWithCalc,
            };

            var pzSav = GlobalContext.ViewModels.PzMain?.Export();
            if (pzSav != null)
            {
                pzSav.IsSync = GlobalContext.IsPZSyncWithCalc;
            }

            res.PzSav = pzSav;

            return res;
        }

        public void _Load(MainWindowSav sav)
        {
            InitInputVM.Load(sav.InitInput);
            SkillMiJiVM.Load(sav.SkillMiJi);
            QiXueVM.Load(sav.QiXue);
            BuffVM.Load(sav.Buff);
            ItemDTVM.Load(sav.ItemDT);
            FightOptionVM.Load(sav.FightOption);

            GlobalContext.IsPZSyncWithCalc = sav.IsSync;

            if (sav.IsSync)
            {
                GlobalContext.ViewModels.PzMain.Load(sav.PzSav);
            }
        }

        #endregion
    }
}