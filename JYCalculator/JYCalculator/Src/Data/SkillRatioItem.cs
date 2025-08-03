using System.Collections.Immutable;
using System.Linq;
using Newtonsoft.Json;

namespace JYCalculator.Data
{
    public class SkillRatioItem
    {
        public string Buffs { get; set; }
        public string BuffNames { get; set; }
        public string Skill { get; set; }
        public string SkillName { get; set; }
        public int XinWu { get; set; }
        public int ChengWu { get; set; }
        public int Num { get; set; }
        public string BaseSkillName { get; set; }
        public int BaseSkillNum { get; set; }
        public double BaseSkillRatio { get; set; }
        public string BaseSkillKey { get; set; }

        public ImmutableArray<SkillBuff> SkillBuffs { get; private set; }
        public SkillBuffGroup BuffGroup { get; private set; }
        public SkillInfoItem BasicSkillInfo { get; private set; }
        public SkillInfoItem DerivedSkillInfo { get; private set; }

        public void Parse()
        {
            // 初次解析
            var BuffList = JsonConvert.DeserializeObject<string[]>(Buffs);
            var skillBuffs = BuffList.Select(SkillBuffFactory.Create).ToList();
            SkillBuffs = SkillBuffGroup.UniqueSorted(skillBuffs).ToImmutableArray();
            BuffGroup = new SkillBuffGroup(SkillBuffs);
        }

        public void AfterParse()
        {
            ParseSkillInfo();
            AfterParseSkillBuff();
            MakeDerivedSkillInfo();
        }

        private void ParseSkillInfo()
        {
            BasicSkillInfo = StaticXFData.DB.BaseSkillInfo[BaseSkillKey];
        }

        public void AfterParseSkillBuff()
        {
            // 二次解析
            BuffGroup.Parse();
        }

        /// <summary>
        /// 构造生成的SkillInfoItem
        /// </summary>
        public void MakeDerivedSkillInfo()
        {
            DerivedSkillInfo = SkillInfoItemFactory.GetDerivedSkillInfo(BasicSkillInfo, BuffGroup);
        }

    }
}