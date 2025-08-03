using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Utils;
using JYCalculator.Data;
using Syncfusion.UI.Xaml.Grid;

namespace JYCalculator.Class
{
    // 表示单个技能的类
    public class BaseSkillRatio
    {
        public string BaseSkillName { get; }
        public string BaseSkillKey { get; }
        public SkillRatioItem[] RatioItems { get; }

        public bool IsXinWu { get; } // 是否为心无
        public bool IsChengWu { get; } // 是否为橙武

        public BaseSkillRatio(IEnumerable<SkillRatioItem> items)
        {
            RatioItems = items.ToArray();

            BaseSkillName = RatioItems.First().BaseSkillName;
            BaseSkillKey = RatioItems.First().BaseSkillKey;
            IsXinWu = RatioItems.First().XinWu > 0;
            IsChengWu = RatioItems.First().ChengWu > 0;

            // 判断所有 items 的 BaseSkillName 是否一致
            if (RatioItems.Any(item => item.BaseSkillName != BaseSkillName))
            {
                throw new ArgumentException("All items must have the same BaseSkillName.");
            }
        }

        // 对基础技能数进行分配
        public Dictionary<string, double> CalcSkillNum(double baseSkillNum)
        {
            var res = new Dictionary<string, double>(RatioItems.Length);
            foreach (var item in RatioItems)
            {
                var key = item.DerivedSkillInfo.Name;
                var value = item.BaseSkillRatio * baseSkillNum;
                res[key] = value;
            }
            return res;
        }

        // 就地分配修改修改技能数
        public void AllocateSkillNum(Dictionary<string, double> baseSkillNumDict)
        {
            var baseSkillNum = baseSkillNumDict[BaseSkillKey];
            baseSkillNumDict.Remove(BaseSkillKey); // 基础技能数归零
            var res = CalcSkillNum(baseSkillNum);
            baseSkillNumDict.ValueDictAppend(res);
        }

    }

    public class BaseSkillRatioGroup
    {
        public ImmutableDictionary<string, BaseSkillRatio> RatioGroup { get; }

        // 基于SkillRatioItem的BaseSkillName进行分组构建BaseSkillRatio对象
        public BaseSkillRatioGroup(IEnumerable<SkillRatioItem> items)
        {
            var groupedItems = items.GroupBy(item => item.BaseSkillKey);
            RatioGroup = groupedItems.ToImmutableDictionary(
                group => group.Key,
                group => new BaseSkillRatio(group));
        }

        public Dictionary<string, double> CalcSkillNum(IDictionary<string, double> baseSkillNumDict)
        {
            var res = new Dictionary<string, double>();
            foreach (var group in RatioGroup.Values)
            {
                var groupRes = group.CalcSkillNum(baseSkillNumDict[group.BaseSkillKey]);
                groupRes.ForEach(kvp => res.Add(kvp));
            }
            return res;
        }

        // 就地修改一个baseSkillNumDict
        public void AllocateSkillNum(Dictionary<string, double> baseSkillNumDict)
        {
            foreach (var group in RatioGroup.Values)
            {
                group.AllocateSkillNum(baseSkillNumDict);
            }
        }

        public void AllocateSkillNum(SkillFreqDict skillFreq)
        {
            AllocateSkillNum(skillFreq.Data);
        }


        // 区分心无和非心无期间的
        public static Period<BaseSkillRatioGroup> MakePeriodBaseSkillRatioGroup(IEnumerable<SkillRatioItem> items)
        {
            var gItems = items.GroupBy(item => item.XinWu);
            var res = gItems.ToDictionary(g => g.Key, g => new BaseSkillRatioGroup(g));
            var normal = res[0];
            var xinWu = res[1];
            var result = new Period<BaseSkillRatioGroup>(normal, xinWu);
            return result;
        }

        // 区分普通武器和橙武不同流派的, 0: 普通武器， 1: 橙武
        public static Period<BaseSkillRatioGroup>[] MakeMultiGenrePeriodBaseSkillRatioGroup(
            IEnumerable<SkillRatioItem> items)
        {
            var gItems = items.GroupBy(item => item.ChengWu);
            var res_dict = gItems.ToDictionary(g => g.Key, MakePeriodBaseSkillRatioGroup);
            var normal = res_dict[0];
            var chengWu = res_dict[1];
            var res = new Period<BaseSkillRatioGroup>[] { normal, chengWu};
            return res;
        }

    }
}