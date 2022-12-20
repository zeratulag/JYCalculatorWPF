using System.Diagnostics;
using System.Windows.Controls;
using JX3CalculatorShared.Utils;
using JYCalculator.Src.Class;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Minimod.PrettyPrint;
using static JYCalculator.Globals.JYStaticData;


namespace JYCalculator.Src
{
    public class DPSKernelOptimizer
    {
        // 基于DPSKernel Shell层的优化求解器

        public readonly DPSKernelShell OriginalShell; // 原始hell
        public readonly InitCharacter OriginalIChar; // 原始不带大附魔的初始属性
        public readonly FullCharacter OriginalFChar; // 原始FullChar

        public readonly (double CT, double WS) FIncrement; // FullChar 相比InitChar增加的会心和无双属性

        public readonly (double CTOC, double WSPZ) OriginalPointSum; // IChar 初始会破和无招点数和

        public readonly FullCharInfo[] CTFCharSeq; // 改变会心得到的FChar序列
        public readonly FullCharInfo[] WSFCharSeq; // 改变无双得到的FChar序列

        public OptimizedICharResult BestProportion; // 最优会破/无招比例


        public static class OptimizerArgs
        {
            public static readonly VectorBuilder<double> VB = MathNet.Numerics.LinearAlgebra.Vector<double>.Build;
            public static readonly Vector<double> LowerBound = VB.DenseOfArray(new[] {0.0, 0.0});
            public static readonly Vector<double> UpperBound = VB.DenseOfArray(new[] {1.0, 1.0});
            public static readonly Vector<double> InitialGuess = VB.DenseOfArray(new[] {0.5, 0.5});
        }

        public DPSKernelOptimizer(DPSKernelShell originalShell)
        {
            OriginalShell = originalShell;
            OriginalIChar = OriginalShell.Arg.NoneBigFMInitCharacter;
            OriginalFChar = OriginalShell.InputChar;

            FIncrement = (OriginalFChar.CT - OriginalIChar.CT, OriginalFChar.WS - OriginalIChar.WS);
            OriginalPointSum = (OriginalIChar.CTOC_PointSum, OriginalIChar.WSPZ_PointSum);

            CTFCharSeq = new FullCharInfo[101];
            WSFCharSeq = new FullCharInfo[101];

            BestProportion = null;
        }

        public void Calc()
        {
            FindBestProportion();
        }

        /// 将IChar的会心和无双设置为新值，获取对应的面板数值
        /// <param name="ict"></param>
        /// <param name="iws"></param>
        /// <returns></returns>
        protected CharacterInfo _GetCharInfo(double ict, double iws)
        {
            var ioc = OriginalPointSum.CTOC - ict * fGP.CT;
            var ipz = OriginalPointSum.WSPZ - iws * fGP.WS;
            var res = new CharacterInfo(ict, iws, ioc, ipz, ict + FIncrement.CT, iws + FIncrement.WS);
            return res;

        }

        /// <summary>
        /// 基于IChar的会心和无双值占比，获取对应的面板
        /// </summary>
        /// <param name="ctProportion">会心占比（0~1），-1表示不变</param>
        /// <param name="wsProportion">无双占比（0~1），-1表示不变</param>
        public CharacterInfo GetCharInfo(double ctProportion = -1, double wsProportion = -1)
        {
            double iCT = OriginalIChar.CT;
            double iWS = OriginalIChar.WS;

            if (ctProportion >= 0 && ctProportion <= 1)
            {
                iCT = OriginalPointSum.CTOC * ctProportion / fGP.CT;
            }

            if (wsProportion >= 0 && wsProportion <= 1)
            {
                iWS = OriginalPointSum.WSPZ * wsProportion / fGP.WS;
            }

            return _GetCharInfo(iCT, iWS);
        }

