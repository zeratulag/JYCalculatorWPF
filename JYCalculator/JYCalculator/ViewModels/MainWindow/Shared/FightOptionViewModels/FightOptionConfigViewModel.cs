using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Data;
using JYCalculator.DB;
using System.Collections.Immutable;


namespace JYCalculator.ViewModels
{
    public class FightOptionConfigViewModel : CollectionViewModel<AbsViewModel>
    {
        #region 成员

        public TargetViewModel TargetVM { get; }
        public AbilityViewModel AbilityVM { get; }
        public ZhenFaViewModel ZhenFaVM { get; }

        public Target SelectedTarget => TargetVM.SelectedItem;
        public AbilityItem SelectedAbility => AbilityVM.SelectedItem;
        public ZhenFa SelectedZhenFa => ZhenFaVM.SelectedItem;

        public int FightTime { get; set; }
        public bool ShortFight { get; set; } = true;
        public bool LongFight { get; set; }
        public bool TargetAllWaysFullHP { get; set; } = false;

        public FightOptionSummaryViewModel Summary;
        #endregion

        #region 构造

        public FightOptionConfigViewModel(XFDataBase xfdb)
        {
            TargetVM = new TargetViewModel(xfdb.Target.Target);
            AbilityVM = new AbilityViewModel(xfdb.Ability.AbilityItem);
            ZhenFaVM = new ZhenFaViewModel(xfdb.ZhenFa.ZhenFa);

            FightTime = GetDefaultFightTime();

            var data = new AbsViewModel[] { TargetVM, AbilityVM, ZhenFaVM };
            Data = data.ToImmutableArray();

            ExtendInputNames(nameof(FightTime), nameof(ShortFight), nameof(TargetAllWaysFullHP));
            AttachDependVMsOutputChanged();
            PostConstructor();
            _Update();

        }

        public FightOptionConfigViewModel() : this(StaticXFData.DB)
        {

        }

        public static int GetDefaultFightTime()
        {
            int res = 0;
            switch (AppStatic.XinFaTag)
            {
                case "JY":
                    {
                        res = 300;
                        break;
                    }
                case "TL":
                    {
                        res = 240;
                        break;
                    }
            }
            return res;
        }
        #endregion

        #region 方法
        protected void OnFightTimeChanged()
        {
            // 通过生成器实现
            StaticMessager.Send(StaticMessager.FightTimeChangedMsg);
        }

        protected override void _Update()
        {
            Summary = ToSummary();
        }

        #endregion

        #region 导入导出

        public FightOptionSav Export()
        {
            var res = new FightOptionSav()
            {
                ZhenFa = ZhenFaVM.Export(),
                Target = TargetVM.Export(),
                Ability = AbilityVM.Export(),
                FightTime = FightTime,
                ShortFight = ShortFight,
                TargetAllWaysFullHP = TargetAllWaysFullHP,
            };
            return res;
        }

        protected void _Load(FightOptionSav sav)
        {
            ZhenFaVM.Load(sav.ZhenFa);
            TargetVM.Load(sav.Target);
            AbilityVM.Load(sav.Ability);
            FightTime = sav.FightTime;
            ShortFight = sav.ShortFight;
            LongFight = !ShortFight;
            TargetAllWaysFullHP = TargetAllWaysFullHP;
        }

        public void Load(FightOptionSav sav)
        {
            ActionUpdateOnce(_Load, sav);
        }

        public FightOptionSummaryViewModel ToSummary()
        {
            var res = new FightOptionSummaryViewModel()
            {
                CAbility = SelectedAbility,
                CTarget = SelectedTarget,
                CZhenFa = SelectedZhenFa,
                FightTime = FightTime,
                ShortFight = ShortFight,
                TargetAllWaysFullHP = TargetAllWaysFullHP,
            };
            return res;
        }

        #endregion

    }


}