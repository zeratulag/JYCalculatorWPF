﻿using JX3PZ.Class;
using JX3PZ.Globals;

namespace JX3PZ.ViewModels
{
    public class StoneAttributeEntryViewModel : AttributeEntryViewModel
    {
        // 五彩石VM
        public bool IsActive { get; set; } = true;// 是否激活

        public override string Color => IsActive ? ColorConst.Green : ColorConst.Inactive;

        public StoneAttributeEntryViewModel(string desc, string color = "#000000") : base(desc, color)
        {
        }

        public StoneAttributeEntryViewModel(AttributeEntry entry) : this(entry.Desc)
        {
        }

        public static StoneAttributeEntryViewModel Empty = new StoneAttributeEntryViewModel("<只能镶嵌五彩石>", ColorConst.Inactive); // 空五彩石槽位
    }
}