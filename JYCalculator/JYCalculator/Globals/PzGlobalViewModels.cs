using J3PZ.Views;
using JX3PZ.ViewModels;
using JX3PZ.ViewModels.PzOverview;
using JX3PZ.Views;

namespace JX3PZ.Globals
{
    public class PzGlobalViewModels
    {
        public PzMainWindowViewModels PzMain;
        public PzOverviewWindowViewModel PzOverview;

        public PzGlobalViewModels()
        {
        }
    }

    public class PzGlobalViews
    {
        public PzMainWindow PzMain;
        public PzOverviewWindow PzOverview;

        public PzGlobalViews()
        {
        }
    }

    public static class PzGlobalContext
    {
        // 主要View和ViewModel的集合
        public static readonly PzGlobalViewModels ViewModels;
        public static readonly PzGlobalViews Views;

        static PzGlobalContext()
        {
            ViewModels = new PzGlobalViewModels();
            Views = new PzGlobalViews();
        }
    }
}