using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Data;
using JYCalculator.DB;
using System.Collections.Immutable;
using CommunityToolkit.Mvvm.Messaging;
using JX3CalculatorShared.Globals;
using JYCalculator.Globals;


namespace JYCalculator.ViewModels
{
    public class FightOptionConfigViewModel : CollectionViewModel<AbsViewModel>
    {
        #region 成员

        public readonly TargetViewModel TargetViewModel;
        public readonly AbilityViewModel AbilityViewModel;
        public readonly ZhenFaViewModel ZhenFaViewModel;

        public Target SelectedTarget => TargetViewModel.SelectedItem;
        public AbilityItem SelectedAbility => AbilityViewModel.SelectedItem;
        public ZhenFa SelectedZhenFa => ZhenFaViewModel.SelectedItem;

        public int FightTime { get; set; }
        public bool ShortFight { get; set; } = true;

        public bool LongFight { get; set; }

        #endregion

        #region 构造

        public FightOptionConfigViewModel(XFDataBase xfdb)
        {
            TargetViewModel = new TargetViewModel(xfdb.Target.Target);
            AbilityViewModel = new AbilityViewModel(xfdb.Ability.AbilityItem);
            ZhenFaViewModel = new ZhenFaViewModel(xfdb.ZhenFa.ZhenFa);

            FightTime = GetDefaultFightTime();

            var data = new AbsViewModel[] { TargetViewModel, AbilityViewModel, ZhenFaViewModel };
            Data = data.ToImmutableArray();

            ExtendInputNames(nameof(FightTime), nameof(ShortFight));
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
            switch (XFAppStatic.XinFaTag)
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

        #endregion

        #region 导入导出

        public FightOptionSav Export()
        {
            var res = new FightOptionSav()
            {
                ZhenFa = ZhenFaViewModel.Export(),
                Target = TargetViewModel.Export(),
                Ability = AbilityViewModel.Export(),
                FightTime = FightTime,
                ShortFight = ShortFight,
            };
            return res;
        }

        protected void _Load(FightOptionSav sav)
        {
            ZhenFaViewModel.Load(sav.ZhenFa);
            TargetViewModel.Load(sav.Target);
            AbilityViewModel.Load(sav.Ability);
            FightTime = sav.FightTime;
            ShortFight = sav.ShortFight;
            LongFight = !ShortFight;
        }

        public void Load(FightOptionSav sav)
        {
            ActionUpdateOnce(_Load, sav);
        }

        #endregion

    }
}