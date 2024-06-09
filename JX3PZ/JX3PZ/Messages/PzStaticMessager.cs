using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;

namespace JX3PZ.Messages
{
    public static class PzStaticMessager
    {
        public static readonly Dictionary<PzChangedEnum, PzChangedMessage> ChangedLib;

        static PzStaticMessager()
        {
            ChangedLib = new Dictionary<PzChangedEnum, PzChangedMessage>(6);
        }

        private static PzChangedMessage GetMessage(PzChangedEnum pzChanged)
        {
            if (ChangedLib.TryGetValue(pzChanged, out PzChangedMessage msg))
            {
            }
            else
            {
                msg = new PzChangedMessage(pzChanged);
                ChangedLib.Add(pzChanged, msg);
            }

            return msg;
        }

        private static void Send(PzChangedMessage msg)
        {
            WeakReferenceMessenger.Default.Send(msg);
        }

        public static void Send(PzChangedEnum pzChanged)
        {
            Send(GetMessage(pzChanged));
        }
    }
}