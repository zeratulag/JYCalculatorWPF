using Force.DeepCloner;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Globals;
using JYCalculator.Class;
using JYCalculator.Globals;
using JYCalculator.Models;
using JYCalculator.Utils;
using System.Linq;

namespace JYCalculator.Src
{
    /// <summary>
    /// 固定技能数和技能信息，仅仅根据面板计算DPS
    /// </summary>
    public partial class DPSKernelShell
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
        public Period<SkillFreqCTDF> LongSkillFreqCTDFs; // 长时间技能频率
        public Period<SkillFreqCTDF> ShortSkillFreqCTDFs; // 短时间技能频率

        public Period<SkillDataDF> SkillDfs;

        public BuffCoverDF BuffCover;

        public EventTriggerModel TriggerModel;

        public SkillFreqCTs[] SkillFreqCTsArr;

        public Period<FullCharacter> LongFChars; // 长时间战斗最终面板
        public Period<FullCharacter> ShortFChars; // 短时间战斗最终面板

        public DPSKernel LongDPSKernel;
        public DPSKernel ShortDPSKernel;

        public DPSKernel CurrentDPSKernel => FightTime.IsShort ? ShortDPSKernel : LongDPSKernel; // 根据界面选项，提供当前的Kernel

        private bool _HasProceed = false;

        public string Name { get; set; } // 名称

        #endregion

        #region 构造

        public DPSKernelShell(FullCharacter inputChar, Target target,
            Period<SkillDataDF> skillDfs, SkillNumModel skillNum,
            FightTimeSummary fightTime, DPSCalcShellArg arg)
        {
            InputChar = inputChar;
            CTarget = target;

            SkillDfs = skillDfs;
            SkillNum = skillNum;
            FightTime = fightTime;
            Arg = arg;
            PostProceed();
        }

