using JX3CalculatorShared.Data;
using JYCalculator.Globals;
using System.Collections.Immutable;
using JX3CalculatorShared.Utils;
using static JX3CalculatorShared.Utils.ImportTool;

// ReSharper disable InconsistentNaming CheckNamespace

namespace JYCalculator.Data
{
    public class XFDataLoader : DataLoader
    {
        // Can Shared

        #region 成员

        public ImmutableArray<AbilitySkillNumItem> AbilitySkillNum;
        public ImmutableDictionary<string, SkillInfoItem> SkillData;
        public ImmutableDictionary<int, DiamondValueItem> DiamondValue;
        public CalcSetting Setting; // _Settings.json

        #endregion

        #region 构造

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datafile">文件路径</param>
        /// <param name="outfile">Out文件路径</param>
        /// <param name="zhenfafile">阵法json文件路径</param>
        /// <param name="settingfile"></param>
        /// <param name="autoLoad">是否自动加载</param>
        public XFDataLoader(string datafile = XFAppStatic.DATA_PATH,
            string outfile = XFAppStatic.OUTPUT_PATH,
            string zhenfafile = XFAppStatic.ZHENFA_PATH,
            string settingfile = XFAppStatic.SETTING_PATH,
            bool autoLoad = false)
        {
            DataFile = datafile;
            OutFile = outfile;
            ZhenFaFile = zhenfafile;
            SettingFile = settingfile;
            if (autoLoad)
            {
                Load();
            }
        }

        public void Load()
        {
            LoadTarget();
            LoadRecipe();

            LoadZhenFa();
            LoadItemDT();
            LoadBuff_df();

            LoadQiXue();
            LoadBigFM();
            LoadEquipOption();
            LoadAbility();
            LoadAbilitySkillNum();
            LoadSkillData();
            LoadSkillEvent();

            LoadUselessAttrs();
            //LoadRecipeSkillMap();
            LoadSkillModifier();
            LoadDiamondValue();
            LoadSetOption();
            LoadSetting();
        }

        #endregion

        #region 加载

        private void LoadAbilitySkillNum()
        {
            var rows = ReadSheetAsArray<AbilitySkillNumItem>(DataFile, "SkillNum");
            AbilitySkillNum = rows;
        }

        private void LoadSkillData()
        {
            SkillData = ReadSheetAsDict<string, SkillInfoItem>(DataFile, "Skill_Data", "Name");
        }

        private void LoadDiamondValue()
        {
            DiamondValue = ReadSheetAsDict<int, DiamondValueItem>(DataFile, "Diamond", keyName: "Level");
        }

        protected void LoadSetting()
        {
            Setting = ImportTool.ReadJSON<CalcSetting>(SettingFile);
            Setting.Parse();
        }

        #endregion


    }
}