using System;
using System.Diagnostics;
using Serilog;

namespace JX3CalculatorShared.Utils
{
    public static class FuncTool
    {
        /// <summary>
        /// 运行委托，并且计算运行时间
        /// </summary>
        /// <param name="act"></param>
        /// <param name="funcName"></param>
        public static void RunTime(Action act, string funcName = "")
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            act();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Log.Information($"{funcName}运行耗时：{elapsedMs}ms");
        }

    }
}