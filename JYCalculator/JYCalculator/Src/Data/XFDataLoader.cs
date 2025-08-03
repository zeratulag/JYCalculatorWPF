using System;
using System.Collections.Generic;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Utils;
using JYCalculator.Globals;
using System.Collections.Immutable;
using System.Linq;
using JX3CalculatorShared.Common;
using JYCalculator.Class;
using JYCalculator.Src.Class;
using static JX3CalculatorShared.Utils.ImportTool;
using MethodTimer;
using MoreLinq;
using Syncfusion.Data.Extensions;

// ReSharper disable InconsistentNaming CheckNamespace

namespace JYCalculator.Data
{
    public class XFDataLoader : DataLoader
    {
        // Can Shared

        #region 成员

        public ImmutableArray<AbilitySkillNumItem> AbilitySkillNum { get; protected set; }
        public ImmutableDictionary<string, SkillInfoItem> BaseSkillInfoData { get; protected set; }
        public ImmutableDictionary<int, DiamondValueItem> DiamondValue { get; protected set; }
        public ImmutableArray<SkillBuildItem> SkillBuild { get; protected set; }
        public ImmutableDictionary<string, BuffToRecipeItem> BuffToRecipe { get; protected set; }
        public ImmutableArray<SkillRatioItem> SkillRatios { get; protected set; }
        public List<SkillInfoItem> DerivedSkillInfoItem { get; private set; }
        public ImmutableArray<SkillInfoItem> AllSkillInfoItem { get; private set; } // 补上派生的技能信息之后的全量技能信息表
        public ImmutableDictionary<string, SkillInfoItem> AllSkillInfoData { get; private set; }
        public CalcSetting Setting; // _Settings.json
        public Period<BaseSkillRatioGroup> DefaultBaseSkillRatioGroups { get; private set; } // 默认紫武的
        public Period<BaseSkillRatioGroup> ChengWuBaseSkillRatioGroups { get; private set; } // 橙武的

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

        [Time]
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
            LoadEquipSpecialEffectEntries();
            LoadEquipSpecialEffectTable();

            LoadUselessAttrs();
            //LoadRecipeSkillMap();
            LoadSkillModifier();
            LoadDiamondValue();
            LoadSetOption();
            LoadSetting();
            LoadSkillBuild();
            LoadBuffToRecipe();
            LoadSkillRatio();
            PostProcess();
        }

        private void LoadBuffToRecipe()
        {
            var rows = ReadSheetAsArray<BuffToRecipeItem>(DataFile, "BuffToRecipe");
            BuffToRecipe = rows.ToImmutableDictionary(_ => _.BuffRawID, _ => _);
        }


        // 后处理
        public void PostProcess()
        {
            ProcessSkillModifier();
        }


        private void ProcessSkillModifier()
        {
            foreach (var modifier in SkillModifier.Values)
            {
                modifier.FindSkillNames(BaseSkillInfoData.Values);
            }
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
            BaseSkillInfoData = ReadSheetAsDict<string, SkillInfoItem>(DataFile, "SkillInfo", GetName);
            BaseSkillInfoData.Values.ForEach(_ => _.Parse());
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

        protected void LoadSkillRatio()
        {
            SkillRatios = ReadSheetAsArray<SkillRatioItem>(DataFile, "NingXingSkillRatio");
            foreach (var item in SkillRatios)
            {
                item.Parse();
            }
        }

        #endregion

        #region 当DB加载完毕才能执行的动作

        public void AfterDBLoad()
        {
            AfterParseSkillRatios();
            CollectDerivedSkillInfoItem();
            CollectAllSkillInfoItems();
            UpdateSkillModifierInfo();
            MakeDefaultBaseSkillRatioGroups();
        }

        public void UpdateSkillModifierInfo()
        {
            // 更新SkillModifier中可以被应用的技能名称;
            foreach (var modifier in SkillModifier.Values)
            {
                modifier.FindSkillNames(DerivedSkillInfoItem);
            }
        }

        private void MakeDefaultBaseSkillRatioGroups()
        {
            var skillRatioGroups = BaseSkillRatioGroup.MakeMultiGenrePeriodBaseSkillRatioGroup(SkillRatios);
            DefaultBaseSkillRatioGroups = skillRatioGroups[0];
            ChengWuBaseSkillRatioGroups = skillRatioGroups[1];
        }

        public void AfterParseSkillRatios()
        {
            SkillRatios.ForEach(item => item.AfterParse());
        }

        public void CollectDerivedSkillInfoItem()
        {
            DerivedSkillInfoItem = SkillInfoItemFactory.Dict.Values.ToList();
        }

        public void CollectAllSkillInfoItems()
        {
            AllSkillInfoItem = BaseSkillInfoData.Values
                .Concat(DerivedSkillInfoItem)
                .DistinctBy(item => item.Name)
                .ToImmutableArray();

            //AllSkillInfoItem = allSkillInfoItems.Distinct(item => item.Name).ToImmutableArray();
            AllSkillInfoData = AllSkillInfoItem.ToImmutableDictionary(item => item.Name, item => item);
        }

        // 收集所有派生的技能

        #endregion
    }
}