using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JX3CalculatorShared.DB
{
    public class ZhenFaDBBase : IDB<string, ZhenFa>
    {
        public ImmutableDictionary<string, ZhenFa> Data;
        public ImmutableArray<ZhenFa> ZhenFa;

        public ZhenFaDBBase(IDictionary<string, ZhenFa> zhenFaDict, IEnumerable<ZhenFa_dfItem> zhenFaDf)
        {
            Data = zhenFaDict.ToImmutableDictionary();

            var zf = zhenFaDf.Select(zhenFaDfItem => zhenFaDfItem.Name).Select(name => Data[name]);
            if (AppStatic.XinFaTag == "JY")
            {
                ZhenFa = zf.Where(_ => !_.IsOwn).ToImmutableArray(); // 惊羽暂时不考虑自己开阵，太复杂
            }
            else
            {
                ZhenFa = zf.ToImmutableArray();
            }

        }

        public ZhenFa Get(string name)
        {
            return Data[name];
        }

        public ZhenFa this[string name] => Get(name);
    }
}