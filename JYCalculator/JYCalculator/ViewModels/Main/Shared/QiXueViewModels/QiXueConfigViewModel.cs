using JX3CalculatorShared.Globals;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Class;
using JYCalculator.Data;
using JYCalculator.DB;
using JYCalculator.Models;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows.Interop;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using JX3CalculatorShared.Class;

namespace JYCalculator.ViewModels
{
    public class QiXueConfigViewModel : CollectionViewModel<QiXueSlotViewModel>
    {
        #region 成员

        public static readonly int NSLOTS = StaticConst.NumberOfQiXue;

        public string ShortNameDesc { get; private set; }
        public int[] Orders { get; private set; }
        public string HeaderName { get; private set; }

        public readonly string[] ItemNames; //存储奇穴名称

        public QiXueSkill[] SelectedQiXueSkills = null;

        public readonly QiXueConfigModel Model;

        #endregion

        #region 构造

        public QiXueConfigViewModel(QiXueDB db)
        {
            Data = db.QiXue.Select(data => new QiXueSlotViewModel(data)).ToImmutableArray();
            AttachDependVMsOutputChanged();
            PostConstructor();
            ItemNames = new string[NSLOTS];
            Model = new QiXueConfigModel();
            _Update();
        }

        public QiXueConfigViewModel() : this(StaticXFData.DB.QiXue)
        {
        }

        #endregion

        #region 方法

        public int[] GetQiXueSlotIndexes()
        {
            var indexes = Data.Select(_ => _.SelectedIndex).ToArray();
            return indexes;
        }

        public int[] GetQiXueSlotOrders()
        {
            var orders = Data.Select(_ => _.Order).ToArray();
            return orders;
        }


        public string[] GetQiXueSlotShortNames()
        {
            var shortNames = Data.Select(_ => _.ShortName).ToArray();
            return shortNames;
        }

        public string GetShortNameDesc()
        {
            var shortNames = GetQiXueSlotShortNames();
            var res = shortNames.Join("-");
            return res;
        }

        public string GetHeaderName()
        {
            int step = 6;
            var nameline = new List<string>();

            var itempnames = (from _ in Data select _.SelectedItem.ItemName).ToArray();

            int skiped = 0;

            while (skiped + step <= NSLOTS)
            {
                var part = itempnames.Skip(skiped).Take(step).ToArray();
                var namepart = part.Join(" - ");
                nameline.Add(namepart);
                skiped += step;
            }

            var res = nameline.Join("\n");
            return res;
        }

        protected override void _Update()
        {
            GetSelectedQiXues();
            GetItemNames();
            UpdateHeaderName();
            UpdateModel();
            SendMessage();
        }

        private void GetSelectedQiXues()
        {
            SelectedQiXueSkills = Data.Select(_ => _.SelectedItem).ToArray();
        }

        protected void SendMessage()
        {
            // 发送消息，奇穴改变
            StaticMessager.Send(StaticMessager.QiXueChangedMsg);
        }

        protected void GetItemNames()
        {
            for (int i = 0; i < NSLOTS; i++)
            {
                ItemNames[i] = Data[i].SelectedItem.ItemName;
            }
        }

        protected void UpdateHeaderName()
        {
            ShortNameDesc = GetShortNameDesc();
            Orders = GetQiXueSlotOrders();
            HeaderName = GetHeaderName();
        }

        protected void UpdateModel()
        {
            Model.UpdateInput(this);
        }



        public int[] Export()
        {
            return Orders;
        }

        protected void _Load(IList<int> orders)
        {
            for (int i = 0; i < StaticConst.NumberOfQiXue; i++)
            {
                Data[i].LoadOrder(orders[i]);
            }
        }

        protected void _Load(QiXueConfig config)
        {
            _Load(config.Code);
        }

        public void Load(IList<int> orders)
        {
            TryActionUpdateOnce(_Load, orders);
        }

        public void Load(QiXueConfig config)
        {
            TryActionUpdateOnce(_Load, config);
        }

        #endregion
    }
}