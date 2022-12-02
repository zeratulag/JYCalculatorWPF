using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JX3CalculatorShared.Common;

namespace JX3CalculatorShared.Utils
{
    public static class StringTool
    {
        public static void Cat(this IEnumerable<string> stringGroup)
        {
            foreach (var strItem in stringGroup)
            {
                Console.WriteLine(strItem);
            }
        }

        public static string ToStr<T>(this IEnumerable<T> list) where T : ICatable
        {
            string left = "[";
            var middleL = from item in list select item.ToStr();
            var middle = String.Join(", ", middleL);
            string right = "]";
            var res = $"{left} {middle} {right}";
            return res.ToString();
        }

        public static int ToInt(this char c)
        {
            return (int) (c - '0');
        }

        public static string StrJoin(this IEnumerable<string> stringList, string sep = "\n")
        {
            string res = String.Join(sep, stringList);
            return res;
        }

        public static void Cat(this string s)
        {
            Trace.WriteLine(s);
        }

        /// <summary>
        /// 移除字符串前缀
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="prefix">前缀</param>
        /// <returns></returns>
        public static string RemovePrefix(this string str, string prefix)
        {
            if (str.StartsWith(prefix))
            {
                str = str.Remove(0, prefix.Length);
            }

            return str;
        }

        /// <summary>
        /// 移除字符串前缀（只要有一种匹配就退出）
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="prefixes">前缀集合</param>
        /// <returns></returns>
        public static string RemovePrefixes(this string str, IEnumerable<string> prefixes)
        {
            foreach (var prefix in prefixes)
            {
                if (str.StartsWith(prefix))
                {
                    str = str.Remove(0, prefix.Length);
                    break;
                }
            }

            return str;
        }

        /// <summary>
        /// 移除字符串后缀
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="suffix">后缀</param>
        /// <returns></returns>
        public static string RemoveSuffix(this string str, string suffix)
        {
            if (str.EndsWith(suffix))
            {
                str = str.Remove(str.Length - suffix.Length, suffix.Length);
            }

            return str;
        }

        /// <summary>
        /// 将奇穴的ItemName填充到4个中文字符长度，为了选项对齐
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public static string PadQiXueItemName(string itemName)
        {
            var len = itemName.Length;
            var n = (4 - len);
            var res = itemName.PadLeft(len + n).PadRight(len + 2 * n);
            return res;
        }


        /// <summary>
        /// 根据数字大小，将浮点数转化为字符串
        /// </summary>
        /// <param name="x"></param>
        /// <returns>字符串形式</returns>
        public static string ToStr(this double x)
        {
            string fmt;
            double y = Math.Abs(x);
            if (y > 100)
            {
                fmt = "{0:F1}";
            }
            else if (y > 10)
            {
                fmt = "{0:F2}";
            }
            else if (y > 3)
            {
                fmt = "{0:F3}";
            }
            else if (y > 0)
            {
                fmt = "{0:P2}";
            }
            else
            {
                fmt = "{0:F0}";
            }

            var res = string.Format(fmt, x);
            return res;
        }


        /// <summary>
        /// 将一个文件名字符串中不合法的字符改成_
        /// </summary>
        /// <param name="filename">原始文件名</param>
        /// <returns></returns>
        public static string GetSafeFilename(string filename)
        {

            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));

        }

        /// <summary>
        /// 将形如 "a, b" 或是 "[a, b]" 的字符串拆分为数组
        /// </summary>
        /// <param name="x">输入字符串</param>
        /// <returns>字符串数组</returns>
        public static IEnumerable<string> ParseStringList(string x)
        {
            string y = x.RemovePrefix("[").RemoveSuffix("]");
            var xs = y.Split(",");
            var res = from _ in xs select _.Trim();
            return res;
        }

    }
}