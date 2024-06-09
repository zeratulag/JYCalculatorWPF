using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using JX3CalculatorShared.Messages;
using JX3PZ.Globals;
using JYCalculator.Messages;

namespace JX3PZ.ViewModels.PzOverview
{
    public class PzOverviewWindowViewModel : ObservableObject, IRecipient<CalcResultMessage>, IRecipient<PzPlanMessage>
    {
        public PzOverviewViewModel OverviewVM { get; }

        public PzOverviewWindowViewModel()
        {
            OverviewVM = new PzOverviewViewModel();
            // 注册信息接收处理
            WeakReferenceMessenger.Default.Register<CalcResultMessage>(this);
            WeakReferenceMessenger.Default.Register<PzPlanMessage>(this);

            PzGlobalContext.ViewModels.PzOverview = this;
        }

        public void Receive(CalcResultMessage message)
        {
            OverviewVM.Receive(message);
        }

        public void Receive(PzPlanMessage message)
        {
            OverviewVM.Receive(message);
        }

        public void SetPzTitle(string title)
        {
            OverviewVM.Title = title;
        }
    }
}