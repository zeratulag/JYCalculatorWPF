using JYCalculator.Models.SkillSpecialEffectSolverModels;

namespace JYCalculator.Models
{
    /// <summary>
    /// 用于计算特殊触发BUFF的模型
    /// </summary>
    public partial class EventTriggerModel
    {
        public void Calc()
        {
            BuffCover.Reset();
            CalcSkillCTDF();
            CalcBigFMBelt();
            CalcSLBuff();
            CalcLongMenBuff();

            BuffedFChars.Normal.Has_Special_Buff = true;
            BuffedFChars.XinWu.Has_Special_Buff = true;

            CalcBigFM_SHOES();
            CalcEquipSpecialEffect();
            ApplyEquipSpecialEffectResults(); // [TODO]当前为临时解决方案，未考虑装备特效可能改变会心导致对触发的影响，下一步优化
#if DEBUG
            //CalcHuiChang(); // 计算回肠频率
#endif
        }

        private void CalcBigFM_SHOES()
        {
            if (Arg.BigFM_SHOES_120)
            {
                CalcBigFM_SHOES_120();
            }

            if (Arg.BigFM_SHOES_130)
            {
                CalcBigFM_SHOES_130();
            }
        }

        private void CalcEquipSpecialEffect()
        {
            EquipSpecialEffectSuperCustomDamageSolver =
                new EquipSpecialEffectSuperCustomDamageModel(Arg.EquipSpecialEffectConfig, SkillNum);
            EquipSpecialEffectSuperCustomDamageSolver.Calc();

            EquipSpecialEffectNormalBuffSolver = new EquipSpecialEffectNormalBuffModel(Arg.EquipSpecialEffectConfig, SkillNum);
            EquipSpecialEffectNormalBuffSolver.Calc();

            EquipSpecialEffectAdaptiveBuffSolver =
                new EquipSpecialEffectAdaptiveBuffModel(Arg.EquipSpecialEffectConfig, BuffedFChars,
                    BuffedFChars.Normal.Copy("SNAP"));
            EquipSpecialEffectAdaptiveBuffSolver.CalcSnap();
        }

        private void ApplyEquipSpecialEffectResults()
        {
            ApplyEquipSpecialEffectSuperCustomDamageResults();
            ApplyEquipSpecialEffectBuffResults();
            // EquipSpecialEffectAdaptiveBuffSolver 无须Apply，因为计算过程中实时加入了
            EquipSpecialEffectAdaptiveBuffSolver.ApplySnapBuffResults();
            EquipSpecialEffectAdaptiveBuffSolver.CalcAndApplyRealTimeBuffs();

            SkillFreqCTDFs.Normal.UpdateFChar();
            SkillFreqCTDFs.XinWu.UpdateFChar();
        }

        private void ApplyEquipSpecialEffectBuffResults()
        {
            //throw new System.NotImplementedException();
            AddPeriodBaseBuffs(EquipSpecialEffectNormalBuffSolver.FinalBuffs);
        }

        private void ApplyEquipSpecialEffectSuperCustomDamageResults()
        {
            foreach (var e in EquipSpecialEffectSuperCustomDamageSolver.Result)
            {
                AddSkillFreqByInterval(e.Skill.Name, e.NormalInterval, e.XWInterval);
            }
        }

    }
}