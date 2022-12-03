using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Class;
using MiniExcelLibs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace JX3CalculatorShared.Utils
{
    public static class ImportTool
    {
        /// <summary>
        /// 通用从Excel中读取工作簿，并且返回以列名的不变字典方法
        /// </summary>
        /// <typeparam name="TKey">用于作为检索key的类型</typeparam>
        /// <typeparam name="TItem">读取工作表的每一行实例类</typeparam>
        /// <param name="fileName">文件路径</param>
        /// <param name="sheetName">工作表名称</param>
        /// <param name="keyName">用于作为检索key的列名</param>
        /// <returns>ImmutableDictionary</returns>
        public static ImmutableDictionary<TKey, TItem> ReadSheetAsDict<TKey, TItem>(string fileName, string sheetName,
            string keyName = "Name") where TItem : class, new()
        {
            var stream = GetStreamFromResource(fileName);
            var rows = MiniExcel.Query<TItem>(stream, sheetName: sheetName);
            var res = rows.ToDictionary(x => x.GetPropertyValue<TKey>(keyName));
            return res.ToImmutableDictionary();
        }


        public static ImmutableArray<TItem> ReadSheetAsArray<TItem>(string fileName, string sheetName)
            where TItem : class, new()
        {
            var stream = GetStreamFromResource(fileName);
            var rows = MiniExcel.Query<TItem>(stream, sheetName: sheetName);
            var res = rows.ToArray().ToImmutableArray();

            return res;
        }

        public static string[] GetSheetNames(string fileName)
        {
            var stream = GetStreamFromResource(fileName);
            var sheetNames = MiniExcel.GetSheetNames(stream);
            return sheetNames.ToArray();
        }

        /// <summary>
        /// 从Resource中获取文件流
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>文件流</returns>
        public static Stream GetStreamFromResource(string path)
        {
            var uri = new Uri(AppStatic.URI_PREFIX + path, UriKind.Absolute);
            var s = Application.GetResourceStream(uri).Stream;
            return s;
        }

        /// <summary>
        /// 从Resource中读取所有文本内容，以字符串返回，如同读取一个外部文本文件一样，统一使用UTF8编码
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="encode">编码，默认UTF8</param>
        /// <returns>字符串</returns>
        public static string ReadAllTextFromResource(string path, Encoding encode = null)
        {
            var s = GetStreamFromResource(path);
            Encoding encoding = encode ?? Encoding.UTF8;
            var res = s.ReadToEnd(encoding);
            return res;
        }

        public static (bool success, T res) TryDeJSON<T>(string json) where T: class, new()
        {
            // [TODO] ：完善算法
            T res = null;
            bool success = false;

            try
            {
                res = JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                Trace.Write("格式错误！");
            }

            if (res != null)
            {
                success = true;
            }

            return (success, res);
        }

        /// <summary>
        /// 导入JSON并且转换为指定的格式
        /// </summary>
        /// <typeparam name="T">目标格式</typeparam>
        /// <param name="path">JSON路径</param>
        /// <returns></returns>
        public static T ReadJSON<T>(string path) where T : class, new()
        {
            var txt = ReadAllTextFromResource(path, Encoding.UTF8);
            var res = JsonConvert.DeserializeObject<T>(txt);
            return res;
        }

    }
}