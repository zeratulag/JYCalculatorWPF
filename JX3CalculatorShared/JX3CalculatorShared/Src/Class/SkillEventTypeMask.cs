using JX3CalculatorShared.Globals;
using JYCalculator.Data;
using System.Collections.Generic;
using System.Xml.Linq;

namespace JX3CalculatorShared.Data
{
    public class SkillEventTypeMask
    {
        public SkillEventTypeEnum EventType { get; }
        public ulong EventMask1 { get; }
        public ulong EventMask2 { get; }
        public string Name { get; }
        public HashSet<string> TriggerSkillNames { get; } // 可以触发事件的技能Name
        public HashSet<int> TriggerSkillIDs { get; } // 可以触发的技能ID
        public List<SkillEventItem> SkillEventItems { get; } // 关联的触发列表

        public SkillEventTypeMask(SkillEventTypeEnum eventType, ulong eventMask1, ulong eventMask2)
        {
            EventType = eventType;
            EventMask1 = eventMask1;
            EventMask2 = eventMask2;
            Name = $"{eventType.ToString()}@{eventMask1}_{eventMask2}";
            TriggerSkillNames = new HashSet<string>(50);
            TriggerSkillIDs = new HashSet<int>(100);
            SkillEventItems = new List<SkillEventItem>();
        }

        public bool CanTrigger(SkillInfoItemBase skillItem)
        {
            return skillItem.CanTrigger(this);
        }
    }
}