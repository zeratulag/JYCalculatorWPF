using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using MiniExcelLibs.OpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JX3PZ.Models
{
    public class PzBuildCompareRowExportItem
    {
        [ExcelColumnName("排名")]
        [ExcelColumn(Index = 0, Width = 8)]
        public int? Rank { get; set; }

        [ExcelColumnName("装备")]
        [ExcelColumn(Index = 1, Width = 22)]
        public string Name { get; set; }

        [ExcelColumnName("品级")]
        [ExcelColumn(Index = 2, Width = 10)]
        public int? Level { get; set; }

        [ExcelColumnName("属性")]
        [ExcelColumn(Index = 3, Width = 25)]
        public string AttributeDesc { get; set; }

        [ExcelColumnName("DPS")]
        [ExcelColumn(Index = 4, Width = 14)]
        public double? Dps { get; set; }

        [ExcelColumnName("DPS差值")]
        [ExcelColumn(Index = 5, Width = 16)]
        public double? DeltaDps { get; set; }

        [ExcelColumnName("相对差异")]
        [ExcelColumn(Index = 6, Width = 12)]
        [ExcelFormat("0.00%")]
        public double? DeltaDpsPct { get; set; }

        [ExcelColumnName("状态")]
        [ExcelColumn(Index = 7, Width = 14)]
        public string Status { get; set; }

        [ExcelColumnName("当前装备")]
        [ExcelColumn(Index = 8, Width = 10)]
        public string IsCurrent { get; set; }

        public PzBuildCompareRowExportItem(PzBuildCompareRowReport row)
        {
            Rank = row.Rank > 0 ? row.Rank : (int?)null;
            Name = row.Name;
            Level = int.TryParse(row.LevelText, out var level) ? level : (int?)null;
            AttributeDesc = row.AttributeDesc;
            Dps = ToNullable(row.Dps);
            DeltaDps = ToNullable(row.DeltaDps);
            DeltaDpsPct = ToNullable(row.DeltaDpsPct);
            Status = row.Status;
            IsCurrent = row.IsCurrent ? "是" : "";
        }

        private static double? ToNullable(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                return null;
            }

            return value;
        }
    }

    public static class PzBuildCompareReportExporter
    {
        private static readonly char[] InvalidSheetNameChars = { '\\', '/', '?', '*', '[', ']', ':' };

        public static void ExportSlot(string path, PzBuildCompareSlotReport report)
        {
            ExportAll(path, new[] { report });
        }

        public static void ExportAll(string path, IEnumerable<PzBuildCompareSlotReport> reports)
        {
            EnsureOutputPath(path);

            var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var sheets = reports
                .OrderBy(report => report.Position)
                .ToDictionary(
                    report => CreateSheetName(report, usedNames),
                    report => (object)report.Rows.Select(row => new PzBuildCompareRowExportItem(row)).ToList());

            var config = new OpenXmlConfiguration
            {
                AutoFilter = true
            };

            MiniExcel.SaveAs(path, sheets, configuration: config);
        }

        public static string CreateDefaultFileName(string scopeText)
        {
            var timestr = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            var rawName = $"装备智能对比_{scopeText}_{timestr}.xlsx";
            return SanitizeFileName(rawName);
        }

        private static void EnsureOutputPath(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private static string CreateSheetName(PzBuildCompareSlotReport report, HashSet<string> usedNames)
        {
            var baseName = SanitizeSheetName($"{report.Position:00}-{report.Header}");
            if (string.IsNullOrWhiteSpace(baseName))
            {
                baseName = $"Slot{report.Position:00}";
            }

            var sheetName = TrimSheetName(baseName);
            var suffix = 1;
            while (usedNames.Contains(sheetName))
            {
                var suffixText = $"_{suffix}";
                sheetName = TrimSheetName(baseName, suffixText.Length) + suffixText;
                suffix++;
            }

            usedNames.Add(sheetName);
            return sheetName;
        }

        private static string SanitizeSheetName(string name)
        {
            var result = name;
            foreach (var invalidChar in InvalidSheetNameChars)
            {
                result = result.Replace(invalidChar, '-');
            }

            return result.Trim();
        }

        private static string TrimSheetName(string name, int reservedLength = 0)
        {
            var maxLength = Math.Max(1, 31 - reservedLength);
            return name.Length <= maxLength ? name : name.Substring(0, maxLength);
        }

        private static string SanitizeFileName(string fileName)
        {
            var result = fileName;
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
            {
                result = result.Replace(invalidChar, '-');
            }

            return result;
        }
    }
}
