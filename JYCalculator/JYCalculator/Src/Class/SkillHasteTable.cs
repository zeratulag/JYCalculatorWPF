﻿using System.Collections.Generic;
using System.Collections.Immutable;

namespace JYCalculator.Class
{
    public partial class SkillHasteTable
    {
        #region 独有成员

        public readonly HasteTableItem DP;
        public readonly HasteTableItem BY;

        public readonly HasteTableItem BL;

        public readonly HasteTableItem LHBY; // 梨花暴雨

        public readonly HasteTableItem CX3_DOT; // 穿心DOT
        public readonly HasteTableItem CX2_CL_DOT; // 穿林穿心DOT
        public readonly HasteTableItem ZX_DOT; // 逐星DOT

        #endregion

        #region 构造

        public SkillHasteTable(SkillDataDF df)
        {
            SkillDF = df;
            GCD = HasteTableItem.GetGCDItem();

            DP = new HasteTableItem(df.Data[nameof(DP)]);
            BY = new HasteTableItem(df.Data[nameof(BY)]);
            BL = new HasteTableItem(df.Data[nameof(BL)]);

            CX3_DOT = new HasteTableItem(df.Data[nameof(CX3_DOT)]);
            CX2_CL_DOT = new HasteTableItem(df.Data[nameof(CX2_CL_DOT)]);
            ZX_DOT = new HasteTableItem(df.Data[nameof(ZX_DOT)]);
            LHBY = new HasteTableItem(df.Data[nameof(LHBY)]);

            var dict = new Dictionary<string, HasteTableItem>()
            {
                {nameof(GCD), GCD},
                {nameof(DP), DP},
                {nameof(BY), BY},
                {nameof(BL), BL},
                {nameof(CX3_DOT), CX3_DOT},
                {nameof(CX2_CL_DOT), CX2_CL_DOT},
                {nameof(ZX_DOT), ZX_DOT},
                {nameof(LHBY), LHBY},
            };
            Dict = dict.ToImmutableDictionary();
        }
        #endregion
    }
}