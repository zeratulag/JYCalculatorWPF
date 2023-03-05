using System.Collections.Generic;
using System.Linq;

namespace JX3CalculatorShared.Class
{
    public class DPSTable
    {
        #region 成员

        public readonly Dictionary<string, DPSTableItem> Data;
        public DPSTableItem[] Items;

        public double DPS;

        #endregion

        #region 构造

        public DPSTable(params DPSTableItem[] items)
        {
            Items = items.ToArray();
            Data = Items.ToDictionary(_ => _.Name, _ => _);
        }

        public void Proceed()
        {
            DPS = 0;
            foreach (var _ in Items)
            {
                DPS += _.DPSContribute;
            }

            foreach (var _ in Items)
            {
                _.Proportion = _.DPSContribute / DPS;
            }
        }

        #endregion
    }
}