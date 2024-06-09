using CommunityToolkit.Mvvm.Input;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Class;
using JYCalculator.Globals;


namespace JYCalculator.ViewModels
{
    public class InitInputViewModel : AbsDependViewModel<AbsViewModel>
    {
        #region 成员

        public InitCharacter InitCharVM { get; }
        public EquipOptionConfigViewModel EquipOptionVM { get; }
        public BigFMConfigViewModel BigFMVM { get; }
        public RelayCommand OpenImportJBBBDialogCmd { get; set; }

        public NamedAttrs BigFMAttrsDesc; // 大附魔提供的属性
        public InitCharacter NoneBigFMInitCharacter;

        public string JBPanelTitle = null; // 导入的JB配装标题

        #endregion

        #region 构造

        public InitInputViewModel(InitCharacter initchar, EquipOptionConfigViewModel equip, BigFMConfigViewModel bigFmvm)
            : base(InputPropertyNameType.None, initchar, equip, bigFmvm)
        {
            InitCharVM = initchar;
            EquipOptionVM = equip;
            BigFMVM = bigFmvm;

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
            BigFMVM.BigFMSlotDict[EquipSubTypeEnum.HAT].SetInitCharConnect(InitCharVM.Had_BigFM_hat);
            BigFMVM.BigFMSlotDict[EquipSubTypeEnum.JACKET].SetInitCharConnect(InitCharVM.Had_BigFM_jacket);
        }

        protected void GetBigFMNamedSAttrs()
        {
            BigFMAttrsDesc = BigFMVM.Model.GetNamedSAttrs(InitCharVM.Had_BigFM_hat, InitCharVM.Had_BigFM_jacket);
        }


        protected override void _RefreshCommands()
        {
        }


        public InitInputSav Export()
        {
            var res = new InitInputSav(InitCharVM, EquipOptionVM.Export(), BigFMVM.Config);
            return res;
        }

        protected void _Load(InitInputSav sav)
        {
            InitCharVM.LoadFromIChar(sav.InitChar);
            EquipOptionVM.Load(sav.EquipOption);
            BigFMVM.Load(sav.BigFM);
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
            var res = InitCharVM.Copy();
            if (InitCharVM.Had_BigFM_jacket)
            {
                var jacket = BigFMVM.Model.Jacket;
                res.RemoveSAttrDict(jacket?.SAttrs);
            }

            if (InitCharVM.Had_BigFM_hat)
            {
                var hat = BigFMVM.Model.Hat;
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