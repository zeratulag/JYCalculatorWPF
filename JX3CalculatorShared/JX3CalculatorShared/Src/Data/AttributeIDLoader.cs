using JX3CalculatorShared.Class;
using JX3PZ.Class;
using JX3PZ.Globals;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static JX3CalculatorShared.Utils.ImportTool;


namespace JX3CalculatorShared.Data
{
    public static class AttributeIDLoader
    {
        public static string Path;

        public static ImmutableDictionary<string, SkillAttrItem> SkillAttr;
        public static ImmutableDictionary<string, SkillAttributeId> SkillAttributeIdLib { get; private set; }

        public static ImmutableDictionary<string, AttrItem> Attr;
        public static ImmutableDictionary<string, AttributeID> AttributeIDLib { get; private set; }

        public static ImmutableDictionary<string, string> LuaID2FullID { get; private set; } // LuaID到普通ID的映射
        public static AttributeID EmptyAttributeID { get; private set; } // 空

        public static void Load(string path)
        {
            Path = path;
            LoadAttr();
            LoadSkillAttr();
        }

        public static void LoadSkillAttr()
        {
            SkillAttr = ReadSheetAsDict<string, SkillAttrItem>(Path, "SkillAttr", _ => _.FullID);
            SkillAttributeIdLib = SkillAttr.ToImmutableDictionary(_ => _.Key, _ => new SkillAttributeId(_.Value));
        }

        public static void LoadAttr()
        {
            Attr = ReadSheetAsDict<string, AttrItem>(Path, "Attr", _ => _.FullID);
            AttributeIDLib = Attr.ToImmutableDictionary(_ => _.Key, _ => new AttributeID(_.Value));
            EmptyAttributeID = AttributeID.Get("Empty");
            LuaID2FullID = Attr.Values.Where(_ => _.LuaID.IsNotEmptyOrWhiteSpace())
                .ToDictionary(_ => _.LuaID, _ => _.FullID).ToImmutableDictionary();
        }

        public static AttributeID GetAttributeID(string fullId)
        {
            return AttributeIDLib[fullId];
        }

        public static bool AttributeIsValue(string fullId)
        {
            var item = GetAttributeID(fullId);
            return item.IsValue;
        }

        public static SkillAttributeId GetSkillAttributeID(string fullId)
        {
            return SkillAttributeIdLib[fullId];
        }

        public static bool SkillAttributeIsValue(string fullId)
        {
            var item = GetSkillAttributeID(fullId);
            return item.IsValue;
        }

        /// <summary>
        /// 将以LuaID为键的字典转化为标准AttributeEntry对象集合
        /// </summary>
        /// <param name="dict">LuaID的字典</param>
        /// <param name="attributeEntryType">属性类型</param>
        /// <returns></returns>
        public static AttributeEntry[] GetAttributeEntriesFromLuaDict(IDictionary<string, int> dict,
            AttributeEntryTypeEnum attributeEntryType = AttributeEntryTypeEnum.Default)
        {
            var res = dict.Select(kvp =>
                new AttributeEntry(LuaID2FullID[kvp.Key], kvp.Value, attributeEntryType));
            return res.ToArray();
        }
    }
}