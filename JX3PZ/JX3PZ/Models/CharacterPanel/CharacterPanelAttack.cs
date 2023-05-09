using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using System.Collections.Generic;

namespace JX3PZ.Models
{
    public class CharacterPanelAttack: CharacterPanelBase<DamageSubTypeEnum, PanelAttackSlot>
    {
        public PanelAttackSlot Physics { get; } // 外功
        public PanelAttackSlot Lunar { get; } // 阴性
        public PanelAttackSlot Solar { get; } // 阳性
        public PanelAttackSlot Neutral { get; } // 混元
        public PanelAttackSlot Poison { get; } // 毒性

        public CharacterPanelAttack()
        {
            Physics = new PanelAttackSlot(DamageSubTypeEnum.Physics);
            Lunar = new PanelAttackSlot(DamageSubTypeEnum.Lunar);
            Solar = new PanelAttackSlot(DamageSubTypeEnum.Solar);
            Neutral = new PanelAttackSlot(DamageSubTypeEnum.Neutral);
            Poison = new PanelAttackSlot(DamageSubTypeEnum.Poison);

            var dict = new Dictionary<DamageSubTypeEnum, PanelAttackSlot>()
            {
                {DamageSubTypeEnum.Physics, Physics},
                {DamageSubTypeEnum.Lunar, Lunar},
                {DamageSubTypeEnum.Solar, Solar},
                {DamageSubTypeEnum.Neutral, Neutral},
                {DamageSubTypeEnum.Poison, Poison},
            };
            Dict.Merge(dict);
        }

    }
}