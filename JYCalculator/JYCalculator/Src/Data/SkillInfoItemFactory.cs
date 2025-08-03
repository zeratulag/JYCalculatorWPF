using System.Collections.Generic;
using System.Xml.Linq;

namespace JYCalculator.Data
{
    public static class SkillInfoItemFactory
    {
        public static readonly Dictionary<string, SkillInfoItem> Dict = new Dictionary<string, SkillInfoItem>(50);

        public static SkillInfoItem GetDerivedSkillInfo(SkillInfoItem item, SkillBuffGroup group)
        {
            if (group == null || group.IsEmpty) return item;
            var derivedName = item.GetDerivedSkillInfoName(group);
            if (Dict.TryGetValue(derivedName, out var res)) return res;
            var result = item.MakeDerivedSkillInfo(group);
            Dict.Add(derivedName, result);
            return result;
        }
    }
}