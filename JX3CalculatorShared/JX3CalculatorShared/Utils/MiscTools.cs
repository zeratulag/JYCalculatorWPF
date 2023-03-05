using System;
using System.Globalization;
using System.Reflection;

namespace JX3CalculatorShared.Utils
{
    public static class MiscTools
    {
        const string BuildVersionMetadataPrefix = "+build";
        const string dateFormat = "yyyy-MM-ddTHH:mm:ss:fffZ";
        public static DateTime GetLinkerTime(Assembly assembly)
        {
            var attribute = assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            if (attribute?.InformationalVersion != null)
            {
                var value = attribute.InformationalVersion;
                var index = value.IndexOf(BuildVersionMetadataPrefix);
                if (index > 0)
                {
                    //value = value[(index + BuildVersionMetadataPrefix.Length)..];

                    return DateTime.ParseExact(
                        value,
                        dateFormat,
                        CultureInfo.InvariantCulture);
                }
            }
            return default;
        }

        public static void GetBuildTime()
        {
            // 获取程序构建时间
            var buildTime = GetLinkerTime(Assembly.GetEntryAssembly());
        }

    }
}