using System.Collections.Immutable;
using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Src.Class;
using JYCalculator.Src.Data;
using JYCalculator.Src.DB;


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

        public int FightTime { get; set; } = 300;
        public bool ShortFight { get; set; }

        #endregion

        #region 构造

        public FightOptionConfigViewModel(JYDataBase tldb)
        {
            TargetViewModel = new TargetViewModel(tldb.Target.Target);
            AbilityViewModel = new AbilityViewModel(tldb.Ability.AbilityItem);
            ZhenFaViewModel = new ZhenFaViewModel(tldb.ZhenFa.ZhenFa);

            var data = new AbsViewModel[] { TargetViewModel, AbilityViewModel, ZhenFaViewModel };
            Data = data.ToImmutableArray();

            ExtendInputNames(nameof(FightTime), nameof(ShortFight));
            AttachDependVMsOutputChanged();
            PostConstructor();
            _Update();

        }

        public FightOptionConfigViewModel() : this(StaticJYData.DB)
        {

        }

        #endregion

        #region 方法

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
        }

        public void Load(FightOptionSav sav)
        {
            ActionUpdateOnce(_Load, sav);
        }

        #endregion

    }
}