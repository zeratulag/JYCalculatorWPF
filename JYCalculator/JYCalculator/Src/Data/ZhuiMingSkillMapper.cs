using System.Collections.Generic;
using System.Linq;
using JYCalculator.Globals;

namespace JYCalculator.Data
{
    public class ZhuiMingSkillMapper
    {
        /// <summary>
        /// 将追命技能映射到BUFFID
        /// </summary>
        private static readonly Dictionary<int, string> NieJingLevelToBuffID = new Dictionary<int, string>
        {
            {1, "28225_1"},
            {2, "28226_1"},
            {3, "28227_1"}
        };

        private static readonly Dictionary<int, string> ShunFaToBuffID = new Dictionary<int, string>
        {
            {1, "17103_1"}
        };


        public static SkillBuffGroup GetBuffEntityList(int nieJingLevel, int shunFa, bool needParse = false)
        {
            NieJingLevelToBuffID.TryGetValue(nieJingLevel, out string nieJingBuff);
            ShunFaToBuffID.TryGetValue(shunFa, out string shunFaBuff);
            var buffIDList = new List<string> {nieJingBuff, shunFaBuff};
            buffIDList = buffIDList.Where(buff => buff != null).ToList();
            var skillBuffs = buffIDList.Select(e => new SkillBuff(e)).ToArray();
            if (needParse) skillBuffs.ForEach(e => e.AfterParse());
            var res = new SkillBuffGroup(skillBuffs);
            return res;
        }

        public static void BuildDerivedSkills()
        {
            var zm = StaticXFData.DB.BaseSkillInfo[SkillKeyConst.追命箭];
            for (int nieJingLevel = 0; nieJingLevel <= 3; nieJingLevel++)
            {
                for (int shunFa = 0; shunFa <= 1; shunFa++)
                {
                    var buffGroup = GetBuffEntityList(nieJingLevel, shunFa, needParse: true);
                    buffGroup.Parse();
                    var res = SkillInfoItemFactory.GetDerivedSkillInfo(zm, buffGroup);
                }
            }
        }
    }
}