        public void PostProceed()
        {
            BuffCover = new BuffCoverDF();
            var eventTriggerArg = new EventTriggerArg(Arg);

            TriggerModel = new EventTriggerModel(SkillNum, InputChar,
                SkillDfs, BuffCover, eventTriggerArg);
            BuffedFChars = TriggerModel.BuffedFChars;
            SkillFreqCTDFs = TriggerModel.SkillFreqCTDFs;
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
                SkillDfs = new Period<SkillDataDF>(SkillDfs.Normal.Copy(), SkillDfs.XinWu.Copy());
                SkillNum = old.SkillNum.DeepClone();
                FightTime = old.FightTime.DeepClone();
                Arg = old.Arg.DeepClone();
            }
            else
            {
                CTarget = old.CTarget;
                SkillDfs = old.SkillDfs;
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

        /// <summary>
        /// 仅仅改变InputChar的输入，其他条件不变，得到另一个Shell
        /// </summary>
        /// <param name="inputCharacter">新的输入属性</param>
        /// <returns></returns>
        public DPSKernelShell ChangeInputChar(FullCharacter inputCharacter)
        {
            var res = Copy();
            res.InputChar = inputCharacter;
            res.PostProceed();
            return res;
        }

        #endregion

        public void Proceed()
        {
            // 预处理
            CalcTiggerModel();
            CalcSkillFreqCTs();
            CalcTimeFChars();
            GetDPSKernel();
            _HasProceed = true;
        }


        public double CalcCurrent()
        {
            // 仅仅计算当前的，防止重复计算
            if (!_HasProceed)
            {
                Proceed();
            }

            var res = CurrentDPSKernel.Calc();
            return res;
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


        public void CalcTiggerModel()
        {
            TriggerModel.Calc();
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
                var LongXWFChar = GetXWChar(BuffedFChars.XinWu, FightTime.LongItem);
                LongXWFChar.Name = "长时间心无最终";
                var ShortXWFChar = GetXWChar(BuffedFChars.XinWu, FightTime.ShortItem);
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
            ShortSkillFreqCTDFs = SkillFreqCTDFs.Copy();
            LongSkillFreqCTDFs = SkillFreqCTDFs.Copy();
            FixTimeSkillFreqCTDFs();
            LongDPSKernel = new DPSKernel(LongFChars, CTarget, SkillDfs, LongSkillFreqCTDFs, FightTime.LongItem,
                kernelarg);
            ShortDPSKernel = new DPSKernel(ShortFChars, CTarget, SkillDfs, ShortSkillFreqCTDFs, FightTime.ShortItem,
                kernelarg);
        }

        // 修正因为有限时长导致的频率有误
        public void FixTimeSkillFreqCTDFs()
        {
            FixLveYingQiongCangFreq();
            FixNieJingZhuiMing();
            FixNingXingZhuiMing();
        }
        
        // 修正凝形起手的那个追命
        private void FixNingXingZhuiMing()
        {
            if (SkillNum.Normal.百步凝形_丝路风语)
            {
                const double num = 1.0;
                var longFreq = num / FightTime.LongItem.NormalTime;
                var shortFreq = num / FightTime.ShortItem.NormalTime;

                LongSkillFreqCTDFs.Normal.ModifySkillFreq(SkillKeyConst.追命箭, longFreq);
                ShortSkillFreqCTDFs.Normal.ModifySkillFreq(SkillKeyConst.追命箭, shortFreq);
                LongSkillFreqCTDFs.Normal.ModifySkillFreq(SkillKeyConst.追命箭_百步穿杨, longFreq);
                ShortSkillFreqCTDFs.Normal.ModifySkillFreq(SkillKeyConst.追命箭_百步穿杨, shortFreq);
            }
        }

        public void FixLveYingQiongCangFreq()
        {
            if (!SkillNum.QiXue.掠影穹苍) return;
            if (SkillNum.Normal.追夺流_雾海寻龙)
            {
                const double num = -4.0;
                var longDeFreq = num / FightTime.LongItem.NormalTime;
                var shortDeFreq = num / FightTime.ShortItem.NormalTime;

                LongSkillFreqCTDFs.Normal.ModifySkillFreq(SkillKeyConst.掠影穹苍, longDeFreq);
                ShortSkillFreqCTDFs.Normal.ModifySkillFreq(SkillKeyConst.掠影穹苍, shortDeFreq);
            }
        }

        public void FixNieJingZhuiMing()
        {
            // 修正凝形追命
            if (!SkillNum.QiXue.蹑景追风) return;
            if (SkillNum.Normal.追夺流_雾海寻龙)
            {
                const double num = 1;
                var LongDeFreq = num / FightTime.LongItem.XWTime;
                var ShortDeFreq = num / FightTime.ShortItem.XWTime;

                LongSkillFreqCTDFs.XinWu.TransSkillFreq(SkillKeyConst.追命箭_蹑景3_瞬发, SkillKeyConst.追命箭_瞬发, LongDeFreq);
                ShortSkillFreqCTDFs.XinWu.TransSkillFreq(SkillKeyConst.追命箭_蹑景3_瞬发, SkillKeyConst.追命箭_瞬发, ShortDeFreq);
            }
        }

        // 计算属性收益
        public void CalcProfit()
        {
            LongDPSKernel.CalcDerivs();
            ShortDPSKernel.CalcDerivs();
            if (AppStatic.XinFaTag == "TL")
            {
                FixProfit(); // 仅有天罗需要
            }

            GetFinal();
        }

        // 计算会心属性的收益
        public void FixProfit()
        {
            var dCTShell = Copy();
            var dCTPoint = 10; // 会心增量
            dCTShell.InputChar.ProcessPhysicsCriticalStrike(dCTPoint);
            //dCTShell.PostProceed();
            dCTShell.CalcAll();

            var LongdDPS = dCTShell.LongDPSKernel.FinalDPS - LongDPSKernel.FinalDPS;
            var ShortdDPS = dCTShell.ShortDPSKernel.FinalDPS - ShortDPSKernel.FinalDPS;

            var LongCT = LongdDPS / dCTPoint;
            var ShortCT = ShortdDPS / dCTPoint;

            LongDPSKernel.FixCTProfit(LongCT);
            ShortDPSKernel.FixCTProfit(ShortCT);
        }

        public void GetFinal()
        {
            LongDPSKernel.GetFinal();
            ShortDPSKernel.GetFinal();
        }
    }
}