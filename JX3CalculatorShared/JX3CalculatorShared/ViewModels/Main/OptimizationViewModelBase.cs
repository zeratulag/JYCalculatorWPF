using System.Collections.Generic;
using System.Linq;

namespace JX3CalculatorShared.ViewModels
{
    public class OptimizationViewModelBase : CheckBoxViewModel
    {
        public string OptimizationDescSource;
        public IEnumerable<MultiZhenRes> MultiZhenResSource;
        public bool IsChecked { get; set; } = true;
        public string OptimizationDesc { get; set; }
        public MultiZhenRes[] MultiZhenTable { set; get; }

        protected override void _Update()
        {
            OptimizationDesc = IsChecked ? OptimizationDescSource : "";
            MultiZhenTable = IsChecked ? MultiZhenResSource.ToArray() : null;
        }

        protected override void _RefreshCommands()
        {
        }

        public void Refresh()
        {
            _Update();
        }
    }

    public class MultiZhenRes
    {
        public string Name { get; }
        public double DPS { get; }
        public double Relative { get; set; }

        public int IconID { get; }
        public double RelativePct => Relative * 100;
        public int Rank { get; set; }

        public MultiZhenRes(int iconID, double dps, string name)
        {
            IconID = iconID;
            Name = name;
            DPS = dps;
        }
    }
}