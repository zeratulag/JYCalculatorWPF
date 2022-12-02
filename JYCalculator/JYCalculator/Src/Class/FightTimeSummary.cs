using JX3CalculatorShared.Class;
using JYCalculator.ViewModels;


namespace JYCalculator.Class
{
    public class FightTimeSummary : FightTimeSummaryBase
    {
        #region 成员

        public FightTimeSummaryViewModel _VM { get; private set; } = null;

        #endregion

        #region 构造

        public FightTimeSummary() : base()
        {
        }

        public void AttachVM(FightTimeSummaryViewModel vm)
        {
            _VM = vm;
        }

        public FightTimeSummary(FightTimeSummaryViewModel vm) : this()
        {
            AttachVM(vm);
        }

        /// <summary>
        /// 用于描述战斗时间的类
        /// </summary>
        /// <param name="totalTime">总战斗时间（s）</param>
        /// <param name="xwCD">心无CD</param>
        /// <param name="xwDuration">心无持续时间</param>
        public FightTimeSummary(double totalTime, double xwCD = 90.0, double xwDuration = 15.0) : base(totalTime, xwCD,
            xwDuration)
        {
        }

        /// <summary>
        /// 仅仅改变心无CD
        /// </summary>
        /// <param name="xwCD"></param>
        public new void UpdateXWCD(double xwCD)
        {
            base.UpdateXWCD(xwCD);
            if (_VM != null)
            {
                _VM.XWCD = XWCD;
            }
        }

        public new void Update(double totalTime, double xwCD = 90.0, double xwDuration = 15.0, bool isShort = false)
        {
            base.Update(totalTime, xwCD, xwDuration, isShort);

            UpdateXWCD(xwCD);
        }

        public void Update(FightTimeSummaryViewModel vm)
        {
            Update(vm.TotalTime, vm.XWCD, vm.XWDuration, vm.FightOption.ShortFight);
        }

        #endregion

        #region 生成

        #endregion

        #region 生成结果

        #endregion
    }
}