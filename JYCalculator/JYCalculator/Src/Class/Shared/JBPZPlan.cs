using JX3CalculatorShared.Class;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace JYCalculator.Class
{
    public class JBPZPlan : JBPZEquipSnapshotCollection
    {
        #region 成员

        [JsonIgnore] public EquipOptionConfigSav EquipOptionSav;

        [JsonIgnore] public BigFMSlotConfig[] BigFMConfig;

        public JBPZPlan(JBPZEquipSnapshotCollection equipSnapshotCollection)
        {
            if (equipSnapshotCollection != null)
            {
                HAT = equipSnapshotCollection.HAT;
                BELT = equipSnapshotCollection.BELT;
                SHOES = equipSnapshotCollection.SHOES;
                WRIST = equipSnapshotCollection.WRIST;
                JACKET = equipSnapshotCollection.JACKET;
                RING_1 = equipSnapshotCollection.RING_1;
                RING_2 = equipSnapshotCollection.RING_2;
                BOTTOMS = equipSnapshotCollection.BOTTOMS;
                PENDANT = equipSnapshotCollection.PENDANT;
                NECKLACE = equipSnapshotCollection.NECKLACE;
                PRIMARY_WEAPON = equipSnapshotCollection.PRIMARY_WEAPON;
                SECONDARY_WEAPON = equipSnapshotCollection.SECONDARY_WEAPON;
                TERTIARY_WEAPON = equipSnapshotCollection.TERTIARY_WEAPON;
            }
        }

        public JBPZPlan(JBBB j) : this(j?.EquipList) { }

        #endregion

        #region 方法

        // 获取武器特效名称
        public string GetWPOptionName()
        {
            var res = StaticXFData.DB.EquipOption.GetWPNameByEquipID(PRIMARY_WEAPON?.id);
            return res;
        }

        // 获取腰坠特效名称
        public string GetYZOptionName()
        {
            var res = StaticXFData.DB.EquipOption.GetYZNameByEquipID(PENDANT?.id);
            return res;
        }

        public HashSet<string> GetSetOption()
        {
            var eqids = new List<string>(Dict.Count);
            eqids.AddRange(from _ in Dict.Values where _ != null where !_.id.IsEmptyOrWhiteSpace() select _.id);

            var effects = StaticXFData.DB.SetOption.GetSetEffects(eqids);
            return effects;
        }


        /// <summary>
        /// 解析装备特效
        /// </summary>
        public void Parse()
        {
            GetDict();
            ParseEquipOption();
            ParseBigFMSlotConfig();
        }

        public EquipOptionConfigSav ParseEquipOption()
        {
            var wp = GetWPOptionName();
            var yz = GetYZOptionName();
            var sets = GetSetOption();
            var sl = sets.Contains("SL");
            var jn = sets.Contains("JN");

            EquipOptionSav = new EquipOptionConfigSav(jn, sl, wp, yz);
            return EquipOptionSav;
        }

        /// <summary>
        /// 获取部位对应的大附魔ID
        /// </summary>
        /// <param name="subtype">部位字符串</param>
        /// <returns>大附魔ID，如果没有则为-1</returns>
        public int GetBigFMEnchantID(string subtype)
        {
            var snap = Dict[subtype];
            int res = -1;
            if (snap == null) return -1;

            res = snap.enchant ?? -1;
            return res;
        }


        public BigFMSlotConfig[] ParseBigFMSlotConfig()
        {
            var subtypes = new[]
            {
                nameof(HAT), nameof(JACKET), nameof(BELT), nameof(WRIST), nameof(SHOES)
            };

            var enchantids = (from _ in subtypes select GetBigFMEnchantID(_)).ToArray();

            var res = new BigFMSlotConfig[subtypes.Length];

            for (int i = 0; i < subtypes.Length; i++)
            {
                BigFMSlotConfig c;
                var cenchantid = enchantids[i];
                if (cenchantid == -1)
                {
                    c = new BigFMSlotConfig(false, -1);
                }
                else
                {
                    c = new BigFMSlotConfig(true, StaticXFData.DB.BigFM.GetItemID(cenchantid));
                }

                res[i] = c;
            }

            BigFMConfig = res;

            return BigFMConfig;
        }

        #endregion
    }
}