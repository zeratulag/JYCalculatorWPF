using JX3PZ.Class;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JX3PZ.Data
{
    public class pzEquipData
    {
        public ImmutableArray<Equip>[] Equips; // 基于SubType的分表
        public ImmutableArray<int> EquipDiamonds; // 每个SubType装备有多少个孔

        public pzEquipData()
        {
            var n = EquipMapLib.MAX_SUB_TYPE;
            Equips = new ImmutableArray<Equip>[n + 1];
        }

        public void Load(IEnumerable<Equip> equips)
        {
            var q = from _ in equips group _ by _.SubType;
            foreach (var _ in q)
            {
                Equips[_.Key] = _.OrderBy(e => e.Level).ThenBy(e => e.ID).Where(e => e.IsValid).ToImmutableArray();
            }

            var arr = new int[Equips.Length];
            for (int i = 0; i < Equips.Length; i++)
            {
                if (Equips[i] != null)
                {
                    var eq = Equips[i].Last();
                    if (eq.Attributes.DiamondID != null)
                    {
                        arr[i] = eq.Attributes.DiamondID.Length;
                    }
                }
            }

            EquipDiamonds = arr.ToImmutableArray();
        }

        public ImmutableArray<Equip> this[int sub] => Equips[sub];
    }
}