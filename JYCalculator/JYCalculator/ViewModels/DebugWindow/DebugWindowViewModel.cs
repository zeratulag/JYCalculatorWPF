using System.Linq;

namespace JYCalculator.ViewModels
{
    public partial class DebugWindowViewModel
    {
        protected void GetCoverTable()
        {
            var buffCover = _MVMs.CalcShell.KernelShell.TriggerModel.BuffCover.Data.Values; // BUFF覆盖率
            var res = buffCover;
            CoverTable = res.Where(_ => _.IsUseful()).ToArray();
        }
    }
}