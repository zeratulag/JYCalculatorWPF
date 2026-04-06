using System;
using System.Collections.Generic;
using System.Linq;

namespace MingYiTools
{
    public static class MingYiItemMapper
    {
        public static MingYiItemData[] ParseLuaItemData(object luaData)
        {
            var resultList = new List<MingYiItemData>();

            // === 调试信息：打印输入数据的真实类型 ===
            if (luaData == null)
            {
                return new MingYiItemData[0];
            }

            // 定义我们需要遍历的集合
            IEnumerable<object> itemsToProcess = null;

            // 情况 A: 标准 List (MoonSharp 或 LuaParser 识别为纯数组时)
            if (luaData is List<object> list)
            {
                itemsToProcess = list;
                if (itemsToProcess.Count() == 1)
                {
                    var obj = itemsToProcess.First();
                    itemsToProcess = obj as List<object>;
                }
            }
            // 情况 B: Dictionary (LuaParser 识别为对象，或 MoonSharp Table)
            // 此时我们需要遍历它的 Values 来获取实际的装备列表
            else if (luaData is Dictionary<string, object> dict)
            {
                itemsToProcess = dict.Values;
            }
            else
            {
                return new MingYiItemData[0];
            }

            // 开始转换
            foreach (var item in itemsToProcess)
            {
                // 确保每一项也是一个字典（存放 nPos, nDurability 等属性）
                if (item is Dictionary<string, object> itemDict)
                {
                    try
                    {
                        var data = MapDictionaryToItem(itemDict);
                        resultList.Add(data);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return resultList.ToArray();
        }

        private static MingYiItemData MapDictionaryToItem(Dictionary<string, object> dict)
        {
            var data = new MingYiItemData();

            // 1. 基础字段映射
            data.nPos = GetInt(dict, "nPos");
            data.nStrengthLevel = GetInt(dict, "nStrengthLevel");
            data.nStackNum = GetInt(dict, "nStackNum");
            data.nDurability = GetInt(dict, "nDurability");
            data.nMaxDurability = GetInt(dict, "nMaxDurability");
            data.nMaxStrengthLevel = GetInt(dict, "nMaxStrengthLevel");
            data.nQuality = GetInt(dict, "nQuality");

            data.dwTabIndex = GetInt(dict, "dwTabIndex");
            data.dwTabType = GetInt(dict, "dwTabType");
            data.dwTabSubIndex = GetInt(dict, "dwTabSubIndex");
            data.dwItemFEAEnchantID = GetInt(dict, "dwItemFEAEnchantID");
            data.dwTemporaryEnchantID = GetInt(dict, "dwTemporaryEnchantID");
            data.dwPermanentEnchantID = GetInt(dict, "dwPermanentEnchantID");

            // 2. 集合字段处理：aDiamondEnchant
            if (dict.TryGetValue("aDiamondEnchant", out object enchantObj))
            {
                // 情况 A: 它是 List
                if (enchantObj is List<object> enchantList)
                {
                    foreach (var val in enchantList) data.aDiamondEnchant.Add(Convert.ToInt32(val));
                }
                // 情况 B: 它可能是 Dictionary (如果是稀疏数组)
                else if (enchantObj is Dictionary<string, object> enchantDict)
                {
                    foreach (var val in enchantDict.Values) data.aDiamondEnchant.Add(Convert.ToInt32(val));
                }
            }

            return data;
        }

        // =========================================================
        // Helper 方法
        // =========================================================

        private static int? GetInt(Dictionary<string, object> dict, string key)
        {
            if (dict.TryGetValue(key, out object val) && val != null)
            {
                try { return Convert.ToInt32(val); } catch { return null; }
            }
            return null;
        }

        private static long? GetLong(Dictionary<string, object> dict, string key)
        {
            if (dict.TryGetValue(key, out object val) && val != null)
            {
                try { return Convert.ToInt64(val); } catch { return null; }
            }
            return null;
        }
    }
}