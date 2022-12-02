using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;

namespace JX3CalculatorShared.Views
{
    public static class ViewTool
    {
        /// <summary>
        /// 在一个对象中中按照指定的条件筛选子组件对象，返回组件名称:组件的字典
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="parent">父对象</param>
        /// <param name="prefix">组件名称前缀</param>
        /// <param name="suffix">组件名称后缀</param>
        /// <returns></returns>
        public static Dictionary<string, T> FindChildrenElements<T>(this Panel parent,
            string prefix = "", string suffix = "")
            where T : FrameworkElement
        {
            var res = new Dictionary<string, T>();
            foreach (var child in parent.Children)
            {
                if (child is T childT)
                {
                    var name = childT.Name;
                    if (name.StartsWith(prefix) && name.EndsWith(suffix))
                    {
                        res.Add(name, childT);
                    }
                }
            }
            return res;
        }
    }

    /// <summary>
    /// 用于将控件名称转换解析的工具类
    /// </summary>
    public static class ViewNameTool
    {

        /// <summary>
        /// 从奇穴下拉框的名称中解析奇穴重数
        /// </summary>
        /// <param name="qiXueCbbName">奇穴下拉框名称</param>
        /// <returns>奇穴层数（1~12）</returns>
        public static int GetQiXuePosition(string qiXueCbbName)
        {
            var s = qiXueCbbName.RemovePrefix(ViewGlobals.PREFIX.QiXue).RemoveSuffix(ViewGlobals.SUFFIX.ComboBox);
            int i = int.Parse(s);
            return i;
        }

        /// <summary>
        /// 从奇穴重数还原奇穴下列列表对象名称
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static string GetQiXueCbbElementName(int position)
        {
            var res = $"{ViewGlobals.PREFIX.QiXue}{position:D}{ViewGlobals.SUFFIX.ComboBox}";
            return res;
        }

        /// <summary>
        /// 从单体下拉框解析单体类型
        /// </summary>
        /// <param name="itemDTCbbName"></param>
        /// <returns></returns>
        public static string GetItemDTType(string itemDTCbbName)
        {
            var s = itemDTCbbName.RemovePrefix(ViewGlobals.PREFIX.ItemDT).RemoveSuffix(ViewGlobals.SUFFIX.ComboBox);
            return s;
        }


        public static string GetBigFMType(string bigFMCbbName)
        {
            var s = bigFMCbbName.RemovePrefix(ViewGlobals.PREFIX.BigFM).RemoveSuffix(ViewGlobals.SUFFIX.ComboBox);
            return s;
        }

        public static EquipSubTypeEnum GetBigFMTypeEnum(string bigFMCbbName)
        {
            var s = GetBigFMType(bigFMCbbName);
            Enum.TryParse(s, true, out EquipSubTypeEnum subType);
            return subType;
        }

        /// <summary>
        /// 从装备类型枚举量获取大附魔GUI元素的名称
        /// </summary>
        /// <param name="subType">装备类型枚举</param>
        /// <returns>大附魔元素的名称(CheckBoxName, ComboBoxName)</returns>
        public static (string CheckBoxName, string ComboBoxName) GetBigFMElementsName(EquipSubTypeEnum subType)
        {
            var s = subType.ToString();
            var prefix = ViewGlobals.PREFIX.BigFM + s;
            var chbname = prefix + ViewGlobals.SUFFIX.CheckBox;
            var cbbname = prefix + ViewGlobals.SUFFIX.ComboBox;
            return (chbname, cbbname);
        }


        /// <summary>
        /// 从SkillKey获取秘籍GUI元素的名称
        /// </summary>
        /// <param name="skillkey">技能key，例如JD</param>
        /// <returns>秘籍元素的名称(ExpanderName, ListViewName)</returns>
        public static (string ExpanderName, string ListViewName) GetMiJiElementsName(string skillkey)
        {
            var expanderName = ViewGlobals.PREFIX.ExpanderMiJi + skillkey;
            var listViewName = ViewGlobals.PREFIX.ListViewMiJi + skillkey;
            return (expanderName, listViewName);
        }


    }
}