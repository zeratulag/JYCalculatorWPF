using Force.DeepCloner;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Src.Data;
using JYCalculator.Src.Class;
using JYCalculator.Src.Data;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace JYCalculator.Src.DB
{
    public class TargetDB : IDB<string, Target>
    {
        #region 成员

        public readonly ImmutableDictionary<string, Target> Data;
        public readonly ImmutableArray<Target> Target;

        #endregion

        #region 构造
        public TargetDB(IEnumerable<TargetItem> itemdata)
        {
            Data = itemdata.ToImmutableDictionary(_ => _.Name, _ => new Target(_));
            Target = Data.Values.OrderBy(_ => _.Level).ToImmutableArray();
        }

        public TargetDB(JYDataLoader jyDataLoader) : this(jyDataLoader.Target)
        {
        }

        public TargetDB() : this(StaticJYData.Data)
        {
        }

        #endregion

        #region 取出

        public Target Get(string name)
        {
            var res = Data[name].DeepClone(); // 防止意外修改
            return res;
        }

        public Target Get(int level)
        {
            string name = @"M{level}";
            return Get(name);
        }

        public Target this[string name] => Get(name);

        #endregion
    }
}