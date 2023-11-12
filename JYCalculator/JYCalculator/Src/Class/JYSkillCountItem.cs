using HandyControl.Controls;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JYCalculator.Data;
using JYCalculator.Globals;
using System;
using System.Collections.Generic;

namespace JYCalculator.Class
{
    /// <summary>
    /// 描述惊羽技能数/频率模型
    /// </summary>
    public class JYSkillCountItem : SkillCountItemBase
    {
        #region 成员

        // Ability 对象中的标准释放时间
        public static readonly Period<JYSkillStandardTime> AbilitySkillTime =
            XFStaticConst.CurrentHaste.GetJY_SkillStandardTime();

        public int _BYPerCast = 7; // 单次大暴雨跳数

        public double _BYTotalHitNum => _BYCast * _BYPerCast; // 暴雨总Hit数
        public double _BYTail => _BYPerCast == 7 ? LH7 : LH5; // 暴雨尾跳数

        public double DP; // 夺魄数
        public double BY; // 暴雨第1跳次数
        public double LH2; // 暴雨第2跳次数
        public double LH3; // 暴雨第3跳次数
        public double LH4; // 暴雨第4跳次数
        public double LH5; // 暴雨第5跳次数
        public double LH6; // 暴雨第6跳次数
        public double LH7; // 暴雨第6跳次数

        public double ZM; // 追命数
        public double ZM_SF; //  瞬发追命数
        public double ZX; // 逐星数
        public double _BYCast; // 暴雨释放次数
        public double CXL; // 穿心弩次数

        // 以下属性需要结合SkillNumModel计算得到
        public double BL; // 百里
        public double GF;
        public double CX_DOT; // 穿心次数
        public double CXY_DOT; // 鹰扬穿心次数
        public double ZX_DOT; // 逐星DOT数
        public double _CX_DOT_Hit; // 穿心DOT跳的次数

        public static readonly string[] BLEffectNames; // 重新修正百里频率会导致这些技能频率改变
        public readonly Dictionary<string, List<string>> FieldToDictionary;
        #endregion

        #region 构造

        static JYSkillCountItem()
        {
            BLEffectNames = new[]
                {nameof(DP), nameof(ZM), nameof(ZM_SF), nameof(ZX), nameof(ZX_DOT), nameof(CXL), nameof(_BYCast), nameof(_BYTotalHitNum)};
        }

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
            CXL = item.CXL;

            _BYCast = item.BY_Cast;
            _BYPerCast = byPerCast;

            SetLHBYNums(_BYCast);
            FieldToDictionary = GetFieldToDictionary();

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

        /// <summary>
        /// 技能频率数关联关系
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetFieldToDictionary()
        {
            var res = new Dictionary<string, List<string>>()
            {
                {
                    nameof(_BYCast), new List<string>(8)
                    {
                        nameof(LH2), nameof(LH3), nameof(LH4), nameof(LH5), nameof(_BYTail)
                    }
                },
            };

            if (_BYPerCast == 7)
            {
                res[nameof(_BYCast)].Add(nameof(LH6));
                res[nameof(_BYCast)].Add(nameof(LH7));
            }

            return res;
        }

#endregion

/// <summary>
/// 转化为字典
/// </summary>
/// <returns></returns>
public Dictionary<string, double> ToDict()
        {
            var res = new Dictionary<string, double>(25)
            {
                {nameof(DP), DP}, {nameof(BY), BY},
                {nameof(ZM), ZM}, {nameof(ZM_SF), ZM_SF},
                {nameof(ZX), ZX}, {nameof(GF), GF}, {nameof(BL), BL},
                {"_ZX_Org", ZX},
                {nameof(CX_DOT), CX_DOT}, {nameof(CXY_DOT), CXY_DOT}, {nameof(ZX_DOT), ZX_DOT}, {nameof(CXL), CXL},
                {nameof(_BYCast), _BYCast}, {nameof(_BYTotalHitNum), _BYTotalHitNum},
                {nameof(_CX_DOT_Hit), _CX_DOT_Hit},
            };

            foreach (var kvp in FieldToDictionary)
            {
                foreach (var _ in kvp.Value)
                {
                    res[_] = res[kvp.Key];
                }
            }
            
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

        public void ResetTime(double newTime)
        {
            // 重设定时间（当新增字段时候别忘了增加）
            if (Math.Abs(newTime - _Time) < 1e-5) return;
            var k = newTime / _Time;

            _UTime *= k;

            DP *= k;
            ZM *= k;
            ZM_SF *= k;
            ZX *= k;
            CXL *= k;

            _BYCast *= k;
            SetLHBYNums(_BYCast);

            _Time = newTime;
        }


        // 计算罡风数
        public void CalcGF()
        {
            GF = DP + ZM + BL + _BYCast + ZM_SF * 1 + ZX * 1.25 + CXL;
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
            CXL *= k;

            _BYCast *= k;
            SetLHBYNums(_BYCast);
        }

        public double GetEnergyInjection()
        {
            var dict = ToDict();
            var energyInjectionCount = StaticXFData.DB.SkillInfo.GetEnergyInjection(dict);
            double res = energyInjectionCount / _Time;
            return res;
        }
    }
}