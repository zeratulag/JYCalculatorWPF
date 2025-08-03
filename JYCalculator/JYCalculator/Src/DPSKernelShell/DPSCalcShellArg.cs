using JX3CalculatorShared.Class;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Class;
using JYCalculator.Models;

namespace JYCalculator.Src
{
    public readonly struct DPSCalcShellArg
    {
        public readonly bool SL; // 是否有神力
        public readonly bool BigXW; // 是否为大心无

        public readonly int LongMen; // 龙门飞剑等级

        public readonly YZOption YZ; // 腰坠选项

        public readonly BigFMConfigModel BigFM;

        public readonly InitCharacter NoneBigFMInitCharacter; // 不含大附魔的初始人物属性

        public readonly BuffSpecialArg BuffSpecial;
        public readonly EquipSpecialEffectConfigArg EquipSpecialEffectConfig;

        public DPSCalcShellArg(bool sl, bool bigXw, int longMen, YZOption yz, BigFMConfigModel bigFm,
            BuffSpecialArg buffSpecial,
            InitCharacter ichar, EquipSpecialEffectConfigArg equipSpecialEffectConfig)
        {
            SL = sl;
            BigXW = bigXw;
            LongMen = longMen;
            YZ = yz;
            BigFM = bigFm;
            BuffSpecial = buffSpecial;
            NoneBigFMInitCharacter = ichar;
            EquipSpecialEffectConfig = equipSpecialEffectConfig;
        }

    }
}