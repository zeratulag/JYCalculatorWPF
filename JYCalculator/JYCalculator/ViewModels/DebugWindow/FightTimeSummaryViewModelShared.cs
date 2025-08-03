using JX3CalculatorShared.Class;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Class;
using System.Linq;

namespace JYCalculator.ViewModels
{
    public class FightTimeSummaryViewModel : AbsDependViewModel<AbsViewModel>
    {
        #region 成员

        public readonly FightOptionConfigViewModel FightOption;
        public readonly QiXueConfigViewModel QiXue;

        public readonly FightTimeSummary Model;

        public double TotalTime { get; set; }
        public double XWCD { get; set; }
        public double XWDuration { get; set; }

        public FightTimeSummaryItem[] Data { get; set; }
        #endregion

        #region 构造

        public FightTimeSummaryViewModel(FightOptionConfigViewModel fight, QiXueConfigViewModel qixue) : base(InputPropertyNameType.None, fight, qixue)
        {
            FightOption = fight;
            QiXue = qixue;
            Model = new FightTimeSummary(this);
            Data = Model.Data;
            PostConstructor();
        }

        #endregion

        protected override void _Update()
        {
            TotalTime = FightOption.FightTime;
            XWCD = QiXue.Model.XWCD;
            XWDuration = QiXue.Model.XWDuration;
            Model.Update(this);
            Data = Model.Data.ToArray();
        }

        public void UpdateResult()
        {
            _Update();
        }


        protected override void _RefreshCommands()
        {
        }
    }
}