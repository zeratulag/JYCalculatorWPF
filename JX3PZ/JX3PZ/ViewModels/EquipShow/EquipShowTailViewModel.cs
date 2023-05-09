using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using JX3PZ.Class;
using JX3PZ.Models;
using MathNet.Numerics.Optimization;

namespace JX3PZ.ViewModels
{
    public class EquipShowTailViewModel : ObservableObject, IEquipShow
    {
        public string SkillDesc { get; private set; } = "";// 主动使用技能的描述
        public bool HasSkillDesc => SkillDesc.IsNotEmptyOrWhiteSpace(); // 是否有
        public string ItemDesc { get; private set; } = "";// 物品描述
        public bool HasItemDesc => ItemDesc.IsNotEmptyOrWhiteSpace(); // 是否有
        public string RequireDesc { get; private set; } // 需要等级120
        public string DurabilityDesc { get; private set; } // 耐久度：5485/5485
        public bool HasDurability { get; private set; } // 是否有耐久度 
        public FormattedEntryViewModel Quality { get; private set; } // 品级
        public FormattedEntryViewModel Score { get; private set; } // 装分

        public EquipShowTailViewModel()
        {
        }

        public EquipShowTailViewModel(EquipSnapShotModel model)
        {
            UpdateFrom(model);
        }

        public void UpdateFrom(EquipSnapShotModel model)
        {
            if (model.CEquip != null)
            {
                SkillDesc = model.CEquip.SkillDesc;
                RequireDesc = model.CEquip.GetRequireDesc();
                HasDurability = model.CEquip.MaxDurability > 0;
                DurabilityDesc = model.CEquip.GetDurabilityDesc();
                Quality = new QualityEntryViewModel(model.Score);
                Score = new ScoreEntryViewModel(model.Score);
                ItemDesc = model.CEquip.ItemDesc;
            }
            else
            {
                SkillDesc = "";
                RequireDesc = "";
                HasDurability = false;
                DurabilityDesc = "";
                Quality = null;
                Score = null;
                ItemDesc = "";
            }
        }

        public List<string> GetDescList()
        {
            var res = new List<string>(10)
            {
                RequireDesc,
            };

            if (HasDurability)
            {
                res.Add(DurabilityDesc);
            }

            if (HasSkillDesc)
            {
                res.Add("");
                res.Add(SkillDesc);
            }
            res.Add("");
            res.Add(Quality.Text1);
            res.Add(Score.Text1);
            return res;
        }
    }
}