using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.ViewModels;
using JX3CalculatorShared.Views;
using JYCalculator.Class;
using System.Windows;
using System.Windows.Interop;
using JX3CalculatorShared.Messages;
using JYCalculator.Globals;


namespace JYCalculator.ViewModels
{
    public class InitInputViewModel : AbsDependViewModel<AbsViewModel>
    {
        #region 成员

        public readonly InitCharacter InitChar;
        public readonly EquipOptionConfigViewModel EquipOption;
        public readonly BigFMConfigViewModel BigFM;
        public RelayCommand OpenImportJBBBDialogCmd { get; set; }

        public NamedAttrs BigFMAttrsDesc; // 大附魔提供的属性
        public InitCharacter NoneBigFMInitCharacter;

        public string JBPanelTitle = null; // 导入的JB配装标题

        #endregion

        #region 构造

        public InitInputViewModel(InitCharacter initchar, EquipOptionConfigViewModel equip, BigFMConfigViewModel bigFM)
            : base(InputPropertyNameType.None, initchar, equip, bigFM)
        {
            InitChar = initchar;
            EquipOption = equip;
            BigFM = bigFM;

            OpenImportJBBBDialogCmd = new RelayCommand(OpenImportJBBBDialog);
            NoneBigFMInitCharacter = initchar;
            PostConstructor();
            _Update();
        }

        #endregion

        #region 方法

        public void ImportJBPZPanel(JBBBPZPanel jbPanel)
        {
            // 导入JB配装信息
            Load(jbPanel.InitInput);
            JBPanelTitle = jbPanel.BaseJBBB.Title;
        }

        public void ImportJBBB(JBBB j)
        {
            var res = new JBBBPZPanel(j);
            res.Parse();
            ImportJBPZPanel(res);
        }

        public void OpenImportJBBBDialog()
        {
            var (success, j) = ImportJX3BOXViewModel.TryReadFromDialog();
            if (success)
            {
                var jbPanel = new JBBBPZPanel(j);
                jbPanel.Parse();
                ImportJBPZPanel(jbPanel);
            }
            else
            {
            }
        }

        #endregion

        protected override void _Update()
        {
            bool old = _AutoUpdate;
            DisableAutoUpdate();
            ConnectedBigFM();
            GetBigFMNamedSAttrs();
            GetNoneBigFMInitCharacter();
            _AutoUpdate = old;
            GlobalContext.IsPZSyncWithCalc = false;
        }

        // 如果自身属性已经包括了头和衣大附魔，那么必须选中头和衣大附魔
        protected void ConnectedBigFM()
        {
            BigFM.BigFMSlotDict[EquipSubTypeEnum.HAT].SetInitCharConnect(InitChar.Had_BigFM_hat);
            BigFM.BigFMSlotDict[EquipSubTypeEnum.JACKET].SetInitCharConnect(InitChar.Had_BigFM_jacket);
        }

        protected void GetBigFMNamedSAttrs()
        {
            BigFMAttrsDesc = BigFM.Model.GetNamedSAttrs(InitChar.Had_BigFM_hat, InitChar.Had_BigFM_jacket);
        }


        protected override void _RefreshCommands()
        {
        }


        public InitInputSav Export()
        {
            var res = new InitInputSav(InitChar, EquipOption.Export(), BigFM.Config);
            return res;
        }

        protected void _Load(InitInputSav sav)
        {
            InitChar.LoadFromIChar(sav.InitChar);
            EquipOption.Load(sav.EquipOption);
            BigFM.Load(sav.BigFM);
        }

        public void Load(InitInputSav sav)
        {
            ActionUpdateOnce(_Load, sav);
        }


        /// <summary>
        /// 计算没有大附魔加成下的原始初始面板
        /// </summary>
        /// <returns></returns>
        protected InitCharacter GetNoneBigFMInitCharacter()
        {
            var res = InitChar.Copy();
            if (InitChar.Had_BigFM_jacket)
            {
                var jacket = BigFM.Model.Jacket;
                res.RemoveSAttrDict(jacket?.SAttrs);
            }

            if (InitChar.Had_BigFM_hat)
            {
                var hat = BigFM.Model.Hat;
                res.RemoveSAttrDict(hat?.SAttrs);
            }

            NoneBigFMInitCharacter = res;
            return res;
        }

        public void ClearJBTitle()
        {
            JBPanelTitle = null;
        }
    }

    // 序列化存储
    public class InitInputSav
    {
        public readonly InitCharacter InitChar;
        public readonly EquipOptionConfigSav EquipOption;
        public readonly BigFMSlotConfig[] BigFM;

        public InitInputSav(InitCharacter initChar, EquipOptionConfigSav equipOption, BigFMSlotConfig[] bigFM)
        {
            InitChar = initChar;
            EquipOption = equipOption;
            BigFM = bigFM;
        }
    }
}