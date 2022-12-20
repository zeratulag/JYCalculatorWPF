using System.Collections.Generic;
using System.Linq;
using JYCalculator.Src;

namespace JX3CalculatorShared.ViewModels
{
    public class OptimizationViewModel : CheckBoxViewModel
    {
        public bool IsChecked { get; set; } = true;

        public string OptimizationDesc { get; set; }

        public string OptimizationDescSource;
        public IEnumerable<MultiZhenRes> MultiZhenResSource;

        public MultiZhenRes[] MultiZhenTable { set; get; }

        protected override void _Update()
        {
            OptimizationDesc = IsChecked ? OptimizationDescSource : "";
            MultiZhenTable = IsChecked ? MultiZhenResSource.ToArray() : null;
        }

        protected override void _Load<TSave>(TSave sav)
        {
        }

        protected override void _RefreshCommands()
        {
        }


        public void UpdateSource(CalculatorShell shell)
        {
            if (IsChecked)
            {
                OptimizationDescSource = shell.DpsKernelOp.BestProportion.Desc;
                MultiZhenResSource = shell.MultiZhenDPSOp.ResultArr;
            }
            else
            {
                OptimizationDescSource = "";
                MultiZhenResSource = null;
            }

            Refresh();
        }

        public void Refresh()
        {
            _Update();
        }
    }
}