using CommunityToolkit.Mvvm.Input;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Class;
using JYCalculator.Src;
using System.Windows;
using JX3CalculatorShared.Views;
using JYCalculator.Src.Class;


namespace JYCalculator.ViewModels 
{
    public class InitInputViewModel : AbsDependViewModel<AbsViewModel>
    {
        #region 成员

        public readonly InitCharacter InitChar;
        public readonly EquipOptionConfigViewModel EquipOption;
        public readonly BigFMConfigViewModel BigFM;
        public RelayCommand ImportJBPanelCmd { get; set; }

        public NamedAttrs BigFMAttrsDesc; // 大附魔提供的属性

        #endregion

        #region 构造

        public InitInputViewModel(InitCharacter initchar, EquipOptionConfigViewModel equip, BigFMConfigViewModel bigFM)
            : base(InputPropertyNameType.None, initchar, equip, bigFM)
        {
            InitChar = initchar;
            EquipOption = equip;
            BigFM = bigFM;

            ImportJBPanelCmd = new RelayCommand(ImportJBPanel);
            PostConstructor();
            _Update();
        }

        #endregion

        #region 方法

        public bool ImportJBPanel(string jsontxt)
        {
            var (success, jbPanel) = JBPZPanel.TryImportFromJSON(jsontxt);
            if (success)
            {
                jbPanel.Parse();
                Load(jbPanel.InitInput);
                return success;
            }
            else
            {
                return false;
            }
            
        }

        public void ImportJBPanel()
        {
            bool end = false;
            string jsontxt;

            while (!end)
            {
                ImportJBDialog dialog = new ImportJBDialog("");
                if (dialog.ShowDialog() == true)
                {
                    jsontxt = dialog.Answer;
                    var success = ImportJBPanel(jsontxt);
                    if (success)
                    {
                        end = true;
                    }
                    else
                    {
                        MessageBoxButton buttons = MessageBoxButton.OKCancel;
                        var result = MessageBox.Show("数据格式有误！", "错误", buttons, MessageBoxImage.Warning);
                        if (result == MessageBoxResult.Cancel)
                        {
                            end = true;
                        }
                    }
                }
                else
                {
                    end = true;
                }
            }
        }

        #endregion

        protected override void _Update()
        {
            bool old = _AutoUpdate;
            DisableAutoUpdate();
            ConnectedBigFM();
            GetBigFMNamedSAttrs();
            _AutoUpdate = old;
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


        protected override void _Load<TSave>(TSave sav)
        {
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