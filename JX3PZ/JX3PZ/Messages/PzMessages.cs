using JX3PZ.ViewModels;
using JYCalculator.Messages;
using JYCalculator.Src;

namespace JX3PZ.Messages
{
    public class PzChangedMessage
    {
        public readonly PzChangedEnum ChangedType;

        public PzChangedMessage(PzChangedEnum changedType)
        {
            ChangedType = changedType;
        }
    }

    public enum PzChangedEnum
    {
        Equip,
        Diamond,
        Enhance,
        BigFM,
        Stone,
    }

    public class PzOverviewMessage
    {
        public readonly CalculatorShell Shell;
    }

    public class PzEquipFilterMessage
    {
        public int MaxLevel;
        public int MinLevel;

        public PzEquipFilterMessage(int maxLevel, int minLevel)
        {
            MaxLevel = maxLevel;
            MinLevel = minLevel;
        }

        public PzEquipFilterMessage(EquipFilterArg a)
        {
            MaxLevel = a.MaxLevel;
            MinLevel = a.MinLevel;
        }
    }

    public class PzInfoMessage
    {
        public readonly string Title;
        public readonly string Author;

        public PzInfoMessage(string title, string author)
        {
            Title = title;
            Author = author;
        }
    }

}