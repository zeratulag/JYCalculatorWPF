using System.Collections.Generic;

namespace JX3CalculatorShared.Utils
{
    public static class ListTool
    {
        /// <summary>
        /// 列表级浅拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="l"></param>
        /// <returns></returns>
        public static List<T> Copy<T>(this List<T> l)
        {
            List<T> res = null;
            if (l != null)
            {
                res = new List<T>(l.Capacity);
                res.AddRange(l);
            }
            return res;
        }
    }
}