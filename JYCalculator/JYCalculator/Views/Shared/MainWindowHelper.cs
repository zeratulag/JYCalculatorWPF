using JX3CalculatorShared.Views;
using System.Collections.Immutable;
using System.Linq;
using System.Windows.Controls;

namespace JYCalculator.Views
{
    public class MainWindowHelper
    {
        #region 成员

        private readonly MainWindow _MW;

        public ImmutableDictionary<string, ComboBox> BigFMcbbDict { get; private set; }
        public ImmutableDictionary<string, CheckBox> BigFMchbDict { get; private set; }
        public ImmutableDictionary<string, ComboBox> QiXueDict { get; private set; }
        public ImmutableDictionary<string, ComboBox> ItemDTDict { get; private set; }
        public ImmutableDictionary<string, Expander> MiJiExpanderDict { get; private set; }
        public ImmutableDictionary<string, ListView> MiJiListViewDict { get; private set; }

        #endregion

        #region 构造

        public MainWindowHelper(MainWindow mw)
        {
            _MW = mw;
            Proceed();
        }

        public void Proceed()
        {
            FindMiJis();
        }

        #endregion

        #region 方法

        public void FindMiJis()
        {
            MiJiExpanderDict = _MW.Grid_MiJi.FindChildrenElements<Expander>(
                prefix: ViewGlobals.PREFIX.ExpanderMiJi).ToImmutableDictionary();

            var MiJilistViews = MiJiExpanderDict.Values.Select(e => (ListView)e.Content);
            MiJiListViewDict = MiJilistViews.ToImmutableDictionary(e => e.Name);
        }

        #endregion
    }
}