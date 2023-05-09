using System.Collections.Immutable;
using JX3CalculatorShared.Data;
using JX3PZ.Class;
using JX3PZ.Data;
using JX3PZ.Globals;

namespace JX3PZ.Data
{
    public class DiamondTabItem : AttributeTabItem
    {
        public int Value { get; set; }
        public string Tag => Attribute.EquipTag;

        public readonly ImmutableArray<int> LevelValues; // 不同等级的镶嵌数值
        public readonly ImmutableArray<string> LevelDesc; // 不同等级的镶嵌描述
        public readonly ImmutableArray<DiamondLevelItem> LevelItems; // 不同等级的镶嵌描述

        public readonly bool IsEmpty = false;
        
        public static readonly DiamondTabItem EmptyItem = new DiamondTabItem();

        public DiamondTabItem(AttributeTabItem at)
        {
            ID = at.ID;
            ModifyType = at.ModifyType;
            Attribute = at.Attribute;
            Param1 = at.Param1;
            Param2 = at.Param2;
            Value = at.Param1;

            var levels = new int[PzConst.MAX_DIAMOND_LEVEL + 1];
            for (int i = 0; i < PzConst.MAX_DIAMOND_LEVEL + 1; i++)
            {
                levels[i] = GetValue(i);
            }

            LevelValues = levels.ToImmutableArray();

            var desc = new string[PzConst.MAX_DIAMOND_LEVEL + 1];
            for (int i = 0; i < PzConst.MAX_DIAMOND_LEVEL + 1; i++)
            {
                desc[i] = GetDesc(i);
            }

            LevelDesc = desc.ToImmutableArray();
            LevelItems = DiamondLevelItem.GetLevelItems(this).ToImmutableArray();
            IsEmpty = false;

        }

        public DiamondTabItem(int id) : this(AttributeTabLib.Get(id))
        {
        }

        public DiamondTabItem()
        {
            IsEmpty = true;
            // 生成空信息
            Attribute = AttributeIDLoader.EmptyAttributeID;

            var levels = new int[PzConst.MAX_DIAMOND_LEVEL + 1];
            LevelValues = levels.ToImmutableArray();

            var desc = new string[PzConst.MAX_DIAMOND_LEVEL + 1];
            LevelDesc = desc.ToImmutableArray();
            LevelItems = DiamondLevelItem.GetLevelItems(this).ToImmutableArray();
        }

        /// <summary>
        /// 生成描述字符串
        /// </summary>
        /// <param name="level">等级</param>
        /// <param name="prefix">是否加上“熔嵌孔：”前缀</param>
        /// <returns></returns>
        public string GetDesc(int level, bool prefix = true)
        {
            const string PREFIX = "熔嵌孔：";
            string middle = _GetAttributeDesc(level, false);

            if (prefix)
            {
                return PREFIX + middle;
            }
            else
            {
                return middle;
            }
        }

        /// <summary>
        /// 生成描述字符串
        /// </summary>
        /// <param name="level">等级</param>
        /// <param name="prefix">是否加上“熔嵌孔：”前缀</param>
        /// <returns></returns>
        public string GetSimpleDesc(int level, bool prefix = true)
        {
            const string PREFIX = "熔嵌孔：";
            string middle = _GetAttributeDesc(level, true);

            if (prefix)
            {
                return PREFIX + middle;
            }
            else
            {
                return middle;
            }
        }

        private string _GetAttributeDesc(int level, bool simple = false)
        {
            string middle = "";
            if (level <= 0 || level > PzConst.MAX_DIAMOND_LEVEL)
            {
                if (simple)
                {
                    middle = Attribute.SimpleDesc + "?";
                }
                else
                {
                    middle = Attribute.FullBaseDesc + "?";
                }
            }
            else
            {
                if (simple)
                {
                    middle = Attribute.GetSimpleDesc(LevelValues[level]);
                }
                else
                {
                    middle = Attribute.GetFullDesc(LevelValues[level]);
                }
            }

            return middle;
        }


        /// <summary>
        /// 计算等级五行石的镶嵌数值
        /// </summary>
        /// <param name="level"></param>
        public int GetValue(int level)
        {
            var v = GetCoef(level);
            var res = (int) (Value * v);
            return res;
        }

        public static double GetCoef(int level)
        {
            double res;
            if (level <= 6)
            {
                res = 0.195 * level;
            }
            else
            {
                res = 1.3 * (0.65 * level - 3.2);
            }

            return res;
        }

        /// <summary>
        /// 获取品质等级
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int GetQuality(int level)
        {
            var res = 1 + ((double) level + 1) / 2;
            return (int) res;
        }

        /// <summary>
        /// 获取最终属性词条
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public AttributeEntry GetAttriValueEntry(int level)
        {
            var value = GetValue(level);
            var res = new AttributeEntry(ModifyType, value);
            return res;
        }
    }
}