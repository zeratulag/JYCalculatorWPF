using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JX3BOX.JX3.Interact.ClientHelper;
using JX3BOX.JX3.Interact.ClientHelper.Enums;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Messages;
using JX3CalculatorShared.Models;
using JX3CalculatorShared.Utils.DbUtils;
using JX3CalculatorShared.Views;
using Microsoft.Win32;
using MingYiTools;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace JX3CalculatorShared.ViewModels
{
    public class ImportDataMingYiDatabaseViewModel : ObservableObject
    {
        public static string LastConfirmedFolder { get; private set; } = null;
        public static string LastConfirmedMYEquipStatFile { get; private set; } = null;

        public const string MYEquipStatFile =
            @"Interface\MY#DATA\!all-users@zhcn_hd\userdata\role_statistics\equip_stat.v4.db";

        public string EquipStatDbPath { get; private set; } = null;
        public DatabaseConnection EquipStatDb { get; private set; } = null;

        public string Text { get; set; }
        public string JX3Folder { get; set; } = null;

        public bool ParseSuccess => SelectedMingYiOwnerSuit != null;

        public MingYiServerModel ServerModel { get; private set; } = null;

        public MingYiServerInfo[] MingYiServerInfoList { get; private set; } = null;

        [AlsoNotifyFor(nameof(CurrentServerMingYiOwnerInfoList))]
        public int SelectedMingYiServerInfoIndex { get; set; }
        public MingYiServerInfo SelectedMingYiServerInfo { get; set; } // 当前选择的服务器信息

        public bool IsDatabaseLoaded { get; set; } = false;
        public bool IsCurrentOwnerEquipLoaded { get; set; } = false;

        public MingYiOwnerInfoEntity[] MingYiOwnerInfoList { get; private set; }
        public MingYiOwnerInfoEntity[] CurrentServerMingYiOwnerInfoList => SelectedMingYiServerInfo?.Items;

        public MingYiOwnerInfoEntity SelectedMingYiOwnerInfoEntity { get; set; }
        public int SelectedMingYiOwnerInfoIndex { get; set; }
        public MingYiOwnerEquipModel CurrentOwnerEquipModel { get; private set; } = null;

        public MingYiOwnerSuitEquipModel SelectedMingYiOwnerSuit { get; set; }
        public int SelectedSuitIndex { get; set; }
        public MingYiOwnerSuitEquipModel[] MingYiOwnerSuitList { get; set; }

        public RelayCommand ImportCmd { get; }
        public RelayCommand SelectFolderCmd { get; }
        public RelayCommand LoadDatabaseCmd { get; }


        public ImportDataMingYiDatabaseViewModel()
        {
            SelectFolderCmd = new RelayCommand(SelectFolder);
            LoadDatabaseCmd = new RelayCommand(LoadDatabase, () => EquipStatDbPath != null);
            ImportCmd = new RelayCommand(Import, () => ParseSuccess);
            if (LastConfirmedFolder != null)
            {
                JX3Folder = LastConfirmedFolder;
            }

            if (LastConfirmedMYEquipStatFile != null)
            {
                FindLocalClients();
                EquipStatDbPath = LastConfirmedMYEquipStatFile;
                LoadDatabase();
            }

            else
            {
                FindLocalClients();
            }
        }

        public void OnJX3FolderChanged()
        {
            FindEquipStatFile();
        }

        public void FindLocalClients()
        {
            var clientInfos = JX3PathHelper.GetLocalClients();
            var clientInfo_zhcn = clientInfos.Where(e => e.Type == ClientType.zhcn_hd).ToArray();
            if (clientInfo_zhcn.Length == 1)
            {
                var res = clientInfo_zhcn[0];
                JX3Folder = Path.GetFullPath(res.WorkingDirectory); // JX3\bin\zhcn_hd
            }
        }

        private void SelectFolder()
        {
            var res = ViewTool.OpenSelectFolder(JX3Folder);
            if (res != null)
            {
                JX3Folder = res;
                LastConfirmedFolder = res;
            }
        }

        public void LoadDatabase()
        {
            bool isCtrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            if (isCtrlPressed)
            {
                // Ctrl+点击：弹出文件选择对话框
                // 如果用户取消选择，直接返回，不执行后续加载
                if (!ManuallyChooseDatabasePath())
                {
                    return;
                }
                // 用户已确认选择，EquipStatDbPath 已更新，继续执行下方加载逻辑
            }
            else if (string.IsNullOrEmpty(EquipStatDbPath))
            {
                // 普通点击但没有默认路径，不执行
                return;
            }

            LastConfirmedMYEquipStatFile = EquipStatDbPath;
            // 执行核心的数据库加载逻辑（无论手动选择还是默认路径）
            Task.Run(ExecuteDatabaseLoading);
        }

        public void ExecuteDatabaseLoading()
        {
            if (EquipStatDbPath == null) return;
            EquipStatDb = new DatabaseConnection(EquipStatDbPath);
            EquipStatDb.Connect();
            var res = EquipStatDb.GetAll<MingYiOwnerInfo>();
            var itemRes = res.Select(e => new MingYiOwnerInfoEntity(e)).ToArray();
            MingYiOwnerInfoList = itemRes.Where(e => e.OwnerLevel == StaticConst.CurrentLevel && e.OwnerForce == 7)
                .OrderBy(e => e.ServerName).ThenBy(e => e.OwnerKey).ToArray();
            foreach (var e in itemRes)
            {
                e.Parse();
            }
            ServerModel = new MingYiServerModel(MingYiOwnerInfoList);
            ServerModel.Calc();
            MingYiServerInfoList = ServerModel.MingYiServerInfoList;
            SelectedMingYiServerInfoIndex = 0;
            SelectedMingYiOwnerInfoIndex = 0;
            IsDatabaseLoaded = true;
            LoadSelectedOwnerEquipItems();
            UpdateCurrentOwnerEquip();
        }


        public void OnSelectedMingYiServerInfoChanged()
        {
            SelectedMingYiOwnerInfoIndex = 0;
        }

        public void OnSelectedMingYiOwnerInfoEntityChanged()
        {
            LoadSelectedOwnerEquipItems();
            UpdateCurrentOwnerEquip();
        }

        /// <summary>
        /// 手动选择数据库文件路径
        /// </summary>
        /// <returns>是否成功选择了文件</returns>
        public bool ManuallyChooseDatabasePath()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "数据库文件 (*.db)|*.db|所有文件 (*.*)|*.*",
                Title = "选择茗伊装备数据库文件（equip_stat.v4.db）",
                InitialDirectory = !string.IsNullOrEmpty(JX3Folder)
                    ? JX3Folder
                    : Environment.GetFolderPath(Environment.SpecialFolder.MyComputer)
            };

            if (dialog.ShowDialog() != true) return false;
            EquipStatDbPath = dialog.FileName;
            // 通知属性变更（如果有其他地方依赖此属性）
            return true;
        }

        public void LoadSelectedOwnerEquipItems()
        {
            var equipItems = EquipStatDb.Query<MingYiEquipItem>(e => e.OwnerKey == SelectedMingYiOwnerInfoEntity.OwnerKey)
                .ToArray();
            var equipItemEntities = equipItems.Select(e => new MingYiEquipItemEntity(e)).ToArray();
            equipItemEntities.ForEach(e => e.Parse());
            CurrentOwnerEquipModel = new MingYiOwnerEquipModel(SelectedMingYiOwnerInfoEntity, equipItemEntities);
        }

        public void UpdateCurrentOwnerEquip()
        {
            CurrentOwnerEquipModel.Calc();
            MingYiOwnerSuitList = CurrentOwnerEquipModel.SuitEquipModels.Values.ToArray();
            SelectedSuitIndex = 0;
            IsCurrentOwnerEquipLoaded = true;

            var dispatcher = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(() => ImportCmd.NotifyCanExecuteChanged());
            }
            else
            {
                ImportCmd.NotifyCanExecuteChanged();
            }
        }

        public void OnSelectedMingYiOwnerSuitChanged()
        {
            ImportCmd.NotifyCanExecuteChanged();
        }

        public void FindEquipStatFile()
        {
            if (JX3Folder != null)
            {
                var MYEquipStatFilePath = Path.Combine(JX3Folder, MYEquipStatFile);
                MYEquipStatFilePath = Path.GetFullPath(MYEquipStatFilePath);
                if (File.Exists(MYEquipStatFilePath))
                {
                    EquipStatDbPath = MYEquipStatFilePath;
                }
                else
                {
                    EquipStatDbPath = null;
                }
            }

            LoadDatabaseCmd.NotifyCanExecuteChanged();
        }


        public void Import()
        {
            var msg = new ImportDataMessage(ImportDataType.MingYiDBPZ, SelectedMingYiOwnerSuit);
            WeakReferenceMessenger.Default.Send(msg);
        }
    }
}