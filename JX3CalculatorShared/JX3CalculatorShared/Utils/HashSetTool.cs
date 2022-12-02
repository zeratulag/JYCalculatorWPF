using System.Collections.Generic;

namespace JX3CalculatorShared.Utils
{
    public static class HashSetTool
    {
        public static void AddRange<T>(this HashSet<T> org, IEnumerable<T> others)
        {
            foreach (var item in others)
            {
                org.Add(item);
            }
        }
    }
}