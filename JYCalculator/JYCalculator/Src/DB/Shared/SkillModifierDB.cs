using JX3CalculatorShared.DB;
using JYCalculator.Data;
using System.Collections.Immutable;

namespace JYCalculator.DB
{
    public class SkillModifierDB : SkillModifierDBBase
    {
        public SkillModifierDB()
        {
            Data = StaticXFData.Data.SkillModifier;
            var qx2MB = ImmutableDictionary.CreateBuilder<string, string>();
            foreach (var KVP in Data)
            {
                if (KVP.Value.Type == "奇穴")
                {
                    qx2MB.Add(KVP.Value.Associate, KVP.Key);
                }
            }

            QiXueToMods = qx2MB.ToImmutable();
        }
    }
}