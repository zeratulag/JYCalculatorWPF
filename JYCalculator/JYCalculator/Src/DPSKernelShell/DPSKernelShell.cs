using Force.DeepCloner;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Src.Class;
using JYCalculator.Class;
using JYCalculator.Models;
using JYCalculator.Models.BuffExtraTriggerModel;
using JYCalculator.Src.Class;
using JYCalculator.Utils;
using System.Linq;

namespace JYCalculator.Src
{
    /// <summary>
    /// 固定技能数和技能信息，仅仅根据面板计算DPS
    /// </summary>
    public class DPSKernelShell
    {
        #region 成员

        // 输入参数
        public FullCharacter InputChar; // 除了特殊增益（神力，弩心，催寒）之外，计算了所有增益的属性面板
        public readonly Target CTarget; // 当前目标

        public readonly SkillNumModel SkillNum; // 技能频率数据
        public readonly FightTimeSummary FightTime; // 战斗时间总结
        public readonly DPSCalcShellArg Arg; // 其他参数

        // 中间过程

        public Period<FullCharacter> BuffedFChars; // 存储计算了特殊BUFF，但是未考虑腰坠后的面板（腰坠的属性需要结合时间计算）
        public Period<SkillFreqCTDF> SkillFreqCTDFs; // 技能频率
        public Period<SkillDataDF> SkillDataDfs;

        public BuffCoverDF BuffCover;

        public BuffExtraTriggerModel BuffTriggerModel;

        public SkillFreqCTs[] SkillFreqCTsArr;

        public Period<FullCharacter> LongFChars; // 长时间战斗最终面板
        public Period<FullCharacter> ShortFChars; // 短时间战斗最终面板

        public DPSKernel LongDPSKernel;
        public DPSKernel ShortDPSKernel;

        public DPSKernel CurrentDPSKernel; // 根据界面选项，提供当前的Kernel

        private bool _HasProceed = false;

        #endregion

        #region 构造

        public DPSKernelShell(FullCharacter inputChar, Target target,
            SkillDataDF skillDataDf, SkillNumModel skillNum,
            FightTimeSummary fightTime, DPSCalcShellArg arg)
        {
            InputChar = inputChar;
            CTarget = target;

            SkillDataDfs = new Period<SkillDataDF>(skillDataDf, skillDataDf.Copy());
            SkillNum = skillNum;
            FightTime = fightTime;
            Arg = arg;
            PostProceed();
        }

        public void PostProceed()
        {
            var sy = Arg.BigFM.Belt != null; // 是否有伤腰大附魔
            var bigfm_shoes_120 = Arg.BigFM.Shoes?.DLCLevel == 120; // 是否有120级伤鞋子大附魔
            var buffExtraTriggerArg = new BuffExtraTriggerArg(Arg.SL, sy, bigfm_shoes_120, Arg.BigXW);

            BuffTriggerModel = new BuffExtraTriggerModel(SkillNum, InputChar,
                SkillDataDfs, buffExtraTriggerArg);
            BuffedFChars = BuffTriggerModel.BuffedFChars;
            BuffCover = BuffTriggerModel.BuffCover;
            SkillFreqCTDFs = BuffTriggerModel.SkillFreqCTDFs;
        }

        /// <summary>
        /// 复制构造
        /// </summary>
        /// <param name="old">已有的对象</param>
        /// <param name="deep">是否深拷贝（浅拷贝仅仅修改面板，其余对象均直接引用；深拷贝复制所有对象）</param>

        public DPSKernelShell(DPSKernelShell old, bool deep = false)
        {
            InputChar = old.InputChar.Copy();

            if (deep)
            {
                CTarget = old.CTarget.Copy();
                SkillDataDfs = new Period<SkillDataDF>(SkillDataDfs.Normal.Copy(), SkillDataDfs.XW.Copy());
                SkillNum = old.SkillNum.DeepClone();
                FightTime = old.FightTime.DeepClone();
                Arg = old.Arg.DeepClone();
            }
            else
            {
                CTarget = old.CTarget;
                SkillDataDfs = old.SkillDataDfs;
                SkillNum = old.SkillNum;
                FightTime = old.FightTime;
                Arg = old.Arg;
            }
            PostProceed();
        }

        public DPSKernelShell Copy(bool deep = false)
        {
            var res = new DPSKernelShell(this, deep);
            return res;
        }

        #endregion

        public void Proceed()
        {
            // 预处理
            CalcBuffExtraTigger();
            CalcSkillFreqCTs();
            CalcTimeFChars();
            GetDPSKernel();
            GetCurrentKernel();
            _HasProceed = true;
        }


