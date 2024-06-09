using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Utils;
using JX3PZ.Data;
using JX3PZ.Models;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace JX3PZ.ViewModels
{
    public class EquipShowStoneViewModel : ObservableObject
    {
        public bool HasStone { get; private set; } = false; //是否有五彩石
        public bool HasStoneSlot { get; set; } = false; // 是否有五彩石孔（仅有主武器才有五彩石孔）
        public Stone CStone { get; private set; }
        public int DiamondCount { get; private set; } = 0;
        public int DiamondIntensity { get; private set; } = 0;
        public int IconID { get; private set; } = -1000; // 未激活五彩石时图标位为-1000

        public bool[] IsActive; // 词条激活状态
        public ImmutableArray<StoneAttributeEntryViewModel> Entries { get; set; }

        public static ImmutableArray<StoneAttributeEntryViewModel> EmptyEntries { get; }

        public static EquipShowStoneViewModel Empty = new EquipShowStoneViewModel();

        static EquipShowStoneViewModel()
        {
            EmptyEntries = (new StoneAttributeEntryViewModel[] { StoneAttributeEntryViewModel.Empty }).ToImmutableArray();
        }

        public EquipShowStoneViewModel()
        {
            Entries = ImmutableArray<StoneAttributeEntryViewModel>.Empty;
        }

        public void ChangeStone(Stone stone)
        {
            CStone = stone;
            HasStone = CStone != null;
            if (HasStone)
            {
                Entries = stone.Attributes.GetAttributeEntryViewModels();
                IconID = stone.IconID;
            }
            else
            {
                Entries = EmptyEntries;
                IconID = -1000;
            }

            CheckActivate();
        }

        public void CheckActivate()
        {
            if (HasStone)
            {
                IsActive = CStone.Attributes.IsActive(DiamondCount, DiamondIntensity);
                UpdateActive(IsActive);
            }
            else
            {
                MakeInActive();
            }
        }

        public void UpdateActive(bool[] isActive)
        {
            IsActive = isActive;
            if (HasStone)
            {
                for (int i = 0; i < IsActive.Length; i++)
                {
                    Entries[i].IsActive = IsActive[i];
                }
            }
        }

        // 设置为未激活
        public void MakeInActive()
        {
            foreach (var e in Entries)
            {
                e.IsActive = false;
            }
        }

        public void UpdateActive(EquipStoneModel model)
        {
            UpdateActive(model.IsActive);
        }

        /// <summary>
        /// 更新五行石计数状态
        /// </summary>
        /// <param name="diamondCount">个数</param>
        /// <param name="diamondIntensity">等级和</param>
        public void UpdateDiamond(int diamondCount, int diamondIntensity)
        {
            DiamondCount = diamondCount;
            DiamondIntensity = diamondIntensity;
            CheckActivate();
        }

        #region FlowDocument元素生成

        public Table GetStoneTable()
        {
            if (!HasStoneSlot)
            {
                return null;
            }

            var spans = Entries.Select(_ => _.GetSpan()).ToList();
            int nrow = spans.Count;

            // 创建Table对象
            Table table = new Table() { Margin = new Thickness(0, 0, 0, 0) };

            // 定义列
            TableColumn column1 = new TableColumn() { Width = new GridLength(18) };
            TableColumn column2 = new TableColumn();
            table.Columns.Add(column1);
            table.Columns.Add(column2);

            // 创建图片所在的行
            TableRow imageRow = new TableRow();
            var trg_img = new TableRowGroup();
            trg_img.Rows.Add(imageRow);
            table.RowGroups.Add(trg_img);

            var path = BindingTool.IconID2Path(IconID);
            // 创建Image控件
            Image image = new Image
            {
                Source = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute)),
                Width = 18,
                Height = 18
            };

            // 创建包含图片的单元格，设置其跨越所有行
            TableCell imageCell = new TableCell(new BlockUIContainer(image))
            {
                RowSpan = nrow,
                TextAlignment = TextAlignment.Center
            };
            var firstEntryCell = new TableCell(new Paragraph(spans[0]));

            imageRow.Cells.Add(imageCell);
            imageRow.Cells.Add(firstEntryCell);

            spans.RemoveAt(0);

            // 添加描述文本的行
            foreach (var span in spans)
            {
                // 创建TableRow
                TableRow row = new TableRow();

                // 创建第一列的单元格（空）
                TableCell cell1 = new TableCell();
                row.Cells.Add(cell1);

                // 创建第二列的单元格并添加描述
                TableCell cell2 = new TableCell(new Paragraph(span));
                row.Cells.Add(cell2);
                var trg = new TableRowGroup();
                trg.Rows.Add(row);
                table.RowGroups.Add(trg);
            }

            // 返回包含Table的FlowDocument
            return table;
        }

        public Section GetSection()
        {
            var res = FlowDocumentTool.NewSection(tag: "Stone");
            res.Margin = new Thickness(0, 0, 0, 0);
            var tab = GetStoneTable();
            if (tab != null)
            {
                res.Blocks.Add(tab);
            }

            return res;
        }

        #endregion
    }
}