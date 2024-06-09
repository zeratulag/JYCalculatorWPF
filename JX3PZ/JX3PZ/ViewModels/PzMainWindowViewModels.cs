using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Messages;
using JX3CalculatorShared.ViewModels;
using JX3PZ.Globals;
using JX3PZ.Messages;
using JX3PZ.Models;
using JYCalculator.Globals;
using JYCalculator.Messages;
using System;
using System.Collections.Immutable;
using System.Linq;
using CalcResultViewModel = JX3CalculatorShared.ViewModels.CalcResultViewModel;

namespace JX3PZ.ViewModels
{
    public class PzMainWindowViewModels : CollectionViewModel<PzTabItemViewModel>,
        IRecipient<PzChangedMessage>, IRecipient<CalcResultMessage>, IRecipient<PzInfoMessage>
    {
        public readonly PzPlanModel Model;

        public readonly ImmutableDictionary<int, PzTabItemViewModel> Dict;

        public PzTabItemViewModel PrimaryWeaponVM => Data[11]; // 主武器的VM
        public PzResultViewModel PzResultVM { get; }
        public CalcResultViewModel CalcResultVM { get; }
        public CalcInputViewModel CalcInputVM { get; }

        public bool _RecievePzTabMessage { get; private set; } = false;// 是否接收来自PzTab的消息
        public string FinalDPStxt { get; private set; }
        public string FinalDPStxtF;
        public string ProfitOrderDesc { get; private set; }

        public string Title { get; set; } = "配装方案";
        public string Author { get; set; } = "配装器用户";

        private bool CanOpenOverView { get; set; } = false; // 是否可以预览

        public RelayCommand OpenImportJBBBDialogCmd { get; }
        public RelayCommand OpenLoadPZCmd { get; }
        public RelayCommand SavePZCmd { get; }
        public RelayCommand<string> OneKeyEmbedCmd { get; }
        public RelayCommand OneKeyStrengthCmd { get; }
        public RelayCommand<string> OneKeyEnchantCmd { get; }
        public RelayCommand OpenOverviewCmd { get; }
        public RelayCommand SaveOverviewImageCmd { get; }

        public CalcResult CCalcResult;

        public PzMainWindowViewModels()
        {
            Data = (from _ in Enumerable.Range(0, PzConst.POSITIONS) select new PzTabItemViewModel(_))
                .ToImmutableArray();
            Dict = Data.ToImmutableDictionary(_ => _.Position, _ => _);
            Model = new PzPlanModel(Data);
            PzResultVM = new PzResultViewModel();
            CalcResultVM = new CalcResultViewModel();
            CalcInputVM = new CalcInputViewModel();

            // 命令
            OpenImportJBBBDialogCmd = new RelayCommand(OpenImportJBBBDialog);
            OneKeyEmbedCmd = new RelayCommand<string>(OneKeyEmbed);
            OneKeyStrengthCmd = new RelayCommand(OneKeyStrength);
            OneKeyEnchantCmd = new RelayCommand<string>(OneKeyEnchant, (_) => CCalcResult != null);
            OpenLoadPZCmd = new RelayCommand(OpenLoadPZ);
            SavePZCmd = new RelayCommand(SavePZ);
            OpenOverviewCmd = new RelayCommand(OpenOverview, () => CanOpenOverView);
            SaveOverviewImageCmd = new RelayCommand(SaveOverviewImage, () => CanOpenOverView);


            // 注册信息接收处理
            _RecievePzTabMessage = true;
            WeakReferenceMessenger.Default.Register<PzChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<PzInfoMessage>(this);
            WeakReferenceMessenger.Default.Register<CalcResultMessage>(this);
            PzGlobalContext.ViewModels.PzMain = this;


            _SendMessage = false;
            UpdateAndRefresh();
            CalcSet();
            AfterUpdate();
            _SendMessage = true;
        }

        private void SaveOverviewImage()
        {
            PzGlobalContext.Views.PzOverview.CaptureOverviewImg();
        }

        private void OpenOverview()
        {
            PzGlobalContext.Views.PzMain.ShowOverview();
        }

        public PzMainSav Export()
        {
            var res = Model.Export();
            res.Author = Author;
            return res;
        }

        private void SavePZ()
        {
            GlobalContext.ViewModels.Main.SaveCurrent();
        }

        public void OpenLoadPZ()
        {
            var res = GlobalContext.ViewModels.Main.OpenFileAsSav(false);
            if (res != null) Load(res.PzSav);
        }

        // 读取文件
        public void ReadFile(string filepath)
        {
            GlobalContext.ViewModels.Main.ReadFile(filepath);
        }

        #region 一键操作
        public void OneKeyAction(Action act)
        {
            // 一键操作系列
            _RecievePzTabMessage = false;
            _SendMessage = false;
            ActionUpdateOnce(act);
            CalcSet();
            _RecievePzTabMessage = true;
            _SendMessage = true;
            AfterUpdate();
            SendPzMessage();
        }

        public void OneKeyAction<T>(Action<T> act, T param)
        {
            OneKeyAction(() => act(param));
        }
        #endregion

        public void OneKeyEmbed(string l = "6")
        {
            if (int.TryParse(l, out var level))
            {
                OneKeyEmbed(level);
            }
        }

        private void _OneKeyEmbed(int level = 6)
        {
            if (level >= 0 && level <= PzConst.MAX_DIAMOND_LEVEL)
            {
                foreach (var _ in Data)
                {
                    _.EquipDiamondVM.OneKeyEmbed(level);
                }
            }
        }

        public void OneKeyEmbed(int level = 6) => OneKeyAction(_OneKeyEmbed, level);


        private void _OneKeyStrength()
        {
            Data.ForEach(_ => _.EquipEnhanceVM.StrengthToMaxLevel());
        }

        public void OneKeyStrength() => OneKeyAction(_OneKeyStrength);

        public void OneKeyEnchant(string p = "0") => OneKeyAction(_OneKeyEnchant, p);

        private void _OneKeyEnchant(string p = "0")
        {
            if (p == "0")
            {
                _OneKeyEnhance();
            }
            else
            {
                _OneKeyBigFM();
            }
        }

        private void _OneKeyEnhance()
        { // 一键小附魔
            var model = new PzOneKeyEnhanceModel(this);
            model.Calc();
            OneKeyAction(_LoadEnhance, model.FinalIdx);
        }

        /// <summary>
        /// 批量更换附魔（基于index）
        /// </summary>
        /// <param name="d"></param>
        private void _LoadEnhance(int[] d)
        {
            for (int i = 0; i < d.Length; i++)
            {
                Data[i].EquipEnhanceVM.SelectedEnhanceIndex = d[i];
            }
        }

        private void _OneKeyBigFM()
        {
            Data.ForEach(_ => _.EquipEnhanceVM.OneKeyBigFM());
        }

        protected override void _Update()
        {
            // 当装备未发生改变时，无须更新套装
            Model.UpdateFrom(Data);
            Model.Calc();
            UpdateStoneActive();
            UpdateShowBoxs();
        }

        public void UpdatePzResult()
        {
            PzResultVM.UpdateFrom(Model);
        }

        public void UpdatePzHeader()
        {
            Data.ForEach(_ => _.UpdatePzSummary());
        }

        public void UpdateStoneActive()
        {
            // 更新五彩石激活情况
            PrimaryWeaponVM.EquipShowVM.Stone.UpdateDiamond(Model.DiamondCount, Model.DiamondIntensity);
            PrimaryWeaponVM.EquipEmbedVM.UpdateDiamond(Model.DiamondCount, Model.DiamondIntensity);
        }

        public void CalcSet()
        {
            Model.CalcSet();
            UpdateSetVMs();
            UpdateShowBoxs();
        }

        public void UpdateSetVMs()
        {
            // 更新套装信息
            foreach (var vm in Data)
            {
                var pos = vm.Position;
                var setID = Model.SetModel.Position2SetID[pos];
                if (setID > 0)
                {
                    var setVM = Model.SetViewModels[setID];
                    vm.EquipShowSetVM = setVM;
                }
                else
                {
                    vm.EquipShowSetVM = EquipShowSetViewModel.Empty;
                }
            }
        }

        public void UpdateShowBoxs()
        {
            Dict.Values.ForEach(_ => _.EquipShowVM.UpdateShowBox());
        }

        private void _Load(JBPZEquipSnapshotCollection equipList)
        {
            _LoadEquipSnapshotCollection(equipList);
        }

        private void _LoadEquipSnapshotCollection(JBPZEquipSnapshotCollection jc)
        {
            if (jc.Dict == null) jc.GetDict();

            foreach (var kvp in Dict)
            {
                var slot = (EquipSlotEnum)kvp.Key;
                var slotStr = slot.ToString();
                var snap = jc.Dict[slotStr];
                kvp.Value.Load(snap);
            }
        }

        public void Load(JBBB j)
        {
            DisableAutoUpdate();
            _RecievePzTabMessage = false;
            _SendMessage = false;
            if (j.Title != null) Title = j.Title;
            _Load(j.EquipList);
            _Update();
            CalcSet();
            AfterUpdate();
            SendPzMessage();
            EnableAutoUpdate();
            _RecievePzTabMessage = true;
            _SendMessage = true;
        }

        public void Load(PzMainSav save)
        {
            if (save == null) return;
            if (save.Author != null) Author = save.Author;
            var j = save.GetJBBB();
            Load(j);
        }

        private void OpenImportJBBBDialog()
        {
            var (success, j) = ImportJX3BOXViewModel.TryReadFromDialog();
            if (success)
            {
                Load(j);
            }
        }

        protected override void _RefreshCommands()
        {
        }

        public void Receive(PzChangedMessage message)
        {
            if (!_RecievePzTabMessage) return;

            _Update();
            if (message.ChangedType == PzChangedEnum.Equip)
            {
                CalcSet();
            }
            AfterUpdate();
            if (_SendMessage) SendPzMessage();
        }

        public void AfterUpdate()
        {
            Model.GetTotalEntryCollection();
            Model.GetCharacterPanel();
            Model.XFPanel.Name = Title;
            UpdatePzResult();
            UpdatePzHeader();
        }


        private void SendPzMessage()
        {
            WeakReferenceMessenger.Default.Send(new PzPlanMessage(Title, Author, Model));
            CanOpenOverView = true;
            OpenOverviewCmd.NotifyCanExecuteChanged();
            SaveOverviewImageCmd.NotifyCanExecuteChanged();
        }

        public void Receive(CalcResultMessage message)
        {
            if (GlobalContext.IsPZSyncWithCalc)
            {
                UpdateCalcResult(message);
                OneKeyEnchantCmd.NotifyCanExecuteChanged();
            }
        }

        public void UpdateCalcResult(CalcResultMessage message)
        {
            CalcResultVM.UpdateFrom(message);
            CalcInputVM.UpdateFrom(message);
            CCalcResult = message.Result;
        }

        public PzEquipSummaryViewModel[] GetEquipSummaryViewModels()
        {
            var res = Data.Select(_ => _.Summary).ToArray();
            return res;
        }

        public void Receive(PzInfoMessage message)
        {
            UpdateTitle(message.Title);
            Author = message.Author;
        }

        public void UpdateTitle(string title)
        {
            Title = title;
            Model.XFPanel.Name = title;
        }
    }
}