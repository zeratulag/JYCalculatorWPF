using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3PZ.Src;
using JX3PZ.ViewModels;
using JYCalculator.DB;
using JYCalculator.Globals;
using Syncfusion.Data.Extensions;
using System.Linq;
using System.Security.Policy;


namespace JYCalculator.Data
{
    // 心法数据库 Can Shared
    public static class StaticXFData
    {
        public static XFDataLoader Data { get; private set; }

        public static XFDataBase DB { get; private set; }

        static StaticXFData()
        {
            Data = new XFDataLoader(XFAppStatic.DATA_PATH,
                XFAppStatic.OUTPUT_PATH,
                XFAppStatic.ZHENFA_PATH);

        }

        public static void Load()
        {
            Data.Load();
            DB = new XFDataBase();
            StaticPzData.Enhance.LoadBigFM(DB.BigFM);
            AfterLoad();
        }

        public static void AfterLoad()
        {
            AttachBuffToRecipeItem();
            AfterParseSkillBuff();
        }

        private static void AfterParseSkillBuff()
        {
            SkillBuffFactory.AfterParse();
            Data.AfterDBLoad();
            DB.BuildAllSkillInfo();
        }

        public static Recipe GetRecipe(string name) => DB.Recipe[name];
        public static KBuff GetExtraTriggerBuff(string name) => DB.Buff.Buff_ExtraTrigger[name];

        public static void MakeStoneAttrFilter()
        {
            // 制作五彩石属性筛选列
            var res = XFStaticConst.UsefulStoneAttrs.ToList();
            res.AddRange(StaticConst.UsefulStoneAttrs);
            EquipStoneSelectViewModel.SetFilterItems(res);
        }

        public static void AttachBuffToRecipeItem()
        {
            foreach (var item in Data.BuffToRecipe.Values)
            {
                DB.AttachBuffToRecipeItem(item);
            }
        }
    }
}