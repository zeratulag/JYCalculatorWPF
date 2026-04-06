using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace LuaUtils
{
    public static class LuaTools
    {
        public static string DecompressLuaData(string compressedBase64)
        {
            // 1. Base64 解码 (处理 URL 安全字符)
            // Python: .replace('-', '+').replace('_', '/')
            string base64Str = compressedBase64.Replace('-', '+').Replace('_', '/');

            // 处理填充 (Padding)
            // Python 的 base64 库对 padding 要求严格，C# Convert.FromBase64String 也是如此
            switch (base64Str.Length % 4)
            {
                case 2: base64Str += "=="; break;
                case 3: base64Str += "="; break;
            }

            byte[] bytesData = Convert.FromBase64String(base64Str);

            // 2. Zlib 解压
            // Python 的 zlib.decompress 包含了 Zlib header (RFC 1950)
            // .NET 的 DeflateStream 使用的是 Raw Deflate (RFC 1951)
            // 解决方法：跳过前 2 个字节 (Header)，通常是 0x78 0x9C 或 0x78 0xDA
            using (MemoryStream inputStream = new MemoryStream(bytesData))
            {
                // 跳过 Zlib 头部 (2 bytes)
                if (bytesData.Length > 2)
                {
                    inputStream.ReadByte();
                    inputStream.ReadByte();
                }

                using (DeflateStream deflateStream = new DeflateStream(inputStream, CompressionMode.Decompress))
                using (MemoryStream outputStream = new MemoryStream())
                {
                    deflateStream.CopyTo(outputStream);
                    byte[] decompressedBytes = outputStream.ToArray();

                    // 3. 解码为字符串
                    // Python: .decode('utf-8')
                    string luaString = Encoding.UTF8.GetString(decompressedBytes);

                    return luaString;
                }
            }
        }

        public static object ParseLuaTableString(string luaString)
        {
            var res = LuaTableParser.Parse(luaString);
            return res;
        }

        /// <summary>
        /// 将 Lua 数据字符串转换为 C# 原生对象 (List 或 Dictionary)
        /// </summary>
        //public static object ParseLuaTableString1(string luaString)
        //{
        //    // MoonSharp 需要在其上下文中运行脚本
        //    // 注意：Lua 数据通常只是表定义，我们需要加 "return" 才能把它取出来
        //    string script = "return " + luaString;

        //    Script luaScript = new Script();
        //    DynValue luaValue = luaScript.DoString(script);

        //    return ConvertDynValue(luaValue);
        //}

        /// <summary>
        /// 递归转换 MoonSharp 的 DynValue 为 C# 原生类型
        /// </summary>
        //private static object ConvertDynValue(DynValue value)
        //{
        //    switch (value.Type)
        //    {
        //        case DataType.Table:
        //            var table = value.Table;
        //            // 判断是 List (数组) 还是 Dictionary (对象)
        //            // Lua 的数组索引是连续整数，且从 1 开始
        //            bool isArray = table.Length > 0 && table.Keys.All(k => k.Type == DataType.Number);

        //            if (isArray)
        //            {
        //                var list = new List<object>();
        //                // 遍历 Lua 数组 (注意：Lua 索引从1开始，但 MoonSharp 的 Values 迭代器会自动处理)
        //                foreach (var val in table.Values)
        //                {
        //                    list.Add(ConvertDynValue(val));
        //                }
        //                return list;
        //            }
        //            else
        //            {
        //                var dict = new Dictionary<string, object>();
        //                foreach (var pair in table.Pairs)
        //                {
        //                    // 忽略 Lua 这里的索引键，只保留字符串键（针对混合型 Table）
        //                    // 你的数据全是字符串键，所以这里主要处理 key=value
        //                    if (pair.Key.Type == DataType.String)
        //                    {
        //                        dict[pair.Key.String] = ConvertDynValue(pair.Value);
        //                    }
        //                    // 如果有数字索引的混合数据，可视情况添加逻辑
        //                }
        //                return dict;
        //            }

        //        case DataType.Number:
        //            // Lua 的 Number 都是 double，这里尝试转为 int/long 让 JSON 好看点
        //            if (Math.Abs(value.Number % 1) < double.Epsilon)
        //                return (long)value.Number;
        //            return value.Number;

        //        case DataType.String:
        //            return value.String;

        //        case DataType.Boolean:
        //            return value.Boolean;

        //        case DataType.Nil:
        //            return null;

        //        default:
        //            return value.ToString();
        //    }
        ////}

    }
}