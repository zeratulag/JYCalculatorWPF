using System.Collections;
using System.Runtime.Remoting.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace JX3CalculatorShared.ViewModels
{
    public class StringMessage
    {
        public readonly string Value;
        public StringMessage(string value)
        {
            Value = value;
        }

    }
}