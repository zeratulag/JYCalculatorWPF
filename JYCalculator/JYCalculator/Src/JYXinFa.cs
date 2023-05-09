using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Documents;
using HandyControl.Tools.Extension;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JX3PZ.Class;
using JX3PZ.Globals;
using JX3PZ.Models;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace JYCalculator.Src
{
    public readonly struct LevelSkillData
    {
        public readonly int DecriticalDamagePowerBase;
        public readonly int nNeHit;
        public readonly int nPhysicsOvercome;
        public readonly int nPhysicsAttackPower;

        public LevelSkillData(int DecriticalDamagePowerBase, int nNeHit, int nPhysicsOvercome, int nPhysicsAttackPower)
        {
            this.DecriticalDamagePowerBase = DecriticalDamagePowerBase;
            this.nNeHit = nNeHit;
            this.nPhysicsOvercome = nPhysicsOvercome;
            this.nPhysicsAttackPower = nPhysicsAttackPower;
        }
    }

    public class JYXinFa
    {
        public static readonly ImmutableArray<LevelSkillData> SkillData; // tSkillData 
        public static readonly ImmutableArray<int> HuaJinData; // 化劲值 DecriticalDamagePowerBase

        #region 自动生成

        static JYXinFa()
        {
            SkillData = new[]
            {
                new LevelSkillData(DecriticalDamagePowerBase: 25, nNeHit: 10, nPhysicsOvercome: 13,
                    nPhysicsAttackPower: 8),
                new LevelSkillData(DecriticalDamagePowerBase: 44, nNeHit: 18, nPhysicsOvercome: 23,
                    nPhysicsAttackPower: 13),
                new LevelSkillData(DecriticalDamagePowerBase: 70, nNeHit: 28, nPhysicsOvercome: 36,
                    nPhysicsAttackPower: 21),
                new LevelSkillData(DecriticalDamagePowerBase: 95, nNeHit: 39, nPhysicsOvercome: 50,
                    nPhysicsAttackPower: 30),
                new LevelSkillData(DecriticalDamagePowerBase: 120, nNeHit: 49, nPhysicsOvercome: 63,
                    nPhysicsAttackPower: 41),
                new LevelSkillData(DecriticalDamagePowerBase: 146, nNeHit: 59, nPhysicsOvercome: 77,
                    nPhysicsAttackPower: 48),
                new LevelSkillData(DecriticalDamagePowerBase: 171, nNeHit: 70, nPhysicsOvercome: 90,
                    nPhysicsAttackPower: 58),
                new LevelSkillData(DecriticalDamagePowerBase: 197, nNeHit: 80, nPhysicsOvercome: 104,
                    nPhysicsAttackPower: 68),
                new LevelSkillData(DecriticalDamagePowerBase: 222, nNeHit: 91, nPhysicsOvercome: 117,
                    nPhysicsAttackPower: 76),
                new LevelSkillData(DecriticalDamagePowerBase: 491, nNeHit: 201, nPhysicsOvercome: 268,
                    nPhysicsAttackPower: 247),
                new LevelSkillData(DecriticalDamagePowerBase: 1078, nNeHit: 442, nPhysicsOvercome: 597,
                    nPhysicsAttackPower: 619),
                new LevelSkillData(DecriticalDamagePowerBase: 1078, nNeHit: 442, nPhysicsOvercome: 1327,
                    nPhysicsAttackPower: 1450),
                new LevelSkillData(DecriticalDamagePowerBase: 1078, nNeHit: 442, nPhysicsOvercome: 2929,
                    nPhysicsAttackPower: 3277),
            }.ToImmutableArray();

            HuaJinData =
                new int[] {18, 32, 51, 69, 87, 106, 124, 143, 161, 356, 782, 1725}
                    .ToImmutableArray(); // atDecriticalDamagePowerBase
        }

        #endregion

        public const int DST_NPC_DAMAGE_COEFFICIENT = 82; // 非侠士增伤
        public const int STRENGTH_TO_PHYSICS_ATTACK_POWER_COF = 1485; // 力道提高1.45外功AP
        public const int STRENGTH_TO_PHYSICS_CRITICAL_STRIKE_COF = 604; // 力道提高0.59会心
        public const int CURRENTLEVEL = 13; // 当前版本心法等级

        public readonly int Level = CURRENTLEVEL;
        public readonly int MAGIC_SHIELD = 0;
        public readonly int PHYSICS_SHIELD_BASE = 0;
        public readonly int MAX_LIFE_PERCENT_ADD = 0; // 基础气血提高
        public readonly int PHYSICS_OVERCOME_BASE = 0; // 外功破防
        public readonly int PHYSICS_ATTACK_POWER_BASE = 0; // 外功攻击 
        public readonly double HuaJinPct = 0.1; // 化劲率
        public readonly double PZCoef = 0.78 - 1; // 破招系数

        public readonly ImmutableDictionary<string, int> LuaDictionary; // 心法LUA属性集合
        public readonly ImmutableArray<AttributeEntry> AttributeEntries; // 心法属性集合

        public JYXinFa(int level = CURRENTLEVEL)
        {
            Level = level;

            if (Level >= 11)
            {
                MAGIC_SHIELD += 950;
                PHYSICS_SHIELD_BASE += 950;
            }

            MAX_LIFE_PERCENT_ADD = (Level * 1 + 10) * 1024 / 100;
            var sd = SkillData[Level - 1];
            PHYSICS_OVERCOME_BASE = sd.nPhysicsOvercome; // LUA表下标从1开始
            PHYSICS_ATTACK_POWER_BASE = sd.nPhysicsAttackPower;


            var luaDict = new Dictionary<string, int>()
            {
                {nameof(MAX_LIFE_PERCENT_ADD), MAX_LIFE_PERCENT_ADD}, // 基础气血提高
                {nameof(PHYSICS_SHIELD_BASE), PHYSICS_SHIELD_BASE}, // 外防
                {nameof(MAGIC_SHIELD), MAGIC_SHIELD}, // 内防
                {nameof(PHYSICS_ATTACK_POWER_BASE), PHYSICS_ATTACK_POWER_BASE}, // 外功基础攻击
                {nameof(PHYSICS_OVERCOME_BASE), PHYSICS_OVERCOME_BASE}, // 外功基础破防
            };

            LuaDictionary = luaDict.ToImmutableDictionary();
            AttributeEntries = GetAttributes().ToImmutableArray();
        }

        public List<AttributeEntry> GetAttributes()
        {
            var res = new List<AttributeEntry>(LuaDictionary.Count + 5);
            res.AddRange(AttributeIDLoader.GetAttributeEntriesFromLuaDict(LuaDictionary, AttributeEntryTypeEnum.XinFa));
            res.Add(new AttributeEntry("atPhysicsCriticalStrikeBaseRate", 100, AttributeEntryTypeEnum.Buff)); // 弩箭1%会心
            res.Add(new AttributeEntry("atDecriticalDamagePowerBaseKiloNumRate", 100,
                AttributeEntryTypeEnum.XinFa)); // 10%化劲
            return res;
        }

        public XinFaAttribute GetXinFaAttribute()
        {
            // 生成XinFaAttribute对象
            var primary = PrimaryTypeEnum.Strength;
            var attack = DamageSubTypeEnum.Physics;
            var critical = DamageSubTypeEnum.Physics;
            var overcome = DamageSubTypeEnum.Physics;
            int primaryToAdditionalAttack = STRENGTH_TO_PHYSICS_ATTACK_POWER_COF;
            int primaryToAdditionalOvercome = 0;
            int primaryToCriticalPoint = STRENGTH_TO_PHYSICS_CRITICAL_STRIKE_COF;
            var attributeEntries = AttributeEntries;
            var res = new XinFaAttribute(primary, attack, critical, overcome,
                primaryToAdditionalAttack, primaryToAdditionalOvercome, primaryToCriticalPoint,
                DST_NPC_DAMAGE_COEFFICIENT, PZCoef, attributeEntries);
            return res;
        }

        public static void GetCurrentXinFa()
        {
            // 获取当前心法加成，并且设定为全局的
            var xf = new JYXinFa();
            var xfAttr = xf.GetXinFaAttribute();
            XinFaAttribute.CurrentXinFa = xfAttr;
        }
    }
}