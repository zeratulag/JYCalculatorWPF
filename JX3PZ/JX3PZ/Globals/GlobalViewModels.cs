using System.Collections.Immutable;
using JX3CalculatorShared.Globals;
using JX3PZ.Globals;
using JX3PZ.ViewModels;
using JX3PZ.Views;
using JYCalculator.ViewModels;
using JYCalculator.Views;

namespace JYCalculator.Globals
{
    public class GlobalViewModels
    {
        public MainWindowViewModels Main;
        public DebugWindowViewModel Debug;
        public PzMainWindowViewModels PzMain => PzGlobalContext.ViewModels.PzMain;

        public GlobalViewModels()
        {
        }
    }

    public class GlobalViews
    {
        public MainWindow Main;
        public DebugMainWindow Debug;
        public PzMainWindow PzMain => PzGlobalContext.Views.PzMain;

        public GlobalViews()
        {
        }
    }

    public static class GlobalContext
    {
        // 主要View和ViewModel的集合
        public static readonly GlobalViewModels ViewModels;
        public static readonly GlobalViews Views;

        public static bool IsPZSyncWithCalc
        {
            get => ViewModels.Main?.IsPZSyncWithCalc ?? false;
            set
            {
                if (ViewModels.Main != null)
                {
                    ViewModels.Main.IsPZSyncWithCalc = value;
                }
            }
        }

        public static ImmutableArray<EquipSubTypeEnum> BigFMSlots; // 大附魔顺序

        static GlobalContext()
        {
            ViewModels = new GlobalViewModels();
            Views = new GlobalViews();
        }
    }
}