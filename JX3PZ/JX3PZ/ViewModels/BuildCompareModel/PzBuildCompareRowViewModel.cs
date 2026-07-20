using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Utils;
using JYCalculator.Class;
using JYCalculator.Models;
using PropertyChanged;
using System.Collections.Generic;

namespace JX3PZ.ViewModels
{
    public class PzBuildCompareRowViewModel : ObservableObject
    {
        public BuildCompareCandidate Candidate { get; }
        public bool IsCurrent { get; }
        public JX3PZ.ViewModels.EquipShowViewModel EquipShowVM { get; }
        public string Name { get; }
        public int Level { get; }
        public string LevelText { get; }
        public int IconID { get; }
        public JX3CalculatorShared.Globals.QualityEnum Quality { get; }
        public string AttributeDesc { get; }
        public bool HasIcon => IconID != 0;
        public bool HasEquipShow => EquipShowVM != null;

        public double Dps { get; private set; }

        [AlsoNotifyFor(nameof(DeltaDpsPct), nameof(DeltaDpsColor), nameof(DeltaDpsPctColor))]
        public double DeltaDps { get; private set; }

        [AlsoNotifyFor(nameof(StatusColor))]
        public PzBuildCompareSlotStatusText StatusTextKind { get; private set; }
        public string Status { get; private set; }
        public string ErrorMessage { get; private set; }
        public double BaseDps { get; private set; }
        public int Rank { get; private set; }
        public RelativeDeltaBarViewModel RelativeDeltaBar { get; }

        public double DeltaDpsPct => BaseDps <= 0 ? 0 : DeltaDps / BaseDps;
        public string RankText => Rank > 0 ? Rank.ToString() : "—";

        public string DeltaDpsColor => DeltaDisplayTool.GetPointColor(DeltaDps);
        public string DeltaDpsPctColor => DeltaDisplayTool.GetPercentColor(DeltaDpsPct);
        public string StatusColor => StatusTextKind.GetStatusColor();

        public bool HasValue => Dps > 0.5;

        public bool IsEmpty => Candidate.IsEmpty;

        public PzBuildCompareRowViewModel(BuildCompareCandidate candidate, double baseDps)
        {
            Candidate = candidate;
            IsCurrent = candidate.IsCurrent;
            EquipShowVM = candidate.EquipShowVM;
            Name = candidate.Name;
            Level = candidate.Level;
            LevelText = candidate.LevelText ?? candidate.Level.ToString();
            IconID = candidate.IconID;
            Quality = candidate.Quality;
            AttributeDesc = candidate.AttributeDesc;
            BaseDps = baseDps;
            RelativeDeltaBar = new RelativeDeltaBarViewModel();
            SetStatus(PzBuildCompareSlotStatusText.等待);
            ErrorMessage = "";
        }

        public void BeginCalc()
        {
            SetStatus(PzBuildCompareSlotStatusText.计算中);
            ErrorMessage = "";
            RelativeDeltaBar.Reset();
        }

        public void UpdateFrom(BuildCompareResultItem item)
        {
            if (item.Success)
            {
                Dps = item.Dps;
                DeltaDps = item.DeltaDps;
                SetStatus(PzBuildCompareSlotStatusText.完成);
            }
            else
            {
                Dps = double.NaN;
                DeltaDps = double.NaN;
                SetStatus(PzBuildCompareSlotStatusText.计算失败,
                    PzBuildCompareDisplayTool.GetStatusText(item.CalcResult));
                RelativeDeltaBar.Reset();
            }
            ErrorMessage = item.ErrorMessage;
            OnPropertyChanged(nameof(HasValue));
        }

        public void SetRank(int rank)
        {
            Rank = rank;
            OnPropertyChanged(nameof(RankText));
        }

        private void SetStatus(PzBuildCompareSlotStatusText statusTextKind, string status = null)
        {
            StatusTextKind = statusTextKind;
            Status = status ?? statusTextKind.ToString();
        }
    }

    public static class PzBuildCompareDisplayTool
    {
        private static readonly Dictionary<CalcResultStatus, string> StatusTextDict = new Dictionary<CalcResultStatus, string>
        {
            { CalcResultStatus.InvalidInput, "输入无效" },
            { CalcResultStatus.UnsupportedSkillBuild, "奇穴/秘籍不支持" },
            { CalcResultStatus.UnsupportedHaste, "加速不支持" },
            { CalcResultStatus.Error, "计算失败" }
        };

        public static string GetStatusText(CalcResult result)
        {
            if (result == null)
            {
                return "无效";
            }

            string text;
            if (StatusTextDict.TryGetValue(result.Status, out text))
            {
                return text;
            }

            if (!string.IsNullOrEmpty(result.FailReason))
            {
                return result.FailReason;
            }

            return "无效";
        }
    }
}
