using System;
using System.Collections.Generic;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Utils;
using JYCalculator.Class;
using JYCalculator.Data;
using JYCalculator.Globals;
using JYCalculator.Models;

namespace JYCalculator.Src
{
    /// <summary>
    /// 描述惊羽通用技能数/频率模型
    /// </summary>
    public class JYSkillCountItem : SkillCountItemBase
    {
        public static readonly Period<JYSkillStandardTime> AbilitySkillTime =
            XFStaticConst.CurrentHaste.GetJY_SkillStandardTime();

        #region 需要外部输入的技能数，新增项时候记得同步修改ResetTime()方法体

        public double DP; // 夺魄数
        public double BL; // 百里
        public double ZM = 0; // 读条追命数
        public double ZM_SF; //  瞬发追命数
        public double ZX; // 逐星数
        public double KongQueLing; // 孔雀翎
        public double LveYingQiongCang; // 掠影苍穹
        public double CXL; // 穿心弩次数 

        public double DP_LaoJia; // 牢甲夺魄次数（百步凝形专用）
        public double CXDotCount; // 穿心DOT次数（百步凝形专用）
        public double PZ; // 破招次数

        public double _DPCast => DP + DP_LaoJia; // 夺魄释放次数
        public double _ZM_SFCast => ZM_SF; // 追命释放次数
        public double _ZMCast => ZM; // 读条追命释放次数

        #endregion

        public double ZM_BBCY = 0; // 百步穿杨

        public int _BYPerCast = 5; // 单次大暴雨跳数
        public double BY; // 暴雨第1跳次数
        public double LH2; // 暴雨第2跳次数
        public double LH3; // 暴雨第3跳次数
        public double LH4; // 暴雨第4跳次数
        public double LH5; // 暴雨第5跳次数
        public double LH6; // 暴雨第6跳次数
        public double LH7; // 暴雨第6跳次数
        public double _BYCast; // 暴雨释放次数

        public double GF;

        public double CX_DOT; // 穿心总次数
        public double ZX_DOT; // 逐星DOT数
        public double _CX_DOT_Hit; // 穿心DOT跳的次数
        public Dictionary<string, List<string>> FieldToDictionary;

        public double _BYTotalHitNum => _BYCast * _BYPerCast; // 暴雨总Hit数
        public double _BYTail => _BYPerCast == 7 ? LH7 : LH5; // 暴雨尾跳数

        public readonly int _CX_DOT_Stack; // 穿心dot叠的最大层数;
        public readonly bool 梨花带雨; // 是否为大暴雨
        public readonly bool 穿林打叶; // 是否为穿林穿心
        public readonly bool 白雨跳珠;
        public bool 百步凝形 { get; set; } = false; // 是否为百步凝形
        public double ChuanLinDaYe => 穿林打叶 ? ZX : 0; // 穿林打叶数量

        public readonly string __CX_DOT_Key; // 穿心Dot的最终key
        public readonly string __DP_Key; // 夺魄的最终key

        public string[] BLEffectNames { get; protected set; } // 重新修正百里频率会导致这些技能频率改变

        public JYSkillCountItem(AbilitySkillNumItem item, QiXueConfigModel qiXue)
        {
            _XW = item.XW == 1;
            _Time = item.Time;
            _Rank = item.Rank;

            _BYCast = item.BY_Cast;
            DP = item.DP;
            ZM_SF = item.ZM_SF;
            ZX = item.ZX;
            CXL = item.CXL;
            BL = item.BL;
            KongQueLing = item.KongQueLing;
            LveYingQiongCang = item.LveYingQiongCang;
            DP_LaoJia = item.DP_LaoJia;
            CXDotCount = item.CXDotCount;
            PZ = item.PZ;

            _BYCast = item.BY_Cast;
            _BYPerCast = qiXue.BYPerCast;

            _CX_DOT_Stack = qiXue.CX_DOT_Stack;
            穿林打叶 = qiXue.穿林打叶;
            __CX_DOT_Key = qiXue.CX_DOT_Key;
            __DP_Key = qiXue.DP_Key;

            梨花带雨 = qiXue.梨花带雨;
            白雨跳珠 = qiXue.白雨跳珠;

            SetLHBYNums(_BYCast);
            FieldToDictionary = GetFieldToDictionary();
            GetUtilization();
        }


