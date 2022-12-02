using System.Collections.Generic;

namespace JYCalculator.Src.Class
{
    /// <summary>
    /// 用于存储导出与导入
    /// </summary>
    public class CalcData
    {
        #region 成员

        public Dictionary<int, string[]> SkillMiJiConfig;
        public int[] QiXueConfig;
        public InitCharacter InitChar;

        public string JBText;

        #endregion

        public CalcData()
        {
        }

        public static CalcData GetSample()
        {
            var res = new CalcData()
            {
                SkillMiJiConfig = new Dictionary<int, string[]>()
                {
                    {3093, new[] {"838_1", "835_1", "836_1", "842_1"}},
                    {3095, new[] {"843_1", "844_1", "850_1", "846_1"}},
                    {3096, new[] {"851_1", "852_1", "858_1", "5144_1"}},
                    {3098, new[] {"861_1", "859_1", "860_1", "867_1"}},
                    {3101, new[] {"1661_1", "1662_1", "1663_1", "1664_1"}}
                },
                QiXueConfig = new[] {2, 2, 3, 2, 1, 2, 4, 2, 2, 1, 1, 4},
                InitChar = new InitCharacter(4778, 15146, 23507, 1468.0,
                    0.4804, 1.8602, 0.5174,
                    4005, 15591, 289, false, false, ""),
                JBText =
                    @"{""Vitality"":34889,""Agility"":41,""Spirit"":41,""Spunk"":41,""Strength"":7318,""PhysicsAttackPowerBase"":20292,""PhysicsAttackPower"":30904,""PhysicsCriticalStrikeRate"":0.1854205221151706,""PhysicsCriticalDamagePowerPercent"":1.7993571396120123,""PhysicsOvercomePercent"":0.3839613342236637,""StrainPercent"":0.3225068180993744,""HastePercent"":0.0074934898363714095,""SurplusValue"":6413,""MaxHealth"":493241,""PhysicsShieldPercent"":0.061682128156294154,""LunarShieldPercent"":0.05403183576484102,""ToughnessDefCriticalPercent"":0,""DecriticalDamagePercent"":0.1,""MeleeWeaponAttackSpeed"":16,""MeleeWeaponDamage"":1550,""MeleeWeaponDamageRand"":1034,""EquipList"":{""HAT"":{""id"":""7_90835"",""stone"":"""",""enchant"":11684,""enhance"":11531,""strength"":6,""embedding"":[6,6]},""BELT"":{""id"":""7_90777"",""stone"":"""",""enchant"":11680,""enhance"":"""",""strength"":6,""embedding"":[6,6]},""SHOES"":{""id"":""7_90806"",""stone"":"""",""enchant"":11681,""enhance"":11586,""strength"":6,""embedding"":[6,6]},""WRIST"":{""id"":""7_90918"",""stone"":"""",""enchant"":11682,""enhance"":11579,""strength"":6,""embedding"":[6,6]},""JACKET"":{""id"":""7_90864"",""stone"":"""",""enchant"":11683,""enhance"":"""",""strength"":6,""embedding"":[6,6]},""RING_1"":{""id"":""8_34443"",""stone"":"""",""enchant"":"""",""enhance"":11660,""strength"":6,""embedding"":[]},""RING_2"":{""id"":""8_34303"",""stone"":"""",""enchant"":"""",""enhance"":11660,""strength"":6,""embedding"":[]},""BOTTOMS"":{""id"":""7_91333"",""stone"":"""",""enchant"":"""",""enhance"":11524,""strength"":6,""embedding"":[6,6]},""PENDANT"":{""id"":""8_34297"",""stone"":"""",""enchant"":"""",""enhance"":11654,""strength"":6,""embedding"":[6]},""NECKLACE"":{""id"":""8_34291"",""stone"":"""",""enchant"":"""",""enhance"":11652,""strength"":6,""embedding"":[6]},""PRIMARY_WEAPON"":{""id"":""6_33463"",""stone"":561,""enchant"":"""",""enhance"":11517,""strength"":6,""embedding"":[6,6,6]},""SECONDARY_WEAPON"":{""id"":""6_32774"",""stone"":"""",""enchant"":"""",""enhance"":11669,""strength"":6,""embedding"":[6]}},""Title"":""展锋毕业""}"
            };
            return res;
        }
    }
}