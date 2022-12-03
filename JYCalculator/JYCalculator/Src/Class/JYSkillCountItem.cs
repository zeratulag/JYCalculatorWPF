using JX3CalculatorShared.Common;
using JYCalculator.Class;
using JYCalculator.Globals;
using System.Collections.Generic;
using JX3CalculatorShared.Src;
using JYCalculator.Src.Data;

namespace JYCalculator.Src.Class
{
    /// <summary>
    /// 描述惊羽技能数/频率模型
    /// </summary>
    public class JYSkillCountItem
    {
        #region 成员

        // Ability 对象中的标准释放时间
        public static readonly Period<JYSkillStandardTime> AbilitySkillTime =
            JYStaticData.CurrentHaste.GetJY_SkillStandardTime();

        public bool _XW { get; } // 表明是否为心无状态

        public int _Rank { get; } // 手法等级

        public double _Time { get; private set; } // 时间
        public double _UTime { get; private set; } // 技能利用时间
        public double _URate { get; private set; } // 时间利用率

        public int _BYPerCast = 7; // 单次大暴雨跳数

        public double _BYTotalHitNum => _BYCast * _BYPerCast; // 暴雨总Hit数
        public double _BYTail => _BYPerCast == 7 ? LH7 : LH5; // 暴雨尾跳数

        public double DP { get; set; } // 夺魄数
        public double BY { get; set; } // 暴雨第1跳次数
        public double LH2 { get; set; } // 暴雨第2跳次数
        public double LH3 { get; set; } // 暴雨第3跳次数
        public double LH4 { get; set; } // 暴雨第4跳次数
        public double LH5 { get; set; } // 暴雨第5跳次数
        public double LH6 { get; set; } // 暴雨第6跳次数
        public double LH7 { get; set; } // 暴雨第6跳次数

        public double ZM { get; set; } // 追命数
        public double ZM_SF { get; set; } //  瞬发追命数
        public double ZX { get; set; } // 逐星数
        public double _BYCast { get; set; } // 暴雨释放次数

        // 以下属性需要结合SkillNumModel计算得到
        public double BL { get; set; } // 百里
        public double GF { get; set; }
        public double CX_DOT { get; set; } // 穿心次数
        public double CXY_DOT { get; set; } // 鹰扬穿心次数
        public double ZX_DOT { get; set; } // 逐星DOT数
        public double _CX_DOT_Hit { get; set; } // 穿心DOT跳的次数

        #endregion

        #region 构造

        // 基于输入的技能数据导入
        public JYSkillCountItem(AbilitySkillNumItem item, int byPerCast)
        {
            _XW = item.XW == 1;
            _Time = item.Time;
            _Rank = item.Rank;

            _BYCast = item.BY_Cast;
            DP = item.DP;
            ZM_SF = item.ZM_SF;
            ZX = item.ZX;

            _BYCast = item.BY_Cast;
            _BYPerCast = byPerCast;

            SetLHBYNums(_BYCast);

            GetUtilization();
        }

        /// <summary>
        /// 设定梨花暴雨释放次数
        /// </summary>
        /// <param name="castNum">大暴雨施展次数</param>
        /// <param name="BYPerCast">单次大暴雨跳数</param>

        public void SetLHBYNums(double castNum, int BYPerCast)
        {
            LH5 = LH4 = LH3 = LH2 = BY = castNum;
            if (BYPerCast == 7)
            {
                LH7 = LH6 = castNum;
            }
        }

        public void SetLHBYNums(double castNum)
        {
            SetLHBYNums(castNum, _BYPerCast);
        }

        #endregion

        /// <summary>
        /// 转化为字典
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, double> ToDict()
        {
            var res = new Dictionary<string, double>()
            {
                {nameof(DP), DP}, {nameof(BY), BY},
                {nameof(LH2), LH2}, {nameof(LH3), LH3}, {nameof(LH4), LH4}, {nameof(LH5), LH5}, {nameof(LH6), LH6},
                {nameof(LH7), LH7},
                {nameof(ZM), ZM}, {nameof(ZM_SF), ZM_SF},
                {nameof(ZX), ZX}, {nameof(GF), GF}, {nameof(BL), BL},
                {nameof(CX_DOT), CX_DOT}, {nameof(CXY_DOT), CXY_DOT}, {nameof(ZX_DOT), ZX_DOT},
                {nameof(_BYCast), _BYCast}, {nameof(_BYTotalHitNum), _BYTotalHitNum}, {nameof(_BYTail), _BYTail},
                {nameof(_CX_DOT_Hit), _CX_DOT_Hit},
            };
            return res;
        }

        public SkillNumDict ToSkillNumDict()
        {
            var dict = ToDict();
            return new SkillNumDict(dict, _Time);
        }

        public SkillFreqDict ToSkillFreqDict()
        {
            var dict = ToDict();
            return new SkillFreqDict(dict, _Time);
        }

        public void GetUtilization()
        {
            var stdtime = _XW ? AbilitySkillTime.XW : AbilitySkillTime.Normal;
            var GCDNum = DP + ZM_SF + ZX;
            _UTime = GCDNum * stdtime.GCD + stdtime.BL * BL + _BYTotalHitNum * stdtime.BY;
            _URate = _UTime / _Time;
        }

        // 重设定时间
        public void ResetTime(double newTime)
        {
            if (newTime == _Time) return;
            var k = newTime / _Time;

            _UTime *= k;

            DP *= k;
            ZM *= k;
            ZM_SF *= k;
            ZX *= k;

            _BYCast *= k;
            SetLHBYNums(_BYCast);

            _Time = newTime;
        }


        // 计算罡风数
        public void CalcGF()
        {
            GF = DP + ZM + BL + _BYCast + ZM_SF * 1 + ZX * 1.5;
        }


        /// <summary>
        /// 循环内加入百里，计算其损失的暴雨逐星
        /// </summary>
        /// <param name="BLFreq">百里目标频率</param>
        /// <param name="BLTime">百里释放时间</param>
        public void ApplyBLFreq(double BLFreq, double BLTime)
        {
            BL = BLFreq * _Time;
            var k = 1 - BLFreq * BLTime;

            DP *= k;
            ZM *= k;
            ZM_SF *= k;

            ZX *= k;
            ZX_DOT *= k;

            _BYCast *= k;
            SetLHBYNums(_BYCast);

        }


    }
}