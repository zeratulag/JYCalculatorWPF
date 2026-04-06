using JX3CalculatorShared.Common;
using MingYiTools;

namespace JX3CalculatorShared.Models
{
    public class MingYiOwnerEquipModelBase : IModel
    {
        public MingYiOwnerInfoEntity Owner { get; }
        public MingYiEquipItemEntity[] EquipItems { get; }

        public MingYiOwnerEquipModelBase(MingYiOwnerInfoEntity owner, MingYiEquipItemEntity[] equipItems)
        {
            Owner = owner;
            EquipItems = equipItems;
        }

        public virtual void Calc()
        {
        }
    }
}