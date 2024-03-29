﻿using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Class;
using JYCalculator.Src.Data;
using JYCalculator.Src.DB;
using System.Collections.Generic;
using System.Collections.Immutable;
using JX3CalculatorShared.ViewModels;


namespace JYCalculator.ViewModels 
{
    public class AllBuffConfigViewModel : AllBuffConfigViewModelBase
    {
        #region 成员

        #endregion

        #region 构造

        public AllBuffConfigViewModel(BuffDB db): base()
        {
            Buff_Self = new BuffConfigViewModel(db.Buff_Self);
            Buff_Normal = new BuffConfigViewModel(db.Buff_Normal);
            Buff_Banquet = new BuffConfigViewModel(db.Buff_Banquet);
            DeBuff_Normal = new BuffConfigViewModel(db.DeBuff_Normal);
            Buff_Extra = new BuffConfigViewModel(db.Buff_Extra);
            Buff_ExtraStack = new BuffConfigViewModel(db.Buff_ExtraStack);

            var data = new BuffConfigViewModel[]
            {
                Buff_Self, Buff_Normal, Buff_Banquet,
                DeBuff_Normal,
                Buff_Extra, Buff_ExtraStack
            };
            Data = data.ToImmutableArray();

            EmitedBuffGroupsDict = new Dictionary<BuffTypeEnum, BaseBuffGroup>(8);

            AttachDependVMsOutputChanged();
            PostConstructor();
            _Update();
        }


        public AllBuffConfigViewModel() : this(StaticJYData.DB.Buff)
        {
        }

        #endregion


        #region 方法



        protected override void _Update()
        {
            UpdateEmitedBuffGroups();
            UpdateNamedAttrs();
            UpdateSpecial();
        }

        protected override void _DEBUG()
        {
        }

        #endregion

        #region 导入导出

        #endregion
    }



}