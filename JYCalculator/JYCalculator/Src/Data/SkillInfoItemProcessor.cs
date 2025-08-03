using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using Serilog;

namespace JYCalculator.Data
{
    public static class SkillInfoItemProcessor
    {
        public static void ProcessAdd_CT(this SkillInfoItem item, double value)
        {
            item.Add_CT += value;
        }

        public static void ProcessAdd_Dmg(this SkillInfoItem item, double value)
        {
            item.Add_Dmg += value;
        }

        public static void ProcessAdd_CF(this SkillInfoItem item, double value)
        {
            item.Add_CF += value;
        }

        public static void ProcessFrame(this SkillInfoItem item, double value)
        {
            item.Frame += (int) value;
            item.Interval += value / StaticConst.FRAMES_PER_SECOND;
        }

        public static void ProcessAdd_NPC_Dmg(this SkillInfoItem item, double value)
        {
            item.Add_NPCDmg += value;
        }

        public static void ProcessCoef(this SkillInfoItem item, double value)
        {
            item.nChannelInterval *= value;
            item.AP_Coef *= value;
        }
    }

    public delegate void SkillInfoItemProcessorDelegate(SkillInfoItem item, double value);

    public static class SkillInfoItemProcessorManager
    {
        public static readonly ImmutableDictionary<string, SkillInfoItemProcessorDelegate> ModifierDict;

        static SkillInfoItemProcessorManager()
        {
            var dict = new Dictionary<string, SkillInfoItemProcessorDelegate>()
            {
                {"Add_CT", SkillInfoItemProcessor.ProcessAdd_CT},
                {"Add_Dmg", SkillInfoItemProcessor.ProcessAdd_Dmg},
                {"Add_CF", SkillInfoItemProcessor.ProcessAdd_CF},
                {"Frame", SkillInfoItemProcessor.ProcessFrame},
                {"nPrepareFrames", SkillInfoItemProcessor.ProcessFrame},
                {"Add_NPC_Dmg", SkillInfoItemProcessor.ProcessAdd_NPC_Dmg},
                {"Coef", SkillInfoItemProcessor.ProcessCoef},
            };
            ModifierDict = dict.ToImmutableDictionary();
        }

        public static void ProcessSkillAttr(this SkillInfoItem item, string key, double value)
        {
            bool success = ModifierDict.TryGetValue(key, out SkillInfoItemProcessorDelegate modifier);
            if (success)
            {
                modifier(item, value);
            }
            else
            {
                //Log.Information($"无效的属性！ {key}:{value} ");
            }
        }

        public static void ProcessSkillAttrDict(this SkillInfoItem item, IDictionary<string, double> dict)
        {
            foreach (var kvp in dict)
            {
                ProcessSkillAttr(item, kvp.Key, kvp.Value);
            }
        }

        public static void ProcessSkillModifier(this SkillInfoItem item, SkillModifier modifier)
        {
            ProcessSkillAttrDict(item, modifier.SAttrs);
        }

        public static void ProcessCoefList(this SkillInfoItem item, List<object> vList)
        {
            foreach (double v in vList)
            {
                item.ProcessCoef(v);
            }
        }

        public static void ProcessRecipe(this SkillInfoItem item, Recipe recipe)
        {
            ProcessSkillAttrDict(item, recipe.SSkillAttrs.Values);
            var hasCoef = recipe.SSkillAttrs.Others.TryGetValue("Coef", out List<object> vList);
            if (hasCoef)
            {
                item.ProcessCoefList(vList);
            }
        }
    }
}