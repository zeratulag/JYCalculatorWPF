using JX3CalculatorShared.Data;
using JX3CalculatorShared.Models;
using JX3CalculatorShared.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace JX3CalculatorShared.Class
{
    public class SkillBuild
    {
        public string Name { get; }
        public string Build { get; }
        public string GameVersion { get; }

        public bool IsDefault { get; }

        public int Priority { get; }

        public readonly int NumOfEssentialQiXues;
        public readonly int NumOfEssentialMiJis;
        public readonly ImmutableHashSet<string> EssentialQiXues;
        public readonly ImmutableHashSet<string> BannedQiXues;
        public readonly ImmutableHashSet<string> EssentialMiJis;
        public readonly SkillBuildSav Sav;


        public SkillBuild(SkillBuildItem item)
        {
            Name = item.Name;
            Build = item.Build;
            GameVersion = item.GameVersion;
            Priority = item.Priority;
            IsDefault = item.IsDefault > 0;

            EssentialQiXues = StringTool.ParseStringListAsImmutableHashSet(item.RawEssentialQiXues);
            BannedQiXues = StringTool.ParseStringListAsImmutableHashSet(item.RawBannedQiXues);
            EssentialMiJis = StringTool.ParseStringListAsImmutableHashSet(item.RawEssentialMiJis);

            NumOfEssentialQiXues = EssentialQiXues.Count;
            NumOfEssentialMiJis = EssentialMiJis.Count;
            Sav = JsonConvert.DeserializeObject<SkillBuildSav>(item.RawSav);
        }

        // 判断奇穴是否符合此Build
        public bool IsQiXueCompatible(HashSet<string> qiXueNames)
        {
            var essential = EssentialQiXues.Intersect(qiXueNames);
            var banned = BannedQiXues.Intersect(qiXueNames);
            var hasAllEssential = essential.Count == NumOfEssentialQiXues;
            var hasNoBanned = banned.Count == 0;
            return hasAllEssential && hasNoBanned;
        }

        public bool IsMiJiCompatible(HashSet<string> recipeIDs)
        {
            var essential = EssentialMiJis.Intersect(recipeIDs);
            var hasAllEssential = essential.Count == NumOfEssentialMiJis;
            return hasAllEssential;
        }

    }
}