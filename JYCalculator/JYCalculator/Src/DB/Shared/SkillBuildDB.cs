using System.Collections.Generic;
using System.Linq;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.DB;
using JYCalculator.Data;

namespace JYCalculator.DB
{
    public class SkillBuildDB: SkillBuildDBBase
    {
        public SkillBuildDB(XFDataLoader xfDataLoader) : base(xfDataLoader.SkillBuild)
        {
        }

        public SkillBuildDB() : this(StaticXFData.Data)
        {
        }

        public HashSet<SkillBuild> GetQiXueCompatible(HashSet<string> qiXueNames)
        {
            var res = Array.Where(_ => _.IsQiXueCompatible(qiXueNames)).ToHashSet();
            return res;
        }

        public HashSet<SkillBuild> GetRecipeCompatible(HashSet<string> recipeIDs)
        {
            var res = Array.Where(_ => _.IsMiJiCompatible(recipeIDs)).ToHashSet();
            return res;
        }
    }
}