        // 基于穿心DOT跳的次数，计算穿心DOT真实伤害次数
        public void SetCXDOTByHit(double _cX_DOT_Hit)
        {
            _CX_DOT_Hit = _cX_DOT_Hit;
            CX_DOT = _CX_DOT_Stack * _cX_DOT_Hit;
        }

        // 当穿心不能全程保持时，直接设定穿心次数，并且层数为1
        public void SetCXDotByCXDotCount(double cxDotCount)
        {
            _CX_DOT_Hit = cxDotCount;
            CX_DOT = cxDotCount;
        }

        /// <summary>
        /// 设定梨花暴雨释放次数
        /// </summary>
        /// <param name="castNum">大暴雨施展次数</param>
        /// <param name="BYPerCast">单次大暴雨跳数</param>
        public void SetLHBYNums(double castNum, int BYPerCast)
        {
            if (!梨花带雨)
            {
                BY = castNum * BYPerCast;
            }
            else
            {
                BY = castNum;
                LH5 = LH4 = LH3 = LH2 = castNum;
                if (BYPerCast == 7)
                {
                    LH7 = LH6 = castNum;
                }
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
            if (!梨花带雨) return new Dictionary<string, List<string>>();
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

        /// <summary>
        /// 转化为字典
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, double> ToDict()
        {
            var res = new Dictionary<string, double>(40)
            {
                {nameof(BY), BY},
                {nameof(ZM), ZM}, {nameof(ZM_SF), ZM_SF},
                {nameof(ZX), ZX}, {nameof(GF), GF}, {nameof(BL), BL},
                {nameof(KongQueLing), KongQueLing},
                {nameof(LveYingQiongCang), LveYingQiongCang},
                {nameof(ChuanLinDaYe), ChuanLinDaYe},
                {nameof(PZ), PZ},

                {"_ZX_Org", ZX},
                {nameof(ZX_DOT), ZX_DOT}, {nameof(CXL), CXL},
                {nameof(_BYCast), _BYCast}, {nameof(_BYTotalHitNum), _BYTotalHitNum},
                {nameof(_CX_DOT_Hit), _CX_DOT_Hit},

                {__CX_DOT_Key, CX_DOT},
                {__DP_Key, DP},
                {nameof(_DPCast), _DPCast},
                {nameof(_ZM_SFCast), _ZM_SFCast},
                {nameof(_ZMCast), _ZMCast},

                {nameof(ZM_BBCY), ZM_BBCY},
            };

            AddBaoYu(res);

            return res;
        }

        public void AddBaoYu(Dictionary<string, double> dict)
        {
            foreach (var kvp in FieldToDictionary)
            {
                foreach (var _ in kvp.Value)
                {
                    dict[_] = dict[kvp.Key];
                }
            }
        }


        public SkillNumDict ToSkillNumDict()
        {
            var dict = ToDict();
            return new SkillNumDict(dict, _Time);
        }

        public SkillFreqDict ToSkillFreqDict()
        {
            var dict = ToDict();
            var arg = new SkillFreqArg(白雨跳珠, 百步凝形);
            return new SkillFreqDict(dict, _Time, arg);
        }

        public void GetUtilization()
        {
            var stdtime = _XW ? AbilitySkillTime.XinWu : AbilitySkillTime.Normal;
            var GCDNum = DP + ZM_SF + ZX;
            _UTime = GCDNum * stdtime.GCD + stdtime.BL * BL + _BYTotalHitNum * stdtime.BY;
            _URate = _UTime / _Time;
        }

        public virtual void ResetTime(double newTime)
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
            BL *= k;
            KongQueLing *= k;
            LveYingQiongCang *= k;

            DP_LaoJia *= k;
            CXDotCount *= k;
            PZ *= k;

            _BYCast *= k;
            SetLHBYNums(_BYCast);

            _Time = newTime;
        }

        public double GetEnergyInjection()
        {
            var dict = ToDict();
            var energyInjectionCount = StaticXFData.DB.BaseSkillInfo.GetEnergyInjection(dict);
            double res = energyInjectionCount / _Time;
            return res;
        }
    }
}