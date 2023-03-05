using System.Collections.Generic;
using System.Collections.Immutable;

namespace JYCalculator.Class
{
    public partial class SkillHasteTable
    {
        #region 独有成员

        public readonly HasteTableItem DP;
        public readonly HasteTableItem BY;

        public readonly HasteTableItem BL;

        public readonly HasteTableItem CX_DOT; // 穿心DOT
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
    }
}