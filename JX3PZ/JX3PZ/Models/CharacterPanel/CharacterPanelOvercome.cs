using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace JX3PZ.Models
{
    public class CharacterOvercome : CharacterPanelBase<DamageSubTypeEnum, PanelOvercomeSlot>
    {
        public PanelOvercomeSlot Physics { get; } // 外功
        public PanelOvercomeSlot Lunar { get; } // 阴性
        public PanelOvercomeSlot Solar { get; } // 阳性
        public PanelOvercomeSlot Neutral { get; } // 混元
        public PanelOvercomeSlot Poison { get; } // 毒性

        public CharacterOvercome()
        {
            Physics = new PanelOvercomeSlot(DamageSubTypeEnum.Physics);
            Lunar = new PanelOvercomeSlot(DamageSubTypeEnum.Lunar);
            Solar = new PanelOvercomeSlot(DamageSubTypeEnum.Solar);
            Neutral = new PanelOvercomeSlot(DamageSubTypeEnum.Neutral);
            Poison = new PanelOvercomeSlot(DamageSubTypeEnum.Poison);

            var dict = new Dictionary<DamageSubTypeEnum, PanelOvercomeSlot>()
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