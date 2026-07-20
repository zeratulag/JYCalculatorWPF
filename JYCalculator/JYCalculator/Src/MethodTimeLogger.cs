using System.Reflection;
using Serilog;

namespace JYCalculator
{
    // https://github.com/Fody/MethodTimer#what-gets-compiled-with-an-interceptor
    public static class MethodTimeLogger
    {
        public static void Log(MethodBase methodBase, long milliseconds, string message)
        {
            var className = methodBase.DeclaringType?.Name ?? "Unknown";
            var methodName = methodBase.Name;

            if (string.IsNullOrEmpty(message))
            {
                Serilog.Log.Information("[MethodTimer] {ClassName}.{MethodName} 耗时 {Ms} ms",
                    className, methodName, milliseconds);
            }
            else
            {
                Serilog.Log.Information("[MethodTimer] {ClassName}.{MethodName} 耗时 {Ms} ms | {Message}",
                    className, methodName, milliseconds, message);
            }
        }
    }
}
