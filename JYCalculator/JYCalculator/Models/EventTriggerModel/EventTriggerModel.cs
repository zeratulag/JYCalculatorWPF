namespace JYCalculator.Models
{
    /// <summary>
    /// 用于计算特殊触发BUFF的模型
    /// </summary>
    public partial class EventTriggerModel
    {
        public void Calc()
        {
            BuffCover.Reset();
            CalcSkillCTDF();
            CalcBigFMBelt();
            CalcSLBuff();
            CalcLongMenBuff();

            BuffedFChars.Normal.Has_Special_Buff = true;
            BuffedFChars.XW.Has_Special_Buff = true;

            CalcBigFM_SHOES_120();

#if DEBUG
            CalcHuiChang(); // 计算回肠频率
#endif
        }
    }
}