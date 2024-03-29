﻿using JYCalculator.Src;
using JYCalculator.Src.Data;
using JYCalculator.ViewModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Src.Class;


namespace JYCalculator.Class
{

    public class JBPZPanel : JBPZPanelBase
    {
        #region 成员

        public new JBPZPlan EquipList { get; set; }

        [JsonIgnore] public InitInputSav InitInput; // 基于JB信息解析得到的结果

        #endregion

        public new static (bool success, JBPZPanel panel) TryImportFromJSON(string jsontxt)
        {
            JBPZPanel res = null;
            bool success = false;

            try
            {
                res = JsonConvert.DeserializeObject<JBPZPanel>(jsontxt);
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


        #region 构造

        public  void Parse()
        {
            EquipList.Parse();
            GetInitInput();
        }

        /// <summary>
        /// 解析输入面板，包括属性和特效
        /// </summary>
        /// <returns></returns>
        public InitInputSav GetInitInput()
        {
            var initchar = new InitCharacter(this);
            InitInput = new InitInputSav(initchar, EquipList.EquipOptionSav, EquipList.BigFMConfig);
            return InitInput;
        }

        #endregion

    }

    public class JBPZPlan : JBPZPlanBase
    {
        #region 成员

        [JsonIgnore] public EquipOptionConfigSav EquipOptionSav;

        [JsonIgnore] public BigFMSlotConfig[] BigFMConfig;

        #endregion

        #region 方法

        // 获取武器特效名称
        public string GetWPOptionName()
        {
            var res = StaticJYData.DB.EquipOption.GetWPNameByEquipID(PRIMARY_WEAPON?.id);
            return res;
        }

        // 获取腰坠特效名称
        public string GetYZOptionName()
        {
            var res = StaticJYData.DB.EquipOption.GetYZNameByEquipID(PENDANT?.id);
            return res;
        }
        
        public HashSet<string> GetSetOption()
        {
            var eqids = new List<string>(Dict.Count);
            eqids.AddRange(from _ in Dict.Values where _ != null where !_.id.IsEmptyOrWhiteSpace() select _.id);

            var effects = StaticJYData.DB.SetOption.GetSetEffects(eqids);
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
            var subtypes = new []
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
                    c = new BigFMSlotConfig(true, StaticJYData.DB.BigFM.GetItemID(cenchantid));
                }
                res[i] = c;
            }

            BigFMConfig = res;

            Had_BigFM_hat = res[0].IsChecked;
            Had_BigFM_jacket = res[1].IsChecked;

            return BigFMConfig;

        }

        #endregion
    }
}