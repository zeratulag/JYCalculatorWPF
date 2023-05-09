using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Messages;
using Syncfusion.UI.Xaml.Charts;

namespace JX3CalculatorShared.Class
{
    public class CalcInputViewModel: ObservableObject
    {
        public QiXueSkill[] QiXues { get; private set; }
        public ItemDT[] ItemDTs { get; private set; }
        public Dictionary<BuffTypeEnum, BuffViewModel[]> BuffDict;
        public BuffViewModel[] Buffs { get; private set; } // 自身增益
        public BuffViewModel[] DeBuffs { get; private set; } // 目标减益
        public FightOptionSummaryViewModel FightOption { get; private set; } // 战斗选项
        public bool HasItemDT => ItemDTs.IsNotEmptyOrNull();
        public bool HasBuffs => Buffs.IsNotEmptyOrNull();
        public bool HasDeBuffs => DeBuffs.IsNotEmptyOrNull();

        public CalcInputViewModel()
        {
        }

        public CalcInputViewModel(QiXueSkill[] qiXues, ItemDT[] itemDTs,
            FightOptionSummaryViewModel fightOption,
            Dictionary<BuffTypeEnum, BuffViewModel[]> buffDict)
        {
            QiXues = qiXues;
            ItemDTs = itemDTs;
            FightOption = fightOption;
            BuffDict = buffDict;
        }

        public void UpdateFrom(CalcInputViewModel old)
        {
            QiXues = old.QiXues;
            ItemDTs= old.ItemDTs;
            FightOption = old.FightOption;
            BuffDict = old.BuffDict;
            Calc();
        }

        public static bool HasItems<T>(T[] arr)
        {
            return (arr != null && arr.Length > 0);
        }

        public void UpdateFrom(CalcResultMessage message)
        {
            UpdateFrom(message.Input);
        }

        public void Calc()
        {
            GetBuffs(); // buff分类
        }

        public void GetBuffs()
        {
            var buffs = new List<BuffViewModel>(BuffDict.Count);
            var deBuffs = new List<BuffViewModel>(10);
            foreach (var kvp in BuffDict)
            {
                if (kvp.Key == BuffTypeEnum.DeBuff_Normal)
                {
                    deBuffs.AddRange(kvp.Value);
                }
                else
                {
                    buffs.AddRange(kvp.Value);
                }
            }
            Buffs = buffs.ToArray();
            DeBuffs = deBuffs.ToArray();
        }

    }
}