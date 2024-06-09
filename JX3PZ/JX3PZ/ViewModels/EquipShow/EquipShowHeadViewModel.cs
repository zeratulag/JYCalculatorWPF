using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Utils;
using JX3PZ.Class;
using JX3PZ.Data;
using JX3PZ.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace JX3PZ.ViewModels
{
    public class EquipShowHeadViewModel : ObservableObject, IEquipShow
    {
        // 装备头
        public bool HasEquip { get; private set; } = false;

        public string Name { get; private set; } // 装备名称
        public string StrengthStar1 { get; private set; } // 强化星星，★
        public string StrengthStar2 { get; private set; } // 未强化的星星，☆
        public int StrengthLevel { get; private set; } = 0; // 强化等级
        public int MaxStrengthLevel { get; private set; } = 6; // 最大强化等级
        public string SubTypeName { get; private set; } // 类型名称，例如 近身武器

        public int Quality { get; private set; }
        public string WeaponDetailTypeName { get; set; } // 武器详细类型名称，例如 千机匣
        public string WeaponSpeed { get; set; } // 速度，例如 速度 1.0

        public string StrengthStr => $"精炼等级: {StrengthLevel} / {MaxStrengthLevel}";
        public AttributeEntryViewModel[] BaseAttributeVMs { get; protected set; } // 基础属性词条

        public bool HasBaseAttributes { get; private set; } // 是否有基础属性
        public bool IsWeapon { get; private set; } // 是否为武器

        public EquipShowHeadViewModel()
        {
        }

        public EquipShowHeadViewModel(Equip equip, int strengthLevel = 0)
        {
            UpdateFrom(equip, strengthLevel);
        }


        public void UpdateFrom(Equip equip, int strengthLevel = 0)
        {
            HasEquip = equip != null;
            if (!HasEquip)
            {
                return;
            }

            Name = equip.Name;
            Quality = equip.Quality;
            MaxStrengthLevel = equip.MaxStrengthLevel;
            StrengthLevel = Math.Min(strengthLevel, MaxStrengthLevel);
            StrengthStar1 = new string('★', StrengthLevel);
            StrengthStar2 = new string('☆', MaxStrengthLevel - StrengthLevel);
            var map = EquipMapLib.EquipSubType[equip.SubType];
            SubTypeName = map.Label;

            HasBaseAttributes = equip.Attributes.BaseEntry.Any();

            IsWeapon = equip.IsWeapon;

            Weapon wp = null;
            if (IsWeapon)
            {
                wp = equip as Weapon;
                WeaponDetailTypeName = EquipMapLib.Weapon[wp.DetailType].Weapon;
            }

            if (HasBaseAttributes)
            {
                if (IsWeapon)
                {
                    WeaponSpeed = $"速度 {wp?.WeaponBase.WeaponSpeed:F1}";
                    BaseAttributeVMs = wp?.WeaponBase.GetViewModel();
                }
                else
                {
                    BaseAttributeVMs = equip.Attributes.BaseEntry.Select(_ => _.GetViewModel()).ToArray();
                }
            }
            else
            {
                BaseAttributeVMs = null;
            }
        }

        public List<string> GetDescList()
        {
            var res = new List<string>(10)
            {
                $"{Name}\t\t0/{MaxStrengthLevel}",
            };

            if (IsWeapon)
            {
                res.Add($"{SubTypeName}\t\t{WeaponDetailTypeName}");
            }
            else
            {
                res.Add($"{SubTypeName}");
            }

            if (HasBaseAttributes)
            {
                res.AddRange(BaseAttributeVMs.Select(_ => _.Desc));
            }

            return res;
        }

        #region 构建FlowDocument元素

        public Section GetSection()
        {
            if (!HasEquip) return null;

            // 创建一个新的Section
            Section section = FlowDocumentTool.NewSection(tag: "Head");
            var table = GetEquipNameTable();
            if (IsWeapon)
            {
                GetWeaponNameTable(table);
            }

            section.Blocks.Add(table);

            var para = GetBaseAttributeParagraph();
            if (para != null)
            {
                section.Blocks.Add(para);
            }

            return section;
        }

        // 构建第一行（包括装备名称，精炼星星，精炼等级）
        public Table GetEquipNameTable()
        {
            // 创建一个新的Table并设置Margin属性
            Table table = new Table
            { Margin = new Thickness(0), BorderThickness = new Thickness(0), Foreground = Brushes.White, Tag = "HeadDetail" };
            // 定义两个列并设置它们的宽度
            TableColumn column1 = new TableColumn { Width = GridLength.Auto };
            TableColumn column2 = new TableColumn { Width = GridLength.Auto };
            TableColumn column3 = new TableColumn { Width = GridLength.Auto };

            // 将列添加到Table的列集合中
            table.Columns.Add(column1);
            table.Columns.Add(column2);
            table.Columns.Add(column3);

            // 创建一个TableRowGroup
            TableRowGroup trg = new TableRowGroup();

            // 创建一个TableRow
            TableRow row = new TableRow();

            TableCell cell1 = new TableCell() { BorderThickness = new Thickness(0) };
            cell1.ColumnSpan = 2;
            Paragraph para1 = new Paragraph();
            // 添加Run元素到Paragraph
            Run runName = new Run(Name) { Foreground = SolidColorBrushLib.FromQuality(Quality, true), Tag = nameof(Name) };
            Run runStrengthStar1 = new Run(StrengthStar1) { Foreground = Brushes.Yellow, Tag = nameof(StrengthStar1) };
            Run runStrengthStar2 = new Run(StrengthStar2) { Tag = nameof(StrengthStar2) };
            // 将Run元素添加到Paragraph中
            para1.Inlines.Add(runName);
            para1.Inlines.Add(" ");
            para1.Inlines.Add(runStrengthStar1);
            para1.Inlines.Add(runStrengthStar2);
            // 将Paragraph添加到TableCell中
            cell1.Blocks.Add(para1);


            // 创建第二个TableCell并设置文本对齐
            TableCell cell3 = new TableCell() { BorderThickness = new Thickness(0) };
            // 创建第二个Paragraph并设置文本对齐
            Paragraph para3 = new Paragraph { TextAlignment = TextAlignment.Right };
            // 添加Run元素到Paragraph
            Run runStrengthStr = new Run(StrengthStr)
            { Foreground = SolidColorBrushLib.FromColor(ColorConst.Strength, true), Tag = nameof(StrengthStr) };
            // 将Run元素添加到Paragraph中
            para3.Inlines.Add(runStrengthStr);
            // 将Paragraph添加到TableCell中
            cell3.Blocks.Add(para3);

            // 将TableCells添加到TableRow中
            row.Cells.Add(cell1);
            row.Cells.Add(cell3);

            // 将TableRow添加到TableRowGroup中
            trg.Rows.Add(row);

            //// 将TableRowGroup添加到Table中
            table.RowGroups.Add(trg);

            // 普通装备的详细分类（例如帽子）

            // 创建一个TableRow
            TableRow row2 = new TableRow();
            TableCell cellSubTypeName = new TableCell() { BorderThickness = new Thickness(0) };
            Paragraph paraSubTypeName = new Paragraph();
            paraSubTypeName.Inlines.Add(GetSubTypeNameRun());
            cellSubTypeName.Blocks.Add(paraSubTypeName);
            row2.Cells.Add(cellSubTypeName);

            TableRowGroup trg2 = new TableRowGroup();
            trg2.Rows.Add(row2);
            table.RowGroups.Add(trg2);

            return table;
        }

        public Run GetSubTypeNameRun()
        {
            var res = new Run(SubTypeName) { Tag = nameof(SubTypeName) };
            return res;
        }

        // 当是武器时，需要更新头
        public Table GetWeaponNameTable(Table table)
        {
            table.RemoveLastRow();

            // 第一个TableRow
            TableRow row1 = new TableRow();

            // 第一个TableRow的第一个TableCell
            TableCell cell1_row1 = new TableCell(new Paragraph(GetSubTypeNameRun()));
            cell1_row1.ColumnSpan = 2;
            row1.Cells.Add(cell1_row1);

            // 第一个TableRow的第二个TableCell
            TableCell cell2_row1 = new TableCell(new Paragraph(new Run(WeaponDetailTypeName) { Tag = nameof(WeaponDetailTypeName) })
            { TextAlignment = TextAlignment.Right });
            row1.Cells.Add(cell2_row1);

            // 第二个TableRow
            TableRow row2 = new TableRow();

            // 第二个TableRow的第一个TableCell
            var wpAttrDesc = BaseAttributeVMs.First().Desc;
            TableCell cell1_row2 = new TableCell(new Paragraph(new Run(wpAttrDesc)));
            cell1_row2.ColumnSpan = 2;
            row2.Cells.Add(cell1_row2);

            // 第二个TableRow的第二个TableCell
            TableCell cell2_row2 = new TableCell(new Paragraph(new Run(WeaponSpeed) { Tag = nameof(WeaponSpeed) })
            { TextAlignment = TextAlignment.Right });
            row2.Cells.Add(cell2_row2);

            // 如果你需要将这些TableRow添加到Table和TableRowGroup，你可以这样做：
            // 假设你已经有了一个Table和一个TableRowGroup对象
            TableRowGroup trg = new TableRowGroup();

            // 添加TableRow到TableRowGroup
            trg.Rows.Add(row1);
            trg.Rows.Add(row2);

            // 将TableRowGroup添加到Table
            table.RowGroups.Add(trg);
            table.SetColumnWidthAuto();
            return table;
        }

        // 基础属性（白字）
        public Paragraph GetBaseAttributeParagraph()
        {
            Paragraph para = null;
            if (BaseAttributeVMs != null)
            {
                var res = BaseAttributeVMs.Select(_ => _.GetSpan()).ToList();
                if (IsWeapon)
                {
                    res.RemoveAt(0);
                }

                para = FlowDocumentTool.NewParagraph(tag: "BaseAttribute");
                para.AddLines(res);
            }

            return para;
        }

        #endregion
    }
}