using JYCalculator.Src.Class;
using static System.Net.WebRequestMethods;

namespace JYCalculator.Src
{
    public readonly struct CharacterInfo
    {
        // 描述新的面板关键属性信息的类

        public readonly double ICT; // 原始会心值
        public readonly double IWS; // 原始无双值
        public readonly double FCT; // FChar会心值
        public readonly double FWS; // FChar无双值

        public readonly double IOC; // 原始破防点数
        public readonly double IPZ; // 原始破招点数

        public CharacterInfo(double ict, double iws, double ioc, double ipz, double fct, double fws)
        {
            ICT = ict;
            IWS = iws;
            IOC = ioc;
            IPZ = ipz;
            FCT = fct;
            FWS = fws;
        }
    }

    public class FullCharInfo
    {
        public readonly CharacterInfo Info;
        public readonly FullCharacter FChar;
        public readonly double CTProportion;
        public readonly double WSProportion;
        public double DPS;

        public FullCharInfo(CharacterInfo info, FullCharacter fChar, double ctProportion, double wsProportion)
        {
            Info = info;
            FChar = fChar;
            CTProportion = ctProportion;
            WSProportion = wsProportion;
        }
    }
}