        public void CalcCurrent()
        {
            // 仅仅计算当前的，防止重复计算
            if (!_HasProceed)
            {
                Proceed();
            }
            CurrentDPSKernel.Calc();
        }

        public void CalcAll()
        {
            if (!_HasProceed)
            {
                Proceed();
            }
            LongDPSKernel.Calc();
            ShortDPSKernel.Calc();
        }


        public void CalcBuffExtraTigger()
        {
            BuffTriggerModel.Calc();
        }

        public void CalcSkillFreqCTs()
        {
            SkillFreqCTsArr = SkillFreqCTDFs.GetSkillFreqCTTable();
        }

        // 当有腰坠存在时，长时间和短时间战斗下，输入的面板不一样
        public void CalcTimeFChars()
        {
            if (Arg.YZ.IsNormal)
            {
                LongFChars = BuffedFChars;
                ShortFChars = BuffedFChars;
            }
            else
            {
                var LongXWFChar = GetXWChar(BuffedFChars.XW, FightTime.LongItem);
                LongXWFChar.Name = "长时间心无最终";
                var ShortXWFChar = GetXWChar(BuffedFChars.XW, FightTime.ShortItem);
                ShortXWFChar.Name = "短时间心无最终";

                LongFChars = new Period<FullCharacter>(BuffedFChars.Normal, LongXWFChar);
                ShortFChars = new Period<FullCharacter>(BuffedFChars.Normal, ShortXWFChar);
            }
        }

        /// <summary>
        /// 获得考虑了腰坠后的心无面板
        /// </summary>
        /// <param name="FChar">心无属性</param>
        /// <param name="Cover">心无期间腰坠覆盖率</param>
        /// <returns>输入的面板</returns>
        public FullCharacter GetXWChar(FullCharacter FChar, double Cover)
        {
            var YZAttrs = Arg.YZ.SCharAttr;
            var RealAttr = YZAttrs.Values.ToDictionary(_ => _.Key, _ => _.Value * Cover);
            var res = FChar.Copy();
            res.AddSAttrDict(RealAttr);
            return res;
        }

        public FullCharacter GetXWChar(FullCharacter FChar, FightTimeSummaryItem item)
        {
            return GetXWChar(FChar, item.YZOverXWCover);
        }

        public void GetDPSKernel()
        {
            var kernelarg = new DPSKernelArg(SkillNum);
            LongDPSKernel = new DPSKernel(LongFChars, CTarget, SkillDataDfs, SkillFreqCTDFs, FightTime.LongItem, kernelarg);
            ShortDPSKernel = new DPSKernel(ShortFChars, CTarget, SkillDataDfs, SkillFreqCTDFs, FightTime.ShortItem, kernelarg);
        }

        // 明确当前是长时间还是短时间
        public void GetCurrentKernel()
        {
            CurrentDPSKernel = LongDPSKernel;
            if (FightTime.IsShort)
            {
                CurrentDPSKernel = ShortDPSKernel;
            }
        }

        // 计算属性收益
        public void CalcProfit()
        {
            LongDPSKernel.CalcDerivs();
            ShortDPSKernel.CalcDerivs();
            FixProfit();
        }

        // 计算会心属性的收益
        public void FixProfit()
        {
            var dCTShell = Copy();
            var dCTPoint = 10; // 会心增量
            dCTShell.InputChar.Add_CT_Point(dCTPoint);
            //dCTShell.PostProceed();
            dCTShell.CalcAll();

            var LongdDPS = dCTShell.LongDPSKernel.FinalDPS - LongDPSKernel.FinalDPS;
            var ShortdDPS = dCTShell.ShortDPSKernel.FinalDPS - ShortDPSKernel.FinalDPS;

            var LongCT = LongdDPS / dCTPoint;
            var ShortCT = ShortdDPS / dCTPoint;

            LongDPSKernel.FixCTProfit(LongCT);
            ShortDPSKernel.FixCTProfit(ShortCT);

        }

    }

    public readonly struct DPSCalcShellArg
    {
        public readonly bool SL; // 是否有神力
        public readonly bool BigXW; // 是否为大心无
        public readonly YZOption YZ; // 腰坠选项

        public readonly BigFMConfigModel BigFM;

        public DPSCalcShellArg(bool sl, bool bigXw, YZOption yz, BigFMConfigModel bigFm)
        {
            SL = sl;
            BigXW = bigXw;
            YZ = yz;
            BigFM = bigFm;
        }

    }
}