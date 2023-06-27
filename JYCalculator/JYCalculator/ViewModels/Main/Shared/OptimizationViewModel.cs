using JX3CalculatorShared.ViewModels;
using JYCalculator.Src;

namespace JYCalculator.ViewModels
{
    public class OptimizationViewModel : OptimizationViewModelBase
    {
        public void UpdateSource(CalculatorShell shell)
        {
            OptimizationDescSource = shell.DpsKernelOp.BestProportion.Desc;
            MultiZhenResSource = shell.MultiZhenDPSOp.ResultArr;
            Refresh();
        }
    }
}