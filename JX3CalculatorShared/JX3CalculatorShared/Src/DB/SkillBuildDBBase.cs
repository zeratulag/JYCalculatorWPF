using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JX3CalculatorShared.DB
{
    public class SkillBuildDBBase: IDB<string, SkillBuild>
    {
        public ImmutableDictionary<string, SkillBuild> Data;
        public ImmutableArray<SkillBuild> Array;

        public int DefaultIndex = 0;
        public SkillBuild Default => Array[DefaultIndex]; // 默认流派，为了伤害计算服务

        public SkillBuildDBBase(IEnumerable<SkillBuildItem> items)
        {
            var res = items.Select(_ => new SkillBuild(_));
            Array = res.ToImmutableArray();
            Data = Array.ToImmutableDictionary(_ => _.Name);
            FindDefault();
        }

        public void FindDefault()
        {
            for (int i = 0; i < Array.Length; i++)
            {
                if (Array[i].IsDefault)
                {
                    DefaultIndex = i;
                }
            }
        }


        public SkillBuild Get(string name)
        {
            return Data[name];
        }
    }
}