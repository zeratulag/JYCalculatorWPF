using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using System;
using System.Collections.Generic;

namespace JX3CalculatorShared.Class
{
    /// <summary>
    /// 描述游戏里基础buff的类
    /// </summary>
    public class BaseBuff
    {
        #region 成员

        public readonly string Name;
        public readonly string DescName;

        public readonly int IconID;
        public readonly bool IsTarget;

        public string BuffID;

        public string ToolTipDesc;
        public string Source;
        public string ToolTip;

        public int Order;
        public int ID { get; private set; }
        public int Level { get; private set; }

        public int AppendType { get; protected set; } = -1; // 覆盖叠加方式，-1表示无冲突
        public int Intensity { get; protected set; } = -1;

        public readonly CharAttrCollection CharAttrs; // 存储BUFF属性
        public readonly CharAttrCollection SCharAttrs; // 存储简化后的属性

        #endregion

        #region 构造

        /// <param name="name">唯一名称（编码）</param>
        /// <param name="descName">描述名</param>
        /// <param name="iconID">图标ID（可选）</param>
        /// <param name="isTarget">如果为True表示这是对目标生效的DeBuff</param>
        public BaseBuff(string name, string descName, int iconID = -1, bool isTarget = false)
        {
            Name = name;
            DescName = descName;
            IconID = iconID;
            IsTarget = isTarget;
        }

        public BaseBuff()
        {
        }

        /// <param name="name">唯一名称（编码）</param>
        /// <param name="descName">描述名</param>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <param name="iconID">图标ID（可选）</param>
        /// <param name="isTarget">如果为True表示这是对目标生效的DeBuff</param>
        /// <param name="data">属性字典</param>
        public BaseBuff(string name, string descName, int id, int level, int iconID = -1, bool isTarget = false,
            IDictionary<string, double> data = null) : this(name, descName, iconID, isTarget)
        {
            CharAttrs = new CharAttrCollection(data);
            SCharAttrs = CharAttrs.Simplify();
            ID = id;
            Level = level;
        }

        public BaseBuff(string name, string descName, int id, int level, int iconID = -1, bool isTarget = false,
            int appendType = -1, int intensity = -1,
            AttrCollection attrCollect = null) : this(name, descName, iconID, isTarget)
        {
            CharAttrs = new CharAttrCollection(attrCollect);
            SCharAttrs = CharAttrs.Simplify();
            AppendType = appendType;
            Intensity = intensity;
            ID = id;
            Level = level;
        }

        //public static BaseBuff ParseAbsBuffItem(AbsBuffItem item, bool isTarget)
        //{
        //    var attrCollection = item.ParseItem();
        //    var res = new BaseBuff(item.Name, item.DescName, TODO, TODO,
        //        iconID: item.IconID, isTarget: isTarget, attrCollect: attrCollection);
        //    res.ToolTipDesc = item.ToolTipDesc;
        //    res.MakeToolTip();
        //    return res;
        //}


        /// <summary>
        /// 判断一个属性是否只作用于目标而不是自身
        /// </summary>
        public static bool At_is_Target(string fullID)
        {
            bool res = (AttributeIDLoader.GetAttributeID(fullID).Target == 1);
            return res;
        }

        /// <summary>
        /// 判断属性字典的目标对象是否匹配
        /// </summary>
        /// <param name="fullID"></param>
        /// <returns></returns>
        public bool Match_Target(string fullID)
        {
            bool res = At_is_Target(fullID) == IsTarget;
            return res;
        }

        #endregion

        #region 方法

        public static (int ID, int Level) ParseIDLevel(string buffID)
        {
            if (string.IsNullOrWhiteSpace(buffID))
                throw new ArgumentException("buffID cannot be null or empty.", nameof(buffID));

            var parts = buffID.Split('_');
            if (parts.Length != 2)
                throw new FormatException("buffID must be in the format 'number_number', e.g. '3254_1'.");

            int id = int.Parse(parts[0]);
            int level = int.Parse(parts[1]);
            return (id, level);
        }

        public void MakeIDLevel()
        {
            var res = ParseIDLevel(BuffID);
            ID = res.ID;
            Level = res.Level;
        }

        public string GetToolTipHead()
        {
            var res = DescName;
            if (!String.IsNullOrEmpty(Source))
            {
                res += $" ~ {Source}";
            }

            return res;
        }

        public string GetToolTip()
        {
            var head = GetToolTipHead();
            var desc = ToolTipDesc;
            var tail = string.IsNullOrEmpty(BuffID) ? "" : $"\n\nID: {BuffID}";
            var res = $"{head} {StringConsts.TooltipDivider} {desc}{tail}";
            return res;
        }

        public void MakeToolTip()
        {
            ToolTip = GetToolTip();
        }

        #endregion
    }
}