using JX3CalculatorShared.Globals;
using JYCalculator.Data;
using JYCalculator.Globals;
using System;
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


            var guard = 0;

#endif
        }
    }
}
