using System.Collections.Generic;

namespace JX3CalculatorShared.Utils
{
    public static class CommandTool
    {
        public static void OpenUrl(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

        public static void OpenDictUrl(IDictionary<string, string> dict, string key)
        {
            var url = dict.GetValueOrUseDefault(key, "");
            if (url != "")
            {
                OpenUrl(url);
            }
        }

    }
}