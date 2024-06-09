using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JX3CalculatorShared.Class;
using JYCalculator.Messages;
using System.Windows;

namespace JX3CalculatorShared.ViewModels
{
    public class CalcResultViewModel : ObservableObject
    {
        public bool HasResult { get; private set; } = false;
        public string FinalDPSTxt { get; private set; } = string.Empty;
        public string ProfitOrderDesc { get; private set; } = string.Empty;
        public string FinalDPSToolTip { get; private set; } = "";
        public string ProfitOrderDescToolTip { get; private set; } = "";
        public RelayCommand CopyCalcResultCmd { get; }

        public double FinalDPS;

        public CalcResultViewModel()
        {
            CopyCalcResultCmd = new RelayCommand(CopyCalcResult);
            FinalDPSToolTip = "DPS期望值";
            ProfitOrderDescToolTip = "属性收益顺序";
        }

        private void CopyCalcResult()
        {
            var res = $"{FinalDPS:F2}\t{ProfitOrderDesc}";
            Clipboard.SetText(res);
            //Growl.Info("已复制");
        }

        // 更新结果
        public void UpdateResult(bool hasResult, double finalDPS, string profitOrderDesc)
        {
            HasResult = hasResult;
            ProfitOrderDesc = profitOrderDesc;
            FinalDPS = finalDPS;
            FinalDPSTxt = FinalDPS.ToString("F0");
        }

        public void UpdateResult(CalcResult result)
        {
            UpdateResult(result.Success, result.FinalDPS, result.ProfitOrderDesc);
        }

        public void UpdateFrom(CalcResultMessage message)
        {
            UpdateResult(message.Result);
        }
    }
}