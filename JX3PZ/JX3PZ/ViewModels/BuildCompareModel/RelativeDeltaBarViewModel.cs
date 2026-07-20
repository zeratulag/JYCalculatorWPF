using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows;

namespace JX3PZ.ViewModels
{
    public class RelativeDeltaBarViewModel : ObservableObject
    {
        private const double MinVisibleRatio = 0.0001;

        public GridLength NegativeAxisWidth { get; private set; }
        public GridLength PositiveAxisWidth { get; private set; }
        public GridLength NegativeEmptyWidth { get; private set; }
        public GridLength NegativeBarWidth { get; private set; }
        public GridLength PositiveBarWidth { get; private set; }
        public GridLength PositiveEmptyWidth { get; private set; }
        public bool HasNegativeBar { get; private set; }
        public bool HasPositiveBar { get; private set; }
        public bool HasAxis { get; private set; }

        public RelativeDeltaBarViewModel()
        {
            Reset();
        }

        public void Update(double value, double maxPositive, double maxNegativeAbs, bool hasValue)
        {
            if (!hasValue || double.IsNaN(value))
            {
                Reset();
                return;
            }

            var hasPositiveScale = maxPositive > 0;
            var hasNegativeScale = maxNegativeAbs > 0;
            if (!hasPositiveScale && !hasNegativeScale)
            {
                Reset();
                return;
            }

            NegativeAxisWidth = ToStar(hasNegativeScale ? maxNegativeAbs : 0);
            PositiveAxisWidth = ToStar(hasPositiveScale ? maxPositive : 0);

            var positiveRatio = hasPositiveScale && value > 0 ? Math.Min(value / maxPositive, 1.0) : 0;
            var negativeRatio = hasNegativeScale && value < 0 ? Math.Min(Math.Abs(value) / maxNegativeAbs, 1.0) : 0;

            NegativeEmptyWidth = ToStar(1.0 - negativeRatio);
            NegativeBarWidth = ToStar(negativeRatio);
            PositiveBarWidth = ToStar(positiveRatio);
            PositiveEmptyWidth = ToStar(1.0 - positiveRatio);
            HasNegativeBar = negativeRatio > MinVisibleRatio;
            HasPositiveBar = positiveRatio > MinVisibleRatio;
            HasAxis = hasPositiveScale && hasNegativeScale;
            NotifyAll();
        }

        public void Reset()
        {
            NegativeAxisWidth = ToStar(1.0);
            PositiveAxisWidth = ToStar(1.0);
            NegativeEmptyWidth = ToStar(1.0);
            NegativeBarWidth = ToStar(0);
            PositiveBarWidth = ToStar(0);
            PositiveEmptyWidth = ToStar(1.0);
            HasNegativeBar = false;
            HasPositiveBar = false;
            HasAxis = false;
            NotifyAll();
        }

        private static GridLength ToStar(double value)
        {
            return new GridLength(Math.Max(0, value), GridUnitType.Star);
        }

        private void NotifyAll()
        {
            OnPropertyChanged(nameof(NegativeAxisWidth));
            OnPropertyChanged(nameof(PositiveAxisWidth));
            OnPropertyChanged(nameof(NegativeEmptyWidth));
            OnPropertyChanged(nameof(NegativeBarWidth));
            OnPropertyChanged(nameof(PositiveBarWidth));
            OnPropertyChanged(nameof(PositiveEmptyWidth));
            OnPropertyChanged(nameof(HasNegativeBar));
            OnPropertyChanged(nameof(HasPositiveBar));
            OnPropertyChanged(nameof(HasAxis));
        }
    }
}
