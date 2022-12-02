using System.Collections.Generic;
using System.Collections.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Data;
using JYCalculator.Globals;
using JYCalculator.Src;

namespace JYCalculator.Class
{
    public class SkillHasteTable : ObservableObject
    {
        /// <summary>
        /// 一些已经固定的Item，不需要重复计算
        /// </summary>

        #region 成员

        public readonly HasteTableItem GCD;

        public readonly HasteTableItem DP;
        public readonly HasteTableItem BY;

        public readonly HasteTableItem BL;

        public readonly HasteTableItem CX_DOT; // 穿心DOT
        public readonly HasteTableItem ZX_DOT; // 逐星DOT

        public readonly SkillDataDF SkillDF;
        public readonly ImmutableDictionary<string, HasteTableItem> Dict;

        public int HSP { get; set; } // 加速
        public int XWExtraHSP { get; set; } // 心无额外加速

        #endregion

        #region 构造

        public SkillHasteTable(SkillDataDF df)
        {
            SkillDF = df;
            GCD = HasteTableItem.GetGCDItem();

            DP = new HasteTableItem(df.Data[nameof(DP)]);
            BY = new HasteTableItem(df.Data[nameof(BY)]);
            BL = new HasteTableItem(df.Data[nameof(BL)]);

            CX_DOT = new HasteTableItem(df.Data[nameof(CX_DOT)]);
            ZX_DOT = new HasteTableItem(df.Data[nameof(ZX_DOT)]);

            var dict = new Dictionary<string, HasteTableItem>()
            {
                {nameof(GCD), GCD}, 
                {nameof(DP), DP}, {nameof(BY), BY}, {nameof(BL), BL},
                {nameof(CX_DOT), CX_DOT}, {nameof(ZX_DOT), ZX_DOT},
            };
            Dict = dict.ToImmutableDictionary();
        }

        #endregion

        /// <summary>
        /// 更新加速输入
        /// </summary>
        /// <param name="hs"></param>
        /// <param name="xwextrahsp"></param>
        public void UpdateHSP(int hs, int xwextrahsp)
        {
            HSP = hs;
            XWExtraHSP = xwextrahsp;
            Calc();
        }

        public void UpdateHSP(CalculatorShell shell)
        {
            UpdateHSP(shell.HS, shell.XWExtraSP);
        }

        public void Calc()
        {
            foreach (var KVP in Dict)
            {
                KVP.Value.CalcHaste(HSP, XWExtraHSP);
            }
        }
    }

    public class HasteTableItem
    {
        #region 成员

        // 输入数据
        public string Name { get; }
        public string SkillName { get; } // 技能名称
        public int nCount { get; } // 跳数
        public double RawIntervalTime => RawFrame / StaticData.FPS_PER_SECOND; // 初始单跳时间
        public int RawFrame { get; } // 初始帧数
        public double RawTime => nCount * RawIntervalTime; // 初始总时间

        // 通过加速计算的数据
        public int Frame { get; private set; } // 加速后帧数
        public double IntervalTime => Frame / StaticData.FPS_PER_SECOND; // 加速后单跳时间
        public double Time => nCount * IntervalTime; // 加速后总时间

        public int XWFrame { get; private set; } // 心无期间加速后帧数
        public double XWIntervalTime => XWFrame / StaticData.FPS_PER_SECOND; // 心无期间加速后单跳时间
        public double XWTime => nCount * XWIntervalTime; // 心无期间加速后总时间

        #endregion

        #region 构造函数

        public HasteTableItem(string name, string skillname, int rawFrame, int ncount)
        {
            Name = name;
            SkillName = skillname;
            RawFrame = rawFrame;
            nCount = ncount;
        }

        // 基于动态数据构建
        public HasteTableItem(SkillData data)
        {
            Name = data.Name;
            SkillName = data.SkillName;
            RawFrame = data.Frame;
            nCount = data.nCount;
        }

        // 基于静态数据构建
        public HasteTableItem(SkillInfoItem info)
        {
            Name = info.Name;
            SkillName = info.Skill_Name;
            RawFrame = info.Frame;
        }

        /// <summary>
        /// 复制构造
        /// </summary>
        /// <param name="old"></param>
        public HasteTableItem(HasteTableItem old)
        {
            Name = old.Name;
            SkillName = old.SkillName;
            RawFrame = old.RawFrame;
            nCount = old.nCount;
        }

        public HasteTableItem Copy()
        {
            return new HasteTableItem(this);
        }

        /// <summary>
        /// GCD对象
        /// </summary>
        /// <returns></returns>
        public static HasteTableItem GetGCDItem()
        {
            return new HasteTableItem("GCD", "公共CD", StaticData.GCD_FPS, 1);
        }

        #endregion


        #region 计算加速

        /// <summary>
        /// 根据加速值，计算加速
        /// </summary>
        /// <param name="HSP">加速等级</param>
        /// <param name="XWExtraHSP">心无额外加速</param>
        public void CalcHaste(int HSP, int XWExtraHSP = JYStaticData.XWConsts.ExtraSP)
        {
            Frame = JYStaticData.CurrentHaste.SKT_FPS(RawFrame, HSP, 0);
            XWFrame = JYStaticData.CurrentHaste.SKT_FPS(RawFrame, HSP, XWExtraHSP);
        }

        #endregion
    }
}