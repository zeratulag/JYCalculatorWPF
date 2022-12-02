using System.Windows.Input;

namespace JX3CalculatorShared.ViewModels
{
    public class DataCommands
    {
        private static RoutedUICommand requery;

        static DataCommands()
        {
            requery = new RoutedUICommand("Requery", "Requery", typeof(DataCommands));
        }

        public static RoutedUICommand Requery => requery;
    }
}