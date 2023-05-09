using System.Collections.Generic;

namespace JX3CalculatorShared.Utils
{
    public static class ListTool
    {
        public static List<T> Copy<T>(this List<T> l)
        {
            if (l == null)
            {
                return null;
            }
            else
            {
                var res = new List<T>(l.Capacity);
                res.AddRange(l);
            }
            return l;
        }
    }
}