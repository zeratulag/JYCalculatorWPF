using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3PZ.Class;
using JX3PZ.Data;
using JX3PZ.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JX3PZ.ViewModels
{
    public class EquipShowViewModel : ObservableObject, IEquipShow
    {
        public bool HasEquip { get; private set; }
        public bool NotHasEquip => !HasEquip;

        public Equip CEquip { get; set; }
        public EquipShowHeadViewModel Head { get; set; }
        public EquipShowMagicViewModel Magic { get; set; }
        public EquipShowStoneViewModel Stone { get; set; }
        public EquipShowEnhanceViewModel Enhance { get; set; }
        public EquipShowTailViewModel Tail { get; set; }
        public EquipShowExtraViewModel Extra { get; set; }
        public EquipShowSetViewModel Set { get; set; } = EquipShowSetViewModel.Empty;

        public EquipShowBoxViewModel EquipShowBoxVM { get; set; }

        public EquipShowViewModel()
        {
            Stone = new EquipShowStoneViewModel();
            Enhance = new EquipShowEnhanceViewModel(false);
            Tail = new EquipShowTailViewModel();
            Extra = new EquipShowExtraViewModel();

            EquipShowBoxVM = new EquipShowBoxViewModel(this);
        }

        public EquipShowViewModel(EquipShowViewModel old)
        {
            HasEquip = old.HasEquip;
            CEquip = old.CEquip;
            Head = old.Head;
            Magic = old.Magic;
            Stone = old.Stone;
            Enhance = old.Enhance;
            Tail = old.Tail;
            Extra = old.Extra;
            EquipShowBoxVM = old.EquipShowBoxVM;
        }


        /// <summary>
        /// 注意这里传入的是真实精炼等级
        /// </summary>
        /// <param name="equip"></param>
        /// <param name="strengthLevel"></param>
        /// <param name="diamond"></param>
        /// <param name="cStone"></param>
        public void UpdateFrom(Equip equip, int strengthLevel = 0,
            DiamondLevelItem[] diamond = null,
            Stone cStone = null,
            Enhance cEnhance = null,
            BigFM cBigFM = null)
        {
            CEquip = equip;
            Extra.UpdateFrom(equip);
            if (equip == null)
            {
                UpdateWhenNotHasEquip(strengthLevel, cEnhance, cBigFM);
                return;
            }

            HasEquip = true;
            // 更新装备属性
            Head = new EquipShowHeadViewModel(equip, strengthLevel);
            Magic = new EquipShowMagicViewModel(equip, strengthLevel, diamond);

            UpdateStoneFrom(equip, cStone);
            UpdateEnhanceFrom(equip, cEnhance, cBigFM);
        }

        /// <summary>
        /// 注意这里传入的是真实精炼等级
        /// </summary>
        /// <param name="equip"></param>
        /// <param name="strengthLevel"></param>
        /// <param name="diamond"></param>
        /// <param name="cStone"></param>
        public void UpdateFrom(EquipStrengthDiamondModel model,
            Stone cStone = null,
            Enhance cEnhance = null,
            BigFM cBigFM = null)
        {
            CEquip = model.CEquip;
            Extra.UpdateFrom(model.CEquip);
            HasEquip = model.HasEquip;
            if (!HasEquip)
            {
                UpdateWhenNotHasEquip(model.StrengthModel.StrengthLevel, cEnhance, cBigFM);
                EquipShowBoxVM.UpdateWhenNotHasEquip();
                return;
            }

            // 更新装备属性
            Head = new EquipShowHeadViewModel(model.CEquip, model.StrengthModel.RealStrength);
            Magic = new EquipShowMagicViewModel(model);

            UpdateStoneFrom(model.CEquip, cStone);
            UpdateEnhanceFrom(model.CEquip, cEnhance, cBigFM);

            UpdateShowBox();
        }

        public void UpdateShowBox()
        {
            EquipShowBoxVM.Update();
        }

        public void UpdateWhenNotHasEquip(int strengthLevel = 0,
            Enhance cEnhance = null,
            BigFM cBigFM = null)
        {
            // 当未选中装备时调用
            HasEquip = false;
            Head = new EquipShowHeadViewModel(null, strengthLevel);
            Magic = null;

            if (Stone != null) Stone.HasStoneSlot = false;
            Enhance.HasBigFMSlot = false;
            Enhance.UpdateFrom(cEnhance, cBigFM);
        }

        public void UpdateStoneFrom(Equip e, Stone cStone = null)
        {
            // 更新五彩石
            if (e == null) return;
            if (e.SubTypeEnum == EquipSubTypeEnum.PRIMARY_WEAPON)
            {
                Stone.HasStoneSlot = true;
                Stone.ChangeStone(cStone);
            }
            else
            {
                Stone = EquipShowStoneViewModel.Empty;
            }
        }

        public void UpdateEnhanceFrom(Equip e, Enhance cEnhance = null,
            BigFM cBigFM = null)
        {
            // 更新附魔
            if (e == null) return;
            Enhance.HasBigFMSlot = e.SubTypeEnum.EquipSubTypeHasBigFMSlot();
            Enhance.UpdateFrom(cEnhance, cBigFM);
        }


        public void UpdateFrom(EquipSnapShotModel model)
        {
            UpdateFrom(model.EquipStrengthDiamond, model.CStone, model.CEnhance, model.CBigFM);
            Tail.UpdateFrom(model);
            Extra.UpdateFrom(model);
        }

        public List<string> GetDescList()
        {
            var res = new List<string>(30);
            res.AddRange(Head.GetDescList());
            res.Add("");
            res.AddRange(Magic.GetDescList());
            res.Add("");
            res.AddRange(CEquip.Attributes.GetDiamondDesc());
            res.Add("");
            res.AddRange(Tail.GetDescList());
            return res;
        }

        public void DisableExtra()
        {
            // 禁用额外内容
            Extra.ShowURL = false;
            //Extra = null;
        }

        public EquipShowViewModel GetCopyWithOutExtra()
        {
            // 生成一个新的，但是Extra可以修改
            var res = new EquipShowViewModel(this);
            res.Extra = new EquipShowExtraViewModel(this.Extra);
            res.DisableExtra();
            return res;
        }
    }
}