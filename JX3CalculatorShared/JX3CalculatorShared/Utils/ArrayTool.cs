namespace JX3CalculatorShared.Utils
{
    public static class ArrayTool
    {
        public static bool IsNotEmptyOrNull<T>(this T[] arr)
        {
            // 一个数组非空且不为Null
            return arr != null && arr.Length > 0;
        }
    }
}