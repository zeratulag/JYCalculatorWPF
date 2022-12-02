using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Data;
using System;
using System.Collections.Generic;

namespace JX3CalculatorShared.Src.Class
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

        /// <param name="name">唯一名称（编码）</param>
        /// <param name="descName">描述名</param>
        /// <param name="iconID">图标ID（可选）</param>
        /// <param name="isTarget">如果为True表示这是对目标生效的DeBuff</param>
        /// <param name="data">属性字典</param>
        public BaseBuff(string name, string descName, int iconID = -1, bool isTarget = false,
            IDictionary<string, double> data = null) : this(name, descName, iconID, isTarget)
        {
            CharAttrs = new CharAttrCollection(data);
            SCharAttrs = CharAttrs.Simplify();
        }

        public BaseBuff(string name, string descName, int iconID = -1, bool isTarget = false,
            int appendType = -1, int intensity = -1,
            AttrCollection attrCollect = null) : this(name, descName, iconID, isTarget)
        {
            CharAttrs = new CharAttrCollection(attrCollect);
            SCharAttrs = CharAttrs.Simplify();
            AppendType = appendType;
            Intensity = intensity;
        }

        public static BaseBuff ParseAbsBuffItem(AbsBuffItem item, bool isTarget)
        {
            var attrCollection = item.ParseItem();
            var res = new BaseBuff(item.Name, item.DescName,
                iconID: item.IconID, isTarget: isTarget, attrCollect: attrCollection);
            res.ToolTipDesc = item.ToolTipDesc;
            res.MakeToolTip();
            return res;
        }


        /// <summary>
        /// 判断一个属性是否只作用于目标而不是自身
        /// </summary>
        public static bool At_is_Target(string fullID)
        {
            bool res = (AtLoader.GetAtTemplate(fullID).Target == 1);
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

        #region 显示

        #endregion

        #region 方法

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
            var res = $"{head} {Strings.TooltipDivider} {desc}{tail}";
            return res;
        }

        public void MakeToolTip()
        {
            ToolTip = GetToolTip();
        }

        #endregion

        #region 简化

        #endregion
    }
}