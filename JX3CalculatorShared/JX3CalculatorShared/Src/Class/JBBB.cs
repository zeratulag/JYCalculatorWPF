using Newtonsoft.Json;
using System.Diagnostics;

namespace JX3CalculatorShared.Class
{
    public class JBBB
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
        public double PhysicsCriticalStrike { get; set; }
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
        public JBPZEquipSnapshotCollection EquipList { get; set; }
        public string Title { get; set; }
        public int PhysicsOvercome { get; set; }
        public int PVXAllRound { get; set; }
        public int Strain { get; set; }
        public int Haste { get; set; }
        public int Score { get; set; }

        public JBBB()
        {
        }

        /// <summary>
        /// 计算会心率（指的是非装备提供的会心百分比，常规状态就是弩箭1%会心）
        /// </summary>
        /// <param name="physicsCriticalStrikeCeof">会心转换系数，例如130级是 197703.00 </param>
        /// <returns></returns>
        public double CalcPhysicsCriticalStrikeRate(double physicsCriticalStrikeCeof)
        {
            double res = PhysicsCriticalStrikeRate - PhysicsCriticalStrike / physicsCriticalStrikeCeof;
            return res;
        }

        public static (bool Success, JBBB J) TryReadFromJSON(string jsontxt)
        {
            JBBB res = null;
            bool success = false;

            try
            {
                res = JsonConvert.DeserializeObject<JBBB>(jsontxt);
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