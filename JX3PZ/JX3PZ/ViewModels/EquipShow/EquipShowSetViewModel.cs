using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Utils;
using JX3PZ.Class;
using JX3PZ.Globals;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows.Data;
using System.Windows.Documents;

namespace JX3PZ.ViewModels
{
    public class EquipShowSetViewModel : ObservableObject
    {
        public bool HasSet { get; private set; } = false; // 是否有套装
        public readonly PzSet CPzSet;
        public string SetName { get; private set; } // 揽江·寒锁
        public int TotalCount { get; private set; } // 一共几件套
        public int Count { get; private set; } // 激活了几件套
        public string SetNameHeader { get; private set; } // 揽江·寒锁(4/5)

        public ImmutableArray<EquipShowSetEquipViewModel> SetEquips { get; private set; } // 装备列表
        public ImmutableArray<EquipShowSetEffectViewModel> SetEffects { get; private set; } // 激活的效果

        public PzSetResult Result { get; set; }

        public static readonly EquipShowSetViewModel Empty = new EquipShowSetViewModel(null); // 空的标头

        public EquipShowSetViewModel(PzSet p)
        {
            if (p == null)
            {
                HasSet = false;
                SetEquips = ImmutableArray<EquipShowSetEquipViewModel>.Empty;
                SetEffects = ImmutableArray<EquipShowSetEffectViewModel>.Empty;
                return;
            }

            HasSet = true;
            CPzSet = p;
            SetName = p.SetName;
            TotalCount = p.Num;

            var setEquips = ImmutableArray.CreateBuilder<EquipShowSetEquipViewModel>();
            for (int i = 0; i < p.Num; i++)
            {
                var v1 = new EquipShowSetEquipViewModel(p.EIDs[i], p.EquipNames[i]);
                setEquips.Add(v1);
            }

            SetEquips = setEquips.ToImmutableArray();

            var setEffects = ImmutableArray.CreateBuilder<EquipShowSetEffectViewModel>();

            foreach (var kvp in p.Attribute.Effects)
            {
                var v2 = new EquipShowSetEffectViewModel(kvp.Key, kvp.Value.Desc);
                setEffects.Add(v2);
            }

            SetEffects = setEffects.ToImmutableArray();
        }

        public void UpdateFrom(PzSetResult res)
        {
            if (res.CPzSet.ID != CPzSet.ID)
            {
                throw new ValueUnavailableException("套装ID错误！");
            }
            else
            {
                Result = res;
                Count = Result.Count;
            }

            GetHeader();
            GetSetEquips();
            GetSetEffects();
        }

        public void GetHeader()
        {
            SetNameHeader = $"{SetName}({Count}/{TotalCount})";
        }

        public void GetSetEquips()
        {
            Count = Result.Count;
            foreach (var _ in SetEquips)
            {
                _.Has = Result.EIDs.Contains(_.EID);
            }
        }

        public void GetSetEffects()
        {
            foreach (var _ in SetEffects)
            {
                _.IsActive = _.Count <= Result.Count;
            }
        }

        public void Clear()
        {
            Count = 0;
            GetHeader();
            foreach (var _ in SetEquips)
            {
                _.Has = false;
            }

            foreach (var _ in SetEffects)
            {
                _.IsActive = false;
            }
        }

        #region 流文档元素

        public Section GetSection()
        {
            if (!HasSet)
            {
                return null;
            }

            var sec = FlowDocumentTool.NewSection(tag: "Set");
            var p1 = GetSetEquipsParagraph();
            var p2 = GetSetEffectsParagraph();
            sec.AddParagraph(p1);
            sec.AddParagraph(p2);
            return sec;
        }

        public Paragraph GetSetEquipsParagraph()
        {
            var para = new Paragraph() { Tag = nameof(SetEquips) };
            var spans = new List<Span>(9);
            var setNameHeaderSpan = FlowDocumentTool.GetSpan(SetNameHeader, ColorConst.Yellow, nameof(SetNameHeader));
            var setEquipSpans = SetEquips.Select(_ => _.GetSpan());
            spans.Add(setNameHeaderSpan);
            spans.AddRange(setEquipSpans);
            para.AddLines(spans);
            return para;
        }

        public Paragraph GetSetEffectsParagraph()
        {
            var para = new Paragraph() { Tag = nameof(SetEffects) };
            var setSetEffectSpans = SetEffects.Select(_ => _.GetSpan());
            para.AddLines(setSetEffectSpans);
            return para;
        }

        #endregion
    }

    public class EquipShowSetEquipViewModel : ObservableObject
    {
        public string EID { get; }
        public string Name { get; }
        public bool Has { get; set; }
        public string Color => Has ? ColorConst.Yellow : ColorConst.Inactive;

        public EquipShowSetEquipViewModel(string eID, string name)
        {
            EID = eID;
            Name = name;
        }

        #region 流文档元素

        public Span GetSpan()
        {
            var span = FlowDocumentTool.GetSpan(Name, Color);
            return span;
        }

        #endregion
    }

    public class EquipShowSetEffectViewModel : ObservableObject
    {
        public int Count { get; } // 几件套（激活阈值）
        public string Desc { get; } // 描述
        public string CountDesc { get; } // [2] XXX
        public bool IsActive { get; set; } // 是否激活
        public string Color => IsActive ? ColorConst.Green : ColorConst.Inactive;

        public EquipShowSetEffectViewModel(int count, string desc)
        {
            Count = count;
            Desc = desc;
            CountDesc = $"[{Count}]{Desc}";
        }

        #region 流文档元素

        public Span GetSpan()
        {
            var span = FlowDocumentTool.GetSpan(CountDesc, Color);
            return span;
        }

        #endregion
    }
}