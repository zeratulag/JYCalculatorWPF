using CommunityToolkit.Mvvm.Messaging;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Messages;

namespace JX3CalculatorShared.Globals
{
    public static class StaticMessager
    {
        public static class Senders
        {
            public const string QiXue = "QiXueChanged";
            public const string MiJi = "MiJiChanged";
            public const string BaoYuMiJi = "BaoYuMiJiChanged";
            public const string FightTime = "FightTimeChanged";
        }

        public static StringMessage QiXueChangedMsg = new StringMessage(Senders.QiXue); // 奇穴改变
        public static StringMessage MiJiChangedMsg = new StringMessage(Senders.MiJi); // 秘籍改变
        public static StringMessage BaoYuMiJiChangedMsg = new StringMessage(Senders.BaoYuMiJi); // 暴雨秘籍改变（影响气魄）
        public static StringMessage FightTimeChangedMsg = new StringMessage(Senders.FightTime); // 战斗时间改变

        public static void Send(StringMessage msg)
        {
            WeakReferenceMessenger.Default.Send(msg);
        }

        public static void Send(ImportJBMessage msg)
        {
            WeakReferenceMessenger.Default.Send(msg);
        }

        public static void Send(JBPZEquipSnapshotCollection plan)
        {
            var msg = new ImportJBMessage(plan);
            Send(msg);
        }
    }
}