using System;
using System.Diagnostics;

namespace JX3CalculatorShared.Utils
{
    public static class FuncTool
    {
        /// <summary>
        /// 运行委托，并且计算运行时间
        /// </summary>
        /// <param name="act"></param>
        public static void RunTime(Action act)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            act();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Trace.WriteLine($"运行耗时：{elapsedMs}ms");
        }

    }
}