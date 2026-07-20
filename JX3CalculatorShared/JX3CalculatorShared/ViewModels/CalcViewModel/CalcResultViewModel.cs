using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Utils;
using JYCalculator.Messages;
using System.Windows;
using HandyControl.Controls;

namespace JX3CalculatorShared.ViewModels
{
    public class CalcResultViewModel : ObservableObject
    {
        public double PreviousDPS { get; private set; }
        public double FinalDPS { get; private set; }
        public double DeltaDPSPct { get; private set; } // 相对改变量
        public bool HasResult { get; private set; } = false;
        public string ProfitOrderDesc { get; private set; } = string.Empty;
        public RelayCommand CopyCalcResultCmd { get; }
        public bool ShowDelta { get; private set; } = false; // 是否显示改变量

        public string DeltaColor => DeltaDisplayTool.GetPercentColor(DeltaDPSPct);

        public CalcResultViewModel()
        {
            CopyCalcResultCmd = new RelayCommand(CopyCalcResult);
        }

        private void CopyCalcResult()
        {
            var res = $"{FinalDPS:F2}\t{ProfitOrderDesc}";
            int retry = 3;
            while (retry-- > 0)
            {
                try
                {
                    Clipboard.SetText(res);
                    //Growl.Info("已复制");
                    return;
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    if (retry == 0)
                    {
                        //MessageBox.Show("复制失败，剪贴板被占用，请稍后重试。");
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
        }

        // 更新结果
        public void UpdateResult(bool hasResult, double finalDPS, string profitOrderDesc)
        {
            HasResult = hasResult;
            ProfitOrderDesc = profitOrderDesc;
            PreviousDPS = FinalDPS;
            FinalDPS = finalDPS;
            if (PreviousDPS > 0)
            {
                ShowDelta = true;
                DeltaDPSPct = (FinalDPS - PreviousDPS) / PreviousDPS;
            }
        }

        public void UpdateResult(CalcResultBase result)
        {
            UpdateResult(result.Success, result.FinalDPS, result.ProfitOrderDesc);
        }

        public void UpdateFrom(CalcResultMessage message)
        {
            UpdateResult(message.Result);
        }
    }
}