        /// <summary>
        /// 基于CharInfo构建新的IChar和FChar
        /// </summary>
        /// <param name="characterInfo">CharInfo输入</param>
        /// <returns>IChar, FChar</returns>
        public (InitCharacter IChar, FullCharacter FChar) GetNewIFChar(CharacterInfo characterInfo)
        {
            var newIChar = OriginalIChar.Copy();
            newIChar.Reset_CT(characterInfo.ICT);
            newIChar.Reset_WS(characterInfo.IWS);

            var newFChar = OriginalFChar.Copy();
            newFChar.Reset_CT(characterInfo.FCT);
            newFChar.Reset_WS(characterInfo.FWS);
            return (newIChar, newFChar);
        }

        /// <summary>
        /// 基于CharInfo构建新的FChar
        /// </summary>
        /// <param name="characterInfo">CharInfo输入</param>
        /// <returns>IChar, FChar</returns>
        public FullCharacter GetNewFChar(CharacterInfo characterInfo)
        {
            var newFChar = OriginalFChar.Copy();
            newFChar.Reset_CT(characterInfo.FCT);
            newFChar.Reset_WS(characterInfo.FWS);
            return newFChar;
        }

        public FullCharInfo GetNewFCharInfo(double ctProportion = -1, double wsProportion = -1)
        {
            var charinfo = GetCharInfo(ctProportion, wsProportion);
            var fchar = GetNewFChar(charinfo);
            var res = new FullCharInfo(charinfo, fchar, ctProportion, wsProportion);
            return res;
        }

        /// <summary>
        /// 获取当会心比例从0~100时，得到的FChar数组
        /// </summary>
        /// <returns>包含FChar和CharacterInfo的数组</returns>
        public void GetNewFCharSeq()
        {
            for (int i = 0; i <= 100; i++)
            {
                var prop = i / 100.0;
                var ctFullInfo = GetNewFCharInfo(prop, -1);
                var wsFullInfo = GetNewFCharInfo(-1, prop);

                GetInfoDPS(ctFullInfo);
                GetInfoDPS(wsFullInfo);

                CTFCharSeq[i] = ctFullInfo;
                WSFCharSeq[i] = wsFullInfo;
            }
        }

        /// <summary>
        /// 修改会心比例和无双比例，计算当前DPS
        /// </summary>
        /// <param name="ctProportion">会心比例</param>
        /// <param name="wsProportion">无双比例</param>
        /// <returns>DPS值</returns>
        public double GetCurrentDPS(double ctProportion = -1, double wsProportion = -1)
        {
            var charInfo = GetNewFCharInfo(ctProportion, wsProportion);
            var newShell = OriginalShell.ChangeInputChar(charInfo.FChar);
            var DPS = newShell.CalcCurrent();
            return DPS;
        }

        /// <summary>
        /// 向量化版本的GetCurrentDPS，并且求相反数，因为优化目标是最小化
        /// </summary>
        /// <param name="x">(ctProp, wsProp)</param>
        /// <returns>DPS值</returns>
        public double GetNegativeCurrentDPSV(Vector<double> x) => -GetCurrentDPS(x[0], x[1]);


        public double GetInfoDPS(FullCharInfo info)
        {
            var shell = OriginalShell.ChangeInputChar(info.FChar);
            var dps = shell.CalcCurrent();
            info.DPS = dps;
            return dps;
        }


        /// <summary>
        /// 寻找最优的会破比例
        /// </summary>
        public OptimizedICharResult FindBestProportion()
        {
            var OPres = MinimizationTool.OfFunctionConstrained(GetNegativeCurrentDPSV, OptimizerArgs.LowerBound,
                OptimizerArgs.UpperBound,
                OptimizerArgs.InitialGuess);
            var pt = OPres.MinimizingPoint;
            var info = OPres.FunctionInfoAtMinimum;
            var charInfo = GetNewFCharInfo(pt[0], pt[1]);

            var res = new OptimizedICharResult(charInfo)
            {
                FinalDPS = - info.Value
            };
            res.DPSIncreasement = res.FinalDPS / OriginalShell.CurrentDPSKernel.FinalDPS - 1;

            res.Proceed();
            BestProportion = res;

            return res;

        }
    }


}