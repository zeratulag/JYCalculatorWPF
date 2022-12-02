﻿using System.Collections.Generic;
using System.Collections.Immutable;
using JX3CalculatorShared.Src.Data;

namespace JX3CalculatorShared.ViewModels 
{
    public class AbilityViewModel : ComboBoxViewModel<AbilityItem>
    {

        public readonly ImmutableDictionary<string, int> Item2Index;

        public AbilityViewModel(IEnumerable<AbilityItem> data) : base(data)
        {

            var dict = new Dictionary<string, int>(ItemsSource.Length);
            for (int i = 0; i < ItemsSource.Length; i++)
            {
                dict.Add(ItemsSource[i].ItemName, i);
            }
            Item2Index = dict.ToImmutableDictionary();
            SelectedIndex = Length - 1;
        }

        public string Export()
        {
            return SelectedItem.ItemName;
        }

        public void Import(string name)
        {
            SelectedIndex = Item2Index.GetValueOrDefault(name, 0);
        }

    }
}