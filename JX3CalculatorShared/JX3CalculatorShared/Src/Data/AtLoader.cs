using JX3CalculatorShared.Class;
using System.Collections.Immutable;
using static JX3CalculatorShared.Utils.ImportTool;


namespace JX3CalculatorShared.Data
{
    public static class AtLoader
    {
        public static string AtFile;

        public static ImmutableDictionary<string, SkillAttrItem> SkillAttr;
        public static ImmutableDictionary<string, SkillAttrTemplate> SkillAttrTemplate;

        public static ImmutableDictionary<string, AttrItem> Attr;
        public static ImmutableDictionary<string, AttrTemplate> AttrTemplate;

        public static void Load(string path)
        {
            AtFile = path;
            LoadAttr();
            LoadSkillAttr();
        }

        public static void LoadSkillAttr()
        {
            SkillAttr = ReadSheetAsDict<string, SkillAttrItem>(AtFile, "SkillAttr", "FullID");
            SkillAttrTemplate = SkillAttr.ToImmutableDictionary(_ => _.Key, _ => new SkillAttrTemplate(_.Value));
        }
        public static void LoadAttr()
        {
            Attr = ReadSheetAsDict<string, AttrItem>(AtFile, "Attr", "FullID");
            AttrTemplate = Attr.ToImmutableDictionary(_ => _.Key, _ => new AttrTemplate(_.Value));
        }


        public static AttrItem GetAt(string fullid)
        {
            return Attr[fullid];
        }

        public static AttrTemplate GetAtTemplate(string fullid)
        {
            return AttrTemplate[fullid];
        }

        public static bool At_is_Value(string fullid)
        {
            var item = GetAtTemplate(fullid);
            return item.IsValue;
        }

        public static SkillAttrItem GetSkillAt(string fullid)
        {
            return SkillAttr[fullid];
        }

        public static SkillAttrTemplate GetSkillAttrTemplate(string fullid)
        {
            return SkillAttrTemplate[fullid];
        }

        public static bool SkillAt_is_Value(string fullid)
        {
            var item = GetSkillAttrTemplate(fullid);
            return item.IsValue;
        }

    }
}