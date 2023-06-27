using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3PZ.Class;
using JX3PZ.Data;
using JX3PZ.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Windows.Media.Animation;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.ViewModels;
using JX3PZ.ViewModels;

namespace JX3PZ.Models
{
    public class EquipSnapShotModel : IModel
    {
        // 用于表示一个装备槽上装备、附魔、强化、五行石、五彩石的类
        public Equip CEquip { get; set; }
        public int Strength { get; set; } // 装备栏精炼，注意可能大于装备精炼
        public int RealStrength { get; set; } // 真实生效的精炼等级
        public Enhance CEnhance { get; set; } // 附魔
        public BigFM CBigFM { get; set; } // 大附魔
        public DiamondLevelItem[] Diamonds { get; set; } // 五行石镶嵌
        public int[] DiamondLevels { get; set; } // 五行石镶嵌
        public Stone CStone { get; set; } // 五彩石
        public int Position { get; private set; }
        public EquipSlotEnum EquipSlot => (EquipSlotEnum) Position;

        #region 通过计算得到的属性

        public EquipLevelScore Score;
        public int DiamondCount;
        public int DiamondIntensity;
        public EquipStrengthDiamondModel EquipStrengthDiamond;
        public EquipSnapShotAttributeEntry AttributeEntry;

        #endregion

        public EquipSnapShotModel(int position = 0)
        {
            Position = position;
        }

        public EquipSnapShotModel(Equip cEquip, int strengthLevel = 0,
            DiamondLevelItem[] diamonds = null, Stone cStone = null,
            Enhance cEnhance = null, BigFM cBigFM = null) : base()
        {
            Update(cEquip, strengthLevel, diamonds, cStone, cEnhance, cBigFM);
        }

        public void Update(Equip cEquip, int strengthLevel,
            DiamondLevelItem[] diamonds, Stone cStone,
            Enhance cEnhance = null, BigFM cBigFM = null)
        {
            CEquip = cEquip;
            Strength = strengthLevel;

            if (diamonds != null)
            {
                Diamonds = diamonds;
            }
            else
            {
                if (cEquip.HasDiamond)
                {
                    Diamonds = cEquip.Attributes.Diamond.Select(_ => _.LevelItems[0]).ToArray(); // 默认自带的孔
                }
                else
                {
                    Diamonds = null;
                }
            }

            EquipStrengthDiamond = new EquipStrengthDiamondModel(CEquip, strengthLevel, Diamonds);
            RealStrength = EquipStrengthDiamond.StrengthModel.RealStrength;

            DiamondLevels = EquipStrengthDiamond.DiamondLevels;

            CStone = cStone;
            CEnhance = cEnhance;
            CBigFM = cBigFM;

            Calc();
        }

        public void UpdateFrom(PzTabItemViewModel vm)
        {
            Update(vm.EquipSelectVM.SelectedItem, vm.EquipEnhanceVM.StrengthLevel,
                vm.EquipEmbedVM.DiamondVM.Items, vm.EquipEmbedVM.SelectedStone,
                vm.EquipEnhanceVM.SelectedEnhance, vm.EquipEnhanceVM.SelectedBigFM);
            Position = vm.Position;
        }

        public EquipLevelScore GetLevelScore()
        {
            int level = 0;
            int score = 0;
            if (CEquip != null)
            {
                level = CEquip.Level;
                score = CEquip.Score;
            }

            int strengthLevel = EquipStrengthDiamond.StrengthModel.StrengthLevel;
            int strengthScore = EquipStrengthDiamond.StrengthModel.StrengthScore;

            DiamondCount = EquipStrengthDiamond.DiamondCount;
            DiamondIntensity = EquipStrengthDiamond.DiamondIntensity;
            decimal ds = EquipStrengthDiamond.DiamondScore;

            if (CStone != null)
            {
                ds += CStone.GetScore();
            }

            if (CEnhance != null)
            {
                ds += CEnhance.Score;
            }

            if (CBigFM != null)
            {
                ds += CBigFM.Score;
            }

            int enhanceScore = ds.MathRound();

            var res = new EquipLevelScore(level, strengthLevel, score, strengthScore,
                enhanceScore);
            Score = res;
            return res;
        }

        public EquipShowViewModel GetShow()
        {
            var equipShow = new EquipShowViewModel { };
            equipShow.UpdateFrom(this);
            equipShow.DisableExtra();
            return equipShow;
        }

        public void Calc()
        {
            EquipStrengthDiamond.Calc();
            GetLevelScore();
            GetAttributeEntry();
        }

        public void GetAttributeEntry()
        {
            AttributeEntry = new EquipSnapShotAttributeEntry(CEquip?.Attributes.BaseEntry,
                EquipStrengthDiamond?.EquipAttributeEntry,
                EquipStrengthDiamond?.DiamondAttributeEntry, CEnhance?.Entry, CBigFM?.AttributeEntries);
            AttributeEntry.Calc();
        }

        public string GetEquipOptionName()
        {
            return CEquip?.EquipOptionName;
        }

        public BigFMSlotConfig GetBigFMSlotConfig()
        {
            bool isChecked = false;
            int uiid = -1;
            if (CBigFM != null)
            {
                isChecked = true;
                uiid = CBigFM.UIID;
            }

            var res = new BigFMSlotConfig(isChecked, uiid);
            return res;
        }

        public JBPZEquipSnapshot ExportJBPZEquipSnapshot()
        {
            var res = new JBPZEquipSnapshot();
            if (CEquip != null)
            {
                res.id = CEquip.EID;
            }

            if (CStone != null)
            {
                res.stone = CStone.ID;
            }

            if (CBigFM != null)
            {
                res.enchant = CBigFM.ID;
            }

            if (CEnhance != null)
            {
                res.enhance = CEnhance.ID;
            }

            res.strength = Strength;
            res.embedding = DiamondLevels;
            return res;
        }

        public HashSet<PzType> FindHasteAttrs()
        {
            // 查找加速部位
            var res = new HashSet<PzType>(4);
            if (CEquip != null && CEquip.IsHaste)
            {
                res.Add(PzType.Equip);
            }

            if (CEnhance != null && CEnhance.IsHaste)
            {
                res.Add(PzType.Enhance);
            }

            if (CStone != null && CStone.IsHaste)
            {
                res.Add(PzType.Stone);
            }
            return res;
        }
    }


    public enum PzType
    {
        Equip, // 装备
        Enhance, // 小附魔
        BigFM, // 大附魔
        Diamond, // 镶嵌
        Stone, // 五彩石
        Set, // 套装
    }
}