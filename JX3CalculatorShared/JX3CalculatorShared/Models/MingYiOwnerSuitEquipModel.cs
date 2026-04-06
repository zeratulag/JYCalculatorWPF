using System.Linq;
using JX3CalculatorShared.Class;
using MingYiTools;

namespace JX3CalculatorShared.Models
{
    public class MingYiOwnerSuitEquipModel: MingYiOwnerEquipModelBase
    {
        public int SuitIndex { get; }
        public int Score { get; }
        
        public string OwnerName { get; }
        public string DescName { get; }

        public MingYiEquipItemEntity[] ValidEquipItems { get; private set; }

        public MingYiOwnerSuitEquipModel(MingYiOwnerInfoEntity owner, MingYiEquipItemEntity[] equipItems, int suitIndex) : base(owner, equipItems)
        {
            SuitIndex = suitIndex;
            if (owner == null) return;
            OwnerName = owner.OwnerName;
            Score = owner.ValidScores[SuitIndex];
            DescName = $@"{OwnerName} - 第{SuitIndex}套装备 - 装分{Score}";
        }

        public override void Calc()
        {
            if (Owner is null)
            {
                ValidEquipItems = null;
                return;
            }
            FindValidItems();
            ConvertToJBBB();
        }

        private void FindValidItems()
        {
            ValidEquipItems = EquipItems.Where(e => e.IsValidSlot).OrderBy(e => e.EquipSlot).ToArray();
        }

        public JBPZEquipSnapshotCollection MakeJBPZEquipSnapshotCollection()
        {
            var d = ValidEquipItems.ToDictionary(e => e.EquipSlot.ToString(), e => new JBPZEquipSnapshot(e));
            var res = new JBPZEquipSnapshotCollection(d);
            return res;
        }

        public JBBB ConvertToJBBB()
        {
            var c = MakeJBPZEquipSnapshotCollection();
            var res = new JBBB
            {
                EquipList = c,
                Title = Owner.OwnerName
            };
            return res;
        }

    }
}