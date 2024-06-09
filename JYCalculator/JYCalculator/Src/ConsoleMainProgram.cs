using JX3CalculatorShared.Globals;
using JX3PZ.Data;
using JX3PZ.Src;
using JYCalculator.Data;
using JYCalculator.Globals;
using System.Linq;

namespace JYCalculator.Src
{
    public class ConsoleMainProgram
    {
        public static void ConsoleMain()
        {
            var args = new string[] { "0" };
            ConsoleMain(args);

        }

        public static void ConsoleMain(string[] args)
        {

#if DEBUG
            var jyData = StaticXFData.Data;
            var jyDB = StaticXFData.DB;


            var data = StaticXFData.DB.ZhenFa.Data.Skip(1).First().Value.Buff;
            var fGp = StaticConst.fGP;
            var xGp = XFStaticConst.fGP;


            //var d2 = new SqliteConnectionStringBuilder(dbpath);
            //var ds2 = d2.ToString();

            //var db = new SQLiteConnection(dburi);
            //var res = db.Query<AbilityItem>("select * from Ability");
            var tmp = StaticPzData.Enhance;
            var d1 = DiamondTabLib.Data;

            var xf = new JYXinFa();
            var guard = 0;
#endif
        }
    }

}
