using System.Collections.Generic;
using System.Collections.Immutable;
using JX3CalculatorShared.Globals;
using JX3PZ.Class;

namespace JX3PZ.Models
{
    public class XinFaAttribute
    {
        // 描述心法加成的类
        public readonly PrimaryTypeEnum Primary; // 主属性
        public readonly DamageSubTypeEnum Attack; // 攻击属性
        public readonly DamageSubTypeEnum Critical; // 会心属性
        public readonly DamageSubTypeEnum Overcome; // 破防属性

        public readonly int PrimaryToAdditionalAttack = 0; // 主属性加额外攻击的系数
        public readonly int PrimaryToAdditionalOvercome = 0; // 主属性到破防的系数
        public readonly int PrimaryToCriticalPoint = 0; // 主属性加会心等级的系数
        public readonly int NPC_DamageCoef = 0; // 非侠士加成
        public readonly double PZCoef = 0;

        public readonly ImmutableArray<AttributeEntry> AttributeEntries; // 心法加成

        public XinFaAttribute(PrimaryTypeEnum primary, DamageSubTypeEnum attack, DamageSubTypeEnum critical, DamageSubTypeEnum overcome,
            int primaryToAdditionalAttack, int primaryToAdditionalOvercome, int primaryToCriticalPoint,
            int DST_NPC_DAMAGE_COEFFICIENT, double pzCoef,
            IEnumerable<AttributeEntry> attributeEntries)
        {
            Primary = primary;
            Attack = attack;
            Critical = critical;
            Overcome = overcome;
            PrimaryToAdditionalAttack = primaryToAdditionalAttack;
            PrimaryToAdditionalOvercome = primaryToAdditionalOvercome;
            PrimaryToCriticalPoint = primaryToCriticalPoint;
            NPC_DamageCoef = DST_NPC_DAMAGE_COEFFICIENT;
            PZCoef = pzCoef;
            AttributeEntries = attributeEntries.ToImmutableArray();
        }

        public static XinFaAttribute CurrentXinFa; // 当前心法加成
    }
}