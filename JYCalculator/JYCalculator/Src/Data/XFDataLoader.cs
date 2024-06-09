using JX3CalculatorShared.Data;
using JX3CalculatorShared.Utils;
using JYCalculator.Globals;
using System.Collections.Immutable;
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
        public ImmutableArray<SkillBuildItem> SkillBuild;

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
            LoadSkillInfo();
            LoadSkillEvent();

            LoadUselessAttrs();
            //LoadRecipeSkillMap();
            LoadSkillModifier();
            LoadDiamondValue();
            LoadSetOption();
            LoadSetting();
            LoadSkillBuild();
        }

        #endregion

        #region 加载

        private void LoadAbilitySkillNum()
        {
            var rows = ReadSheetAsArray<AbilitySkillNumItem>(DataFile, "SkillNum");
            AbilitySkillNum = rows;
            AbilitySkillNum.ForEach(_ => _.Parse());
        }

        private void LoadSkillInfo()
        {
            SkillData = ReadSheetAsDict<string, SkillInfoItem>(DataFile, "SkillInfo", GetName);
            SkillData.Values.ForEach(_ => _.Parse());
        }

        private void LoadDiamondValue()
        {
            DiamondValue = ReadSheetAsDict<int, DiamondValueItem>(DataFile, "Diamond", _ => _.Level);
        }

        protected void LoadSkillBuild()
        {
            SkillBuild = ReadSheetAsArray<SkillBuildItem>(DataFile, "SkillBuild");
        }

        protected void LoadSetting()
        {
            Setting = ImportTool.ReadJSON<CalcSetting>(SettingFile);
            Setting.Parse();
        }

        #endregion


    }
}