using MiniExcelLibs.Attributes;
using System.Collections.Generic;

namespace JX3CalculatorShared.Class
{
    public class DamageDerivBase
    {
        public string Name { get; set; }
        public double PhysicsBaseAttackPower { get; set; } // 基础攻击
        public double PhysicsBaseOvercome { get; set; } // 基础破防
        public double BaseSurplus { get; set; } // 破招
        public double FinalStrain { get; set; } // 最终无双
        public double BaseStrain { get; set; } // 基础无双
        public double PhysicsCriticalStrike { get; set; } // 会心
        public double PhysicsCriticalPower { get; set; } // 会效
        public double BaseWeaponDamage { get; set; } // 武器伤害
        public double PhysicsFinalAttackPower { get; set; } // 最终攻击
        public double PhysicsFinalOvercome { set; get; } // 最终破防
        [ExcelIgnore] public AttrProfitList ProfitList { get; protected set; }
        public string OrderDesc => ProfitList.OrderDesc;

        public AttrWeightBase Weight;
        public string ToolTip => Weight.ToolTip;

        public DamageDerivBase()
        {
        }
        public DamageDerivBase(string name)
        {
            Name = name;
        }

        public DamageDerivBase(DamageDerivBase old)
        {
            Name = old.Name;
            PhysicsFinalAttackPower = old.PhysicsFinalAttackPower;
            BaseWeaponDamage = old.BaseWeaponDamage;
            BaseSurplus = old.BaseSurplus;
            FinalStrain = old.FinalStrain;
            BaseStrain = old.BaseStrain;
            PhysicsCriticalStrike = old.PhysicsCriticalStrike;
            PhysicsCriticalPower = old.PhysicsCriticalPower;
            PhysicsFinalOvercome = old.PhysicsFinalOvercome;
            PhysicsBaseAttackPower = old.PhysicsBaseAttackPower;
            PhysicsBaseOvercome = old.PhysicsBaseOvercome;
        }

        public virtual DamageDerivBase Copy()
        {
            return new DamageDerivBase(this);
        }

        #region 方法

        /// <summary>
        /// 按照权重相加，用于合并求出最终收益
        /// </summary>
        /// <param name="other">另一个对象</param>
        /// <param name="w">权重系数（技能频率）</param>
        public virtual void WeightedAdd(DamageDerivBase other, double w = 1.0)
        {
            PhysicsFinalAttackPower += other.PhysicsFinalAttackPower * w;
            BaseWeaponDamage += other.BaseWeaponDamage * w;
            BaseSurplus += other.BaseSurplus * w;
            BaseStrain += other.BaseStrain * w;
            FinalStrain += other.FinalStrain * w;
            PhysicsCriticalStrike += other.PhysicsCriticalStrike * w;
            PhysicsCriticalPower += other.PhysicsCriticalPower * w;
            PhysicsFinalOvercome += other.PhysicsFinalOvercome * w;
            PhysicsBaseAttackPower += other.PhysicsBaseAttackPower * w;
            PhysicsBaseOvercome += other.PhysicsBaseOvercome * w;
        }

        // 根据属性加权求收益
        public virtual void ApplyAttrWeight(AttrWeightBase aw)
        {
            Weight = aw;
            Name = aw.Name;
            PhysicsBaseAttackPower *= aw.BaseAttackPower;
            PhysicsBaseOvercome *= aw.BaseOvercome;
            PhysicsCriticalStrike *= aw.CriticalStrike;
            PhysicsCriticalPower *= aw.CriticalPower;
            BaseStrain *= aw.BaseStrain;
            BaseSurplus *= aw.BaseSurplus;
            BaseWeaponDamage *= aw.BaseWeaponDamage;
            PhysicsFinalAttackPower *= aw.FinalAttackPower;
            PhysicsFinalOvercome *= aw.FinalOvercome;
            FinalStrain *= aw.FinalStrain;
        }


        #endregion


        public List<AttrProfitItem> GetPointAttrDerivListBase()
        {
            var l = new List<AttrProfitItem>
            {
                new AttrProfitItem(nameof(PhysicsFinalAttackPower), "最终攻击", PhysicsFinalAttackPower),
                new AttrProfitItem(nameof(PhysicsFinalOvercome), "最终破防", PhysicsFinalOvercome),
                new AttrProfitItem(nameof(PhysicsBaseAttackPower), "基础攻击", PhysicsBaseAttackPower),
                new AttrProfitItem(nameof(PhysicsBaseOvercome), "基础破防", PhysicsBaseOvercome),
                new AttrProfitItem(nameof(BaseSurplus), "破招", BaseSurplus),
                new AttrProfitItem(nameof(FinalStrain), "最终无双", FinalStrain),
                new AttrProfitItem(nameof(BaseStrain), "基础无双", BaseStrain),
                new AttrProfitItem(nameof(PhysicsCriticalStrike), "会心", PhysicsCriticalStrike),
                new AttrProfitItem(nameof(PhysicsCriticalPower), "会效", PhysicsCriticalPower),
                new AttrProfitItem(nameof(BaseWeaponDamage), "武伤", BaseWeaponDamage)
            };
            return l;
        }

        public List<AttrProfitItem> GetScoreAttrDerivListBase()
        {
            var l = new List<AttrProfitItem>
            {
                new AttrProfitItem(nameof(PhysicsBaseAttackPower), "攻击", PhysicsBaseAttackPower),
                new AttrProfitItem(nameof(PhysicsBaseOvercome), "破防", PhysicsBaseOvercome),
                new AttrProfitItem(nameof(BaseSurplus), "破招", BaseSurplus),
                new AttrProfitItem(nameof(BaseStrain), "无双", BaseStrain),
                new AttrProfitItem(nameof(PhysicsCriticalStrike), "会心", PhysicsCriticalStrike),
                new AttrProfitItem(nameof(PhysicsCriticalPower), "会效", PhysicsCriticalPower),
                new AttrProfitItem(nameof(BaseWeaponDamage), "武伤", BaseWeaponDamage)
            };
            return l;
        }

        public virtual List<double> GetValueArr()
        {
            var res = new List<double>(8)
            {
                PhysicsBaseAttackPower, PhysicsBaseOvercome,
                BaseSurplus, BaseStrain,
                PhysicsCriticalStrike, PhysicsCriticalPower,
            };
            return res;
        }

        public virtual List<string> GetDescArr()
        {
            var res = new List<string>(8)
            {
                "攻击", "破防",
                "破招", "无双",
                "会心", "会效",
                //"武伤"
            };
            return res;
        }

        public Dictionary<string, double> GetDict()
        {
            return ProfitList.Dict;
        }
    }
}