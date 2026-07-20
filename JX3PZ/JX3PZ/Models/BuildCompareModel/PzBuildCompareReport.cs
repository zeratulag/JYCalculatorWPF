using JX3PZ.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace JX3PZ.Models
{
    public class PzBuildCompareRowReport
    {
        public string Name { get; set; }
        public string LevelText { get; set; }
        public string AttributeDesc { get; set; }
        public double Dps { get; set; }
        public double DeltaDps { get; set; }
        public double DeltaDpsPct { get; set; }
        public int Rank { get; set; }
        public string Status { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsEmpty { get; set; }

        public PzBuildCompareRowReport(PzBuildCompareRowViewModel row)
        {
            Name = row.Name;
            LevelText = row.LevelText;
            AttributeDesc = row.AttributeDesc;
            Dps = row.Dps;
            DeltaDps = row.DeltaDps;
            DeltaDpsPct = row.DeltaDpsPct;
            Rank = row.Rank;
            Status = row.Status;
            IsCurrent = row.IsCurrent;
            IsEmpty = row.Candidate.IsEmpty;
        }
    }

    public class PzBuildCompareSlotReport
    {
        public int Position { get; set; }
        public string Header { get; set; }
        public string Label { get; set; }
        public double BaseDps { get; set; }
        public string StatusText { get; set; }
        public List<PzBuildCompareRowReport> Rows { get; set; }

        public PzBuildCompareSlotReport(PzBuildCompareSlotViewModel slot)
        {
            Position = slot.Position;
            Header = slot.Header;
            Label = slot.Label;
            BaseDps = slot.BaseDps;
            StatusText = slot.StatusText;
            var validItems = slot.Items.Where(e => !e.IsEmpty);
            Rows = validItems.Select(item => new PzBuildCompareRowReport(item)).ToList();
        }
    }

}
