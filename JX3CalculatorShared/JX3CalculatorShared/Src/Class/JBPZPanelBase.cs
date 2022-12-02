using Newtonsoft.Json;
using System.Diagnostics;

namespace JX3CalculatorShared.Src.Class
{
    public class JBPZPanelBase
    {
        /// <summary>
        /// JBBB标准格式（通用）
        /// </summary>
        public double Agility { get; set; }
        public double Spirit { get; set; }
        public double Spunk { get; set; }
        public double Strength { get; set; }
        public double PoisonAttackPowerBase { get; set; }
        public double PoisonAttackPower { get; set; }
        public double PhysicsAttackPowerBase { get; set; }
        public double PhysicsAttackPower { get; set; }
        public double PhysicsCriticalStrikeRate { get; set; }
        public double PhysicsCriticalDamagePowerPercent { get; set; }
        public double PoisonOvercomePercent { get; set; }
        public double PhysicsOvercomePercent { get; set; }
        public double StrainPercent { get; set; }
        public double HastePercent { get; set; }
        public double SurplusValue { get; set; }
        public double MaxHealth { get; set; }
        public double PhysicsShieldPercent { get; set; }
        public double PoisonShieldPercent { get; set; }
        public double ToughnessDefCriticalPercent { get; set; }
        public double DecriticalDamagePercent { get; set; }
        public double MeleeWeaponAttackSpeed { get; set; }
        public double MeleeWeaponDamage { get; set; }
        public double MeleeWeaponDamageRand { get; set; }
        public JBPZPlanBase EquipList { get; set; }
        public string Title { get; set; }

        public static (bool success, JBPZPanelBase panel) TryImportFromJSON(string jsontxt)
        {
            JBPZPanelBase res = null;
            bool success = false;

            try
            {
                res = JsonConvert.DeserializeObject<JBPZPanelBase>(jsontxt);
            }
            catch
            {
                Trace.Write("格式错误！");
            }

            if (res != null)
            {
                success = true;
            }

            return (success, res);
        }
    }
}

namespace JX3CalculatorShared.Class
{
    public class JBPZEquipSnapshot
    {
        public string id { get; set; } // 装备ID 6_31719
        public int? stone { get; set; } = -1; // 五彩石ID
        public int? enchant { get; set; } = -1; // 大附魔信息
        public int? enhance { get; set; } = 1; // 小附魔信息
        public int strength { get; set; } = 0; // 精炼等级
        public int[] embedding { get; set; } // 装备镶嵌
    }
}