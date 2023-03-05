using JX3CalculatorShared.Data;
using JX3CalculatorShared.DB;
using JYCalculator.Data;
using System.Collections.Generic;

namespace JYCalculator.DB
{
    public class EquipOptionDB : EquipOptionDBBase
    {

        #region 构造
        public EquipOptionDB(IEnumerable<EquipOptionItem> wpitems, IEnumerable<EquipOptionItem> yzitems) : base(wpitems, yzitems)
        {
        }

        public EquipOptionDB(XFDataLoader xfDataLoader) : this(xfDataLoader.EquipOptionWP, xfDataLoader.EquipOptionYZ)
        {
        }

        public EquipOptionDB() : this(StaticXFData.Data)
        {
        }



        // 根据装备ID提取特效ID

        #endregion

        public void AttachWPSkillEvents(SkillInfoDB db)
        {
            foreach (var _ in WP)
            {
                _.GetEvents(db.Events.Values);
            }
        }
    }
}