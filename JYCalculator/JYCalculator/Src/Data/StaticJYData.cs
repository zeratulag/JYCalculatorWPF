using JYCalculator.Globals;
using JYCalculator.Src.DB;


namespace JYCalculator.Src.Data
{
    public static class StaticJYData
    {
        public static readonly JYDataLoader Data;

        public static readonly JYDataBase DB;

        static StaticJYData()
        {
            Data = new JYDataLoader(JYAppStatic.DATA_PATH,
                JYAppStatic.OUTPUT_PATH,
                JYAppStatic.ZHENFA_PATH,
                autoLoad: true);
            DB = new JYDataBase();
        }
    }
}