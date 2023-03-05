using JX3CalculatorShared.ViewModels;
using JYCalculator.Src;

namespace JYCalculator.ViewModels
{
    public class OptimizationViewModel : OptimizationViewModelBase
    {
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
    }
}