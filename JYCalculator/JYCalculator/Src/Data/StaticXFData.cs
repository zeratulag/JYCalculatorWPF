using JX3CalculatorShared.Class;
using JYCalculator.DB;
using JYCalculator.Globals;


namespace JYCalculator.Data
{
    // 心法数据库 Can Shared
    public static class StaticXFData
    {
        public static readonly XFDataLoader Data;

        public static readonly XFDataBase DB;

        static StaticXFData()
        {
            Data = new XFDataLoader(XFAppStatic.DATA_PATH,
                XFAppStatic.OUTPUT_PATH,
                XFAppStatic.ZHENFA_PATH,
                autoLoad: true);
            DB = new XFDataBase();
        }

        public static Recipe GetRecipe(string name) => DB.Recipe[name];
        public static Buff GetExtraTriggerBuff(string name) => DB.Buff.Buff_ExtraTrigger[name];
    }
}