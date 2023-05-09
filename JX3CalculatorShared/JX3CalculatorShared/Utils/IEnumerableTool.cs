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


        // a 是否为 b 的子集
        public static bool IsSubsetOf<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            return !a.Except(b).Any();
        }

        /// <summary>
        /// 寻找一个序列中最大值出现的位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="l"></param>
        /// <returns>位置</returns>
        public static int WhichMax<T>(this IEnumerable<T> l) where T : IComparable
        {
            int i = 0;
            int res = 0;
            T m = l.First();
            foreach (var e in l)
            {
                if (e.CompareTo(m) > 0)
                {
                    m = e;
                    res = i;
                }
                i++;
            }
            return res;
        }

    }
}