using System.Linq;
using JX3CalculatorShared.Class;

namespace MingYiTools
{
    public class MingYiPZData
    {
        public MingYiItemData[] Items { get; }
        public MingYiPZData(MingYiItemData[] items)
        {
            Items = items;
        }

        public JBPZEquipSnapshotCollection MakeJBPZEquipSnapshotCollection()
        {
            var d = Items.ToDictionary(e => e.PositionName, e => new JBPZEquipSnapshot(e));
            var res = new JBPZEquipSnapshotCollection(d);
            return res;
        }

        public JBBB ConvertToJBBB()
        {
            var c = MakeJBPZEquipSnapshotCollection();
            var res = new JBBB
            {
                EquipList = c
            };
            return res;
        }
    }
}