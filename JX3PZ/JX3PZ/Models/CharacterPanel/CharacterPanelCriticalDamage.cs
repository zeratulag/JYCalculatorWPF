using System.Collections.Generic;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;

namespace JX3PZ.Models
{
    public class CharacterPanelCriticalDamage: CharacterPanelBase<DamageSubTypeEnum, PanelCriticalDamageSlot>
    {
        public PanelCriticalDamageSlot Physics { get; } // 外功
        public PanelCriticalDamageSlot Lunar { get; } // 阴性
        public PanelCriticalDamageSlot Solar { get; } // 阳性
        public PanelCriticalDamageSlot Neutral { get; } // 混元
        public PanelCriticalDamageSlot Poison { get; } // 毒性

        public CharacterPanelCriticalDamage()
        {
            Physics = new PanelCriticalDamageSlot(DamageSubTypeEnum.Physics);
            Lunar = new PanelCriticalDamageSlot(DamageSubTypeEnum.Lunar);
            Solar = new PanelCriticalDamageSlot(DamageSubTypeEnum.Solar);
            Neutral = new PanelCriticalDamageSlot(DamageSubTypeEnum.Neutral);
            Poison = new PanelCriticalDamageSlot(DamageSubTypeEnum.Poison);

            var dict = new Dictionary<DamageSubTypeEnum, PanelCriticalDamageSlot>()
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