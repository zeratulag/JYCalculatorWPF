using JX3CalculatorShared.Class;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JX3CalculatorShared.Utils
{
    public static class IEnumerableTool
    {
        public static string ToStr<T>(this IList<T> L, string sep = ", ")
        {
            string left = "[";
            string right = "]";
            var resL = from item in L select item.ToString();
            string middle = String.Join(sep, resL);
            string res = $"{left}{middle}{right}";
            return res;
        }

        public static void Cat<T>(this IList<T> L)
        {
            var catstr = L.ToStr();
            Console.WriteLine(catstr);
        }

        /// <summary>
        /// 一个集合内不重复元素的个数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="group"></param>
        /// <returns>个数</returns>
        public static int GetUniqueNum<T>(this IEnumerable<T> group)
        {
            var dn = group.Distinct().Count();
            return dn;
        }


        /// <summary>
        /// 检查一个集合内的元素是否唯一
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        public static bool IsUnique<T>(this IEnumerable<T> group)
        {
            var n = group.Count();
            var dn = group.GetUniqueNum();
            return (n == dn);
        }

        public static BaseBuffGroup Sum(this IEnumerable<BaseBuffGroup> baseBuffGroups)
        {
            return BaseBuffGroup.Sum(baseBuffGroups);
        }

        public static CharAttrCollection Sum(this IEnumerable<CharAttrCollection> group)
        {
            return CharAttrCollection.Sum(group);
        }

        public static SkillAttrCollection Sum(this IEnumerable<SkillAttrCollection> group)
        {
            return SkillAttrCollection.Sum(group);
        }

    }
}