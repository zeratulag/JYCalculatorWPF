namespace JX3CalculatorShared.Class
{
    public class SkillCountItemBase
    {

        public bool _XW; // 表明是否为心无状态
        public int _Rank; // 手法等级
        public double _Time { get; protected set; } // 时间
        public double _UTime { get; protected set; } // 技能利用时间
        public double _URate { get; protected set; } // 时间利用率

        public double GetNumByFreq(double freq)
        {
            // 基于技能频率计算技能数
            return _Time * freq;
        }

        public double GetNumByInterval(double interval)
        {
            // 基于技能间隔计算技能数
            return _Time / interval;
        }
    }
}