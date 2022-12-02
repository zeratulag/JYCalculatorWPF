using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.Src.DB;
using JYCalculator.Src.Data;
using System.Collections.Generic;

namespace JYCalculator.Src.DB
{
    public class EquipOptionDB: EquipOptionDBBase
    {
        #region 成员

        #endregion

        #region 构造
        public EquipOptionDB(IEnumerable<EquipOptionItem> wpitems, IEnumerable<EquipOptionItem> yzitems) : base(wpitems, yzitems)
        {
        }

        public EquipOptionDB(JYDataLoader jyDataLoader) : this(jyDataLoader.EquipOptionWP, jyDataLoader.EquipOptionYZ)
        {
        }

        public EquipOptionDB() : this(StaticJYData.Data)
        {
        }

        // 根据装备ID提取特效ID

        // 根据装备ID提取特效ID

        #endregion


        #region 取出

        #endregion

        #region 判断是否为特效的方法

        

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