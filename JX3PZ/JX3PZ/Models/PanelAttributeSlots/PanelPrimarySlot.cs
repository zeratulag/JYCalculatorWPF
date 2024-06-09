using JX3CalculatorShared.Globals;
using JX3PZ.Class;
using System.Collections.Generic;
using System.Linq;

namespace JX3PZ.Models
{
    public class PanelPrimarySlot : PanelAttributeSlot // 主属性
    {
        public static readonly string Suffix = "Base";
        public readonly PrimaryTypeEnum PrimaryType;

        public Dictionary<string, int> SystemPrimaryAttributeValues { get; private set; }

        public PanelPrimarySlot(string name) : base(name, keySuffix: Suffix)
        {
            ExtraKey.Add("atBasePotentialAdd");
        }

        public PanelPrimarySlot(PrimaryTypeEnum primaryType) : this(primaryType.ToString())
        {
            PrimaryType = primaryType;
        }

        public Dictionary<string, int> GetSystemPrimaryAttributeValues()
        {
            var res = new Dictionary<string, int>(2);
            // 计算系统自带的主属性加成

            switch (PrimaryType)
            {
                case PrimaryTypeEnum.Vitality:
                    {
                        res.Add("atMaxLifeBase", Final * SystemPrimaryAttribute.VitalityToMaxLifeBase);
                        break;
                    }
                case PrimaryTypeEnum.Spirit:
                    {
                        res.Add("atMagicCriticalStrike",
                            (int)(Final * SystemPrimaryAttribute.SpiritToMagicCriticalStrike));
                        break;
                    }
                case PrimaryTypeEnum.Strength:
                    {
                        res.Add("atPhysicsAttackPowerBase",
                            (int)(Final * SystemPrimaryAttribute.StrengthToPhysicsAttackPower));
                        res.Add("atPhysicsOvercomeBase", (int)(Final * SystemPrimaryAttribute.StrengthToPhysicsOvercome));
                        break;
                    }

                case PrimaryTypeEnum.Agility:
                    {
                        res.Add("atPhysicsCriticalStrike",
                            (int)(Final * SystemPrimaryAttribute.AgilityToPhysicsCriticalStrike));
                        break;
                    }
                case PrimaryTypeEnum.Spunk:
                    {
                        res.Add("atMagicAttackPowerBase", (int)(Final * SystemPrimaryAttribute.SpunkToMagicAttackPower));
                        res.Add("atMagicOvercome", (int)(Final * SystemPrimaryAttribute.SpunkToMagiOvercome));
                        break;
                    }
            }

            SystemPrimaryAttributeValues = res;

            return res;
        }


        public override List<string> GetDescTips()
        {
            var res = base.GetDescTips();

            if (SystemPrimaryAttributeValues == null)
            {
                GetSystemPrimaryAttributeValues();
            }

            var entries = SystemPrimaryAttributeValues.Select(kvp => new AttributeEntry(kvp.Key, kvp.Value)).ToList();
            var desc = entries.Select(_ => _.GetDesc()).ToList();
            res.Add(StringConsts.TooltipDivider0);
            res.Add("属性加成：");

            if (PrimaryType == PrimaryTypeEnum.Spirit || PrimaryType == PrimaryTypeEnum.Agility)
            {
                // 根骨和身法加会心点数，体质力道元气加的都是基础点数
                res.AddRange(desc);
            }
            else
            {
                res.AddRange(desc.Select(_ => $"基础{_}"));
            }

            return res;
        }
    }


    public class XinFaPrimaryAttributeValue
    {
        // 由心法带来的主属性加成

        public readonly XinFaAttribute XF;
        public readonly int PrimaryAttributeValue; // 最终主属性值

        public readonly int AdditionalAttack; // 心法额外攻击
        public readonly int CriticalStrikePoint; // 心法加的会心点数
        public readonly int AdditionalOvercome; // 心法加的额外破防点数

        public XinFaPrimaryAttributeValue(XinFaAttribute xf, int primaryAttributeValue)
        {
            XF = xf;
            PrimaryAttributeValue = primaryAttributeValue;
            AdditionalAttack = XF.PrimaryToAdditionalAttack * PrimaryAttributeValue / 1024;
            CriticalStrikePoint = XF.PrimaryToCriticalPoint * PrimaryAttributeValue / 1024;
            AdditionalOvercome = XF.PrimaryToAdditionalOvercome * PrimaryAttributeValue / 1024;
        }
    }
}