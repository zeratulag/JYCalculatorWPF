using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace JX3CalculatorShared.DB
{
    public class SkillModifierDBBase : IDB<string, SkillModifier>
    {
        public ImmutableDictionary<string, SkillModifier> Data;
        public ImmutableDictionary<string, string> QiXueToMods; // 表示奇穴与Mods名称的关联

        public SkillModifier Get(string name)
        {
            return Data[name];
        }

        /// <summary>
        /// 获取奇穴中含有哪些技能修饰效果
        /// </summary>
        /// <param name="qiXueNames">奇穴名</param>
        /// <returns>有效技能修饰效果</returns>
        public List<SkillModifier> GetQiXueMods(ISet<string> qiXueNames)
        {
            var res = new List<SkillModifier>();
            foreach (var KVP in QiXueToMods)
            {
                if (qiXueNames.Contains(KVP.Key))
                {
                    res.Add(Get(KVP.Value));
                }
            }
            return res;
        }

        public SkillModifier this[string name] => Get(name);
    }
}