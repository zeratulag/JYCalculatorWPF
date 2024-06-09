using System.Collections.Generic;

namespace JX3PZ.Models
{
    public class PanelPVXAllRoundSlot : PanelAttributeSlot
    {
        public new const string Name = "PVXAllRound";
        public const string Suffix = "";
        public const double ConvertToStrainCoef = 1.0;
        public const double ConvertToSurplusValueCoef = 1.0;
        public const double ConvertToDecriticalDamageCoef = 1.0;

        public const string StrainKey = "atStrainBase";
        public const string SurplusValueKey = "atSurplusValueBase";
        public const string DecriticalDamageKey = "atDecriticalDamagePowerBase";


        public int Strain;
        public int SurplusValue;
        public int DecriticalDamage;

        public Dictionary<string, int> ConvertDict = new Dictionary<string, int>();

        public PanelPVXAllRoundSlot() : base(Name, Suffix)
        {
        }

        public override List<string> GetDescTips()
        {
            var res = new List<string>
            {
                $"全能",
                $"破招值提高{SurplusValue:F0}",
                $"无双等级提高{Strain:F0}",
                $"化劲等级提高{DecriticalDamage:F0}"
            };
            return res;
        }

        public override void UpdateFrom(IDictionary<string, int> valueDict)
        {
            base.UpdateFrom(valueDict);
            CalcPVXAllRound();
            MakeDict();
        }

        public void MakeDict()
        {
            ConvertDict = GetPVXAllRoundDict();
        }

        public void CalcPVXAllRound()
        {
            Strain = (int) (Final * ConvertToStrainCoef);
            SurplusValue = (int) (Final * ConvertToSurplusValueCoef);
            DecriticalDamage = (int) (Final * ConvertToDecriticalDamageCoef);
        }

        public Dictionary<string, int> GetPVXAllRoundDict()
        {
            var res = new Dictionary<string, int>(5)
            {
                {StrainKey, Strain}, {SurplusValueKey, SurplusValue}, {DecriticalDamageKey, DecriticalDamage}
            };
            return res;
        }
    }
}