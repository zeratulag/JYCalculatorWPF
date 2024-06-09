using JX3CalculatorShared.Data;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Class;
using JYCalculator.Data;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JYCalculator.Src
{
    public class MultiZhenFaOptimizer
    {
        // 多重阵法优化器

        #region 成员

        public static readonly ImmutableArray<ZhenFa> AllZhen; // 阵法集合
        public static readonly int Num; // 阵法个数

        public readonly DPSKernelShell OriginalShell; // 原始hell
        public readonly ZhenFa OriginalZhenFa; // 原始阵法

        public readonly FullCharacter NoneZhenInputChar; // 不带阵法属性的InputChar;

        public readonly FullCharacter[] InputChars; // 在不同阵法下的InputChar

        public readonly DPSKernelShell[] DPSKernelShells; // 不同阵法下的Kernel

        public readonly Dictionary<string, MultiZhenRes> Result; // 结论
        public MultiZhenRes[] ResultArr;

        #endregion


        #region 构造

        static MultiZhenFaOptimizer()
        {
            var res = ImmutableArray.CreateBuilder<ZhenFa>();
            foreach (var _ in StaticXFData.DB.ZhenFa.ZhenFa)
            {
                if (!_.IsOwn)
                {
                    res.Add(_);
                }
            }

            AllZhen = res.ToImmutableArray();
            Num = AllZhen.Length;
        }


        public MultiZhenFaOptimizer(DPSKernelShell originalShell, ZhenFa originalZhenFa)
        {
            OriginalShell = originalShell;
            OriginalZhenFa = originalZhenFa;
            NoneZhenInputChar = OriginalShell.InputChar.Copy();
            NoneZhenInputChar.RemoveZhenFa(originalZhenFa);

            InputChars = new FullCharacter[Num];
            DPSKernelShells = new DPSKernelShell[Num];
            Result = new Dictionary<string, MultiZhenRes>(Num);
            ResultArr = new MultiZhenRes[Num];
        }

        #endregion

        public void Calc()
        {
            GetInputs();
            GetRelative();
            GetRank();
        }


        public void GetInputs()
        {
            for (int i = 0; i < AllZhen.Length; i++)
            {
                var _ = AllZhen[i];
                var newInput = NoneZhenInputChar.Copy();
                newInput.AddZhenFa(_);
                newInput.Name = _.Name;
                InputChars[i] = newInput;
                var newShell = OriginalShell.ChangeInputChar(newInput);
                newShell.CalcCurrent();
                newShell.Name = _.Name;
                DPSKernelShells[i] = newShell;

                var resi = new MultiZhenRes(_.IconID, newShell.CurrentDPSKernel.FinalDPS, _.ItemName);
                Result.Add(_.Name, resi);
            }
        }

        public void GetRelative()
        {
            // 计算阵法相对收益
            var baseline = Result["None"].DPS;

            foreach (var _ in Result.Values)
            {
                _.Relative = _.DPS / baseline;
            }
        }

        public void GetRank()
        {
            var list = Result.Values.OrderByDescending(_ => _.DPS);
            int i = 0;
            foreach (var _ in list)
            {
                ResultArr[i] = _;
                _.Rank = i + 1;
                i++;
            }
        }
    }
}