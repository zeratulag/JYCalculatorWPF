using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Messages;
using JX3CalculatorShared.ViewModels;
using JX3CalculatorShared.Views;
using JX3PZ.Globals;
using JX3PZ.Messages;
using JYCalculator.Globals;
using JYCalculator.Messages;
using Syncfusion.Windows.Shared;

namespace JX3PZ.ViewModels.PzOverview
{
    public class PzOverviewViewModel : ObservableObject, IRecipient<CalcResultMessage>, IRecipient<PzPlanMessage>
    {
        public string Title { get; set; } = "默认标题"; // 标题
        public int Score { get; private set; } = 0; // 装分
        public string Author { get; set; } = "作者"; // 配装作者
        public double FinalDPS { get; set; } = 0; // 最终DPS
        public bool ShowDPS { get; set; } = true;
        public PzEquipSummaryViewModel[] Summary { get; private set; } // 复用配装信息
        public PzAttributeSlotsViewModelBase[] AttributeSlots { get; private set; }
        public CalcInputViewModel CalcInputVM { get; }

        private bool _SendingInfoMessage = false;
        public string Info { get; }
        public string Url { get; }
        public RelayCommand ConfirmInfoCmd { get; }

        public PzOverviewViewModel()
        {
            CalcInputVM = new CalcInputViewModel();
            Info = $"{AppStatic.XinFa}配装计算器 v{XFAppStatic.AppVersion}";
            Url = XFAppStatic.JX3BOXURL;
        }

        public void Receive(CalcResultMessage message)
        {
            if (GlobalContext.IsPZSyncWithCalc)
            {
                UpdateFromCalcResult(message.Result);
                CalcInputVM.UpdateFrom(message);
            }
        }


        private void UpdateFromCalcResult(CalcResult result)
        {
            FinalDPS = result.FinalDPS;
        }

        public void Receive(PzPlanMessage message)
        {
            Title = message.Title;
            Author = message.Author;
            Score = message.Plan.CEquipScore.TotalScore;
            var vm = PzGlobalContext.ViewModels.PzMain;
            Summary = vm.GetEquipSummaryViewModels();
            AttributeSlots = vm.PzResultVM.GetAttributeSlots();
        }

        public void OnTitleChanged()
        {
            SendInfoMessageIfNecessary();
        }

        public void OnAuthorChanged()
        {
            SendInfoMessageIfNecessary();
        }

        public void SendInfoMessageIfNecessary()
        {
            if (!_SendingInfoMessage)
            {
                _SendingInfoMessage = true;
                var msg = new PzInfoMessage(Title, Author);
                WeakReferenceMessenger.Default.Send(msg);
                _SendingInfoMessage = false;
            }
        }

        private void ConfirmInfo()
        {
            SendInfoMessageIfNecessary();
        }
    }

}