using JX3CalculatorShared.Data;
using System;
using System.Collections.Generic;

namespace JX3CalculatorShared.Class
{
    public class AttrWeightBase
    {
        public string Name { get; }
        public double BaseAttackPower { get; set; }
        public double BaseOvercome { get; set; } = 1;
        public double CriticalStrike { get; set; } = 1;
        public double CriticalPower { get; set; } = 1;
        public double BaseStrain { get; set; } = 1;
        public double BaseSurplus { get; set; } = 1;
        public double BaseWeaponDamage { get; set; } = 0;
        public double FinalAttackPower { get; set; } = double.NaN;
        public double FinalOvercome { get; set; } = double.NaN;
        public double FinalStrain { get; set; } = double.NaN;

        public string ToolTip { get; }

        private const string DiamondSuffix = "级孔";
        public bool IsDiamond = false;

        public AttrWeightBase(DiamondValueItemBase item)
        {
            Name = $"{item.Level}{DiamondSuffix}";
            BaseAttackPower = item.BaseAttackPower;
            BaseOvercome = item.BaseOvercome;
            CriticalStrike = item.CriticalStrike;
            CriticalPower = item.CriticalPower;

            BaseSurplus = item.BaseSurplus;
            BaseStrain = item.BaseStrain;
            BaseWeaponDamage = Double.NaN;

            ToolTip = $"{item.Level}级五行石镶嵌孔增加的DPS";

            IsDiamond = Name.EndsWith(DiamondSuffix);
        }

        public AttrWeightBase(string name, string toolTip = "")
        {
            Name = name;
            ToolTip = toolTip;
            IsDiamond = false;
        }


        /// <summary>
        /// 返回字典形式
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, double> ToDict()
        {
            var res = new Dictionary<string, double>()
            {
                {nameof(BaseAttackPower), BaseAttackPower},
                {nameof(BaseOvercome), BaseOvercome},
                {nameof(CriticalStrike), CriticalStrike},
                {nameof(CriticalPower), CriticalPower},
                {nameof(BaseStrain), BaseStrain},
                {nameof(BaseSurplus), BaseSurplus},
                {nameof(BaseWeaponDamage), BaseWeaponDamage},
                {nameof(FinalAttackPower), FinalAttackPower},
                {nameof(FinalOvercome), FinalOvercome},
                {nameof(FinalStrain), FinalStrain},
            };
            return res;
        }
    }
}