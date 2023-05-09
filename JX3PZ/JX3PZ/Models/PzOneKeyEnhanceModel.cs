using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Utils;
using JX3PZ.Globals;
using JX3PZ.ViewModels;

namespace JX3PZ.Models
{
    public class PzOneKeyEnhanceModel : IModel
    {
        public PzMainWindowViewModels MainVM;
        public EquipEnhanceViewModel[] EnhanceVMs;

        public int[] CurrentIdx;
        public BestEnhanceResult[] BestRes;
        public int[] HasteIdx;
        public int[] FinalIdx;

        public HashSet<PzType> HasteSource;

        public PzOneKeyEnhanceModel(PzMainWindowViewModels mainVM)
        {
            MainVM = mainVM;
            EnhanceVMs = MainVM.Data.Select(_ => _.EquipEnhanceVM).ToArray();
        }

        public void GetCondition()
        {
            CurrentIdx = EnhanceVMs.Select(_ => _.SelectedEnhanceIndex).ToArray(); // 当前附魔
            BestRes = EnhanceVMs.Select(_ => _.GetBestEnhanceByCalcResult(MainVM.CCalcResult)).ToArray(); // 最优附魔
            HasteIdx = EnhanceVMs.Select(_ => _.HasteEnhanceIdx).ToArray();
        }

        public void FindHasteSource()
        {
            // 寻找加速来源
            var res = new HashSet<PzType>(4);
            foreach (var _ in MainVM.Model.SnapShots)
            {
                res.AddRange(_.FindHasteAttrs());
            }

            HasteSource = res;
        }

        public void Calc()
        {
            GetCondition();
            FindHasteSource();
            AddHaste();
        }

        private void AddHaste()
        {
            FinalIdx = BestRes.Select(_ => _.Index).ToArray();

            if (HasteSource.Contains(PzType.Equip) || HasteSource.Contains(PzType.Stone))
            {
                // 装备/五彩石自带加速，无须调整
                return;
            }

            var loss = new BestEnhanceResult[FinalIdx.Length];
            // 若没有装备加速且没有加速五彩石
            for (int i = 0; i < HasteIdx.Length; i++)
            {
                if (HasteIdx[i] >= 0)
                {
                    loss[i] = BestRes[i];
                }
            }

            var res = loss.Where(_ => _ != null).OrderBy(_ => _.Value).ToArray().First();
            var hastSlot = res.Slot; // 需要改成加速的附魔部位
            int hat = (int) EquipSlotEnum.HAT;
            int shoe = (int) EquipSlotEnum.SHOES;
            if (hastSlot == EquipSlotEnum.HAT && loss[shoe] != null)
            {
                if (Math.Abs(loss[hat].Value - loss[shoe].Value) < 0.01)
                    // 头和鞋特殊处理
                    hastSlot = EquipSlotEnum.SHOES;
            }

            FinalIdx[(int) hastSlot] = HasteIdx[(int) hastSlot];
        }
    }
}