using System.Linq;
using JYCalculator.Src.Data;

namespace JYCalculator.Src
{
    public class ConsoleMainProgram
    {
        public static void ConsoleMain()
        {
            var args = new string[] {"0"};
            ConsoleMain(args);

        }
        
        public static void ConsoleMain(string[] args)
        {

#if DEBUG
            var jyData = StaticJYData.Data;
            var jyDB = StaticJYData.DB;
            

            var data = StaticJYData.DB.ZhenFa.Data.Skip(1).First().Value.Buff;
            var guard = 0;

#endif
        }
    }
}
