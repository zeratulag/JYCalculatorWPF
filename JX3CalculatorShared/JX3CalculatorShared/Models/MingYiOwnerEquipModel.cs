using System.Collections.Generic;
using System.Linq;
using JX3CalculatorShared.Utils;
using MingYiTools;

namespace JX3CalculatorShared.Models
{
    public class MingYiOwnerEquipModel: MingYiOwnerEquipModelBase
    {
        public Dictionary<int, MingYiOwnerSuitEquipModel> SuitEquipModels { get; }

        public MingYiOwnerEquipModel(MingYiOwnerInfoEntity owner, MingYiEquipItemEntity[] equipItems) : base(owner, equipItems)
        {
            SuitEquipModels = new Dictionary<int, MingYiOwnerSuitEquipModel>(6);
        }

        public override void Calc()
        {
            GroupSuitEquips();
            CalcSuitEquipModels();
        }

        public void GroupSuitEquips()
        {
            var res = EquipItems.GroupBy(e => e.SuitIndex).ToDictionary(g=> g.Key, g => g.ToArray());
            for (int i = 0; i < 4; i++)
            {
                int suitIndex = i + 1;
                var suItems = res.GetValueOrUseDefault(suitIndex);
                var v = new MingYiOwnerSuitEquipModel(Owner, suItems, suitIndex);
                SuitEquipModels[i + 1] = v;
            }
        }

        public void CalcSuitEquipModels()
        {
            foreach (var v in SuitEquipModels.Values)
            {
                v.Calc();
            }
        }
    }
}