using JX3PZ.Class;
using JX3PZ.ViewModels;
using JYCalculator.Models;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using System.Collections.Generic;
using System.Linq;

namespace JX3PZ.Models
{
    public class PzEquipBuildCompareCandidateProvider
    {
        public PzPlanModel SourceModel { get; }
        public JBPZEquipSnapshotCollection SourceCollection { get; }
        public string Title { get; }
        public PzTabItemViewModel SlotVM { get; }
        public PzBuildVariantFactory Factory { get; }

        public PzEquipBuildCompareCandidateProvider(PzPlanModel sourceModel, PzTabItemViewModel slotVM)
        {
            SourceModel = sourceModel;
            SourceCollection = sourceModel.ExportJBPZEquipSnapshotCollection();
            Title = sourceModel.XFPanel?.Name;
            SlotVM = slotVM;
            Factory = new PzBuildVariantFactory();
        }

        public IList<BuildCompareCandidate> GetCandidates(bool includeEmptyCandidate)
        {
            var equips = SlotVM.EquipSelectVM.ItemsSourceView
                .Cast<Equip>()
                .Where(equip => equip != null && equip.IsValid)
                .GroupBy(equip => equip.EID)
                .Select(group => group.First())
                .ToList();

            var current = SlotVM.EquipSelectVM.SelectedItem;
            if (current != null && current.IsValid && equips.All(equip => equip.EID != current.EID))
            {
                equips.Insert(0, current);
            }

            var candidates = equips.Select(CreateCandidate).ToList();
            if (includeEmptyCandidate)
            {
                candidates.Insert(0, CreateEmptyCandidate());
            }
            return candidates;
        }

        private BuildCompareCandidate CreateCandidate(Equip equip)
        {
            var current = SlotVM.EquipSelectVM.SelectedItem;
            return new BuildCompareCandidate
            {
                Key = equip.EID,
                Name = equip.Name,
                Category = "装备",
                Equip = equip,
                Level = equip.Level,
                LevelText = equip.Level.ToString(),
                IconID = equip.IconID,
                Quality = (QualityEnum)equip.Quality,
                EquipShowVM = equip.EquipShowVM,
                AttributeDesc = GetAttributeDesc(equip),
                IsCurrent = current != null && current.EID == equip.EID,
                CreateInitInput = () => Factory.CreateEquipInitInput(SourceCollection, SlotVM.Position, equip, Title)
            };
        }

        private BuildCompareCandidate CreateEmptyCandidate()
        {
            var current = SlotVM.EquipSelectVM.SelectedItem;
            return new BuildCompareCandidate
            {
                Key = $"EMPTY_{SlotVM.Position}",
                Name = "空",
                Category = "空装备",
                Level = 0,
                LevelText = "—",
                IconID = -1,
                Quality = QualityEnum.None,
                EquipShowVM = null,
                AttributeDesc = "—",
                IsCurrent = current == null || !current.IsValid,
                IsEmpty = true,
                CreateInitInput = () => Factory.CreateEquipInitInput(SourceCollection, SlotVM.Position, null, Title)
            };
        }

        private static string GetAttributeDesc(Equip equip)
        {
            var parts = new[] { equip.BasicTag_Str, equip.ExtraTag_Str }
                .Where(text => !string.IsNullOrWhiteSpace(text));
            return string.Join(" ", parts);
        }
    }
}
