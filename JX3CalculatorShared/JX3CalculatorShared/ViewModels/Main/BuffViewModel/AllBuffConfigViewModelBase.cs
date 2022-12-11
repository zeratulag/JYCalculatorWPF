using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Utils;
using System.Collections.Generic;
using System.Linq;

namespace JX3CalculatorShared.ViewModels
{
    public class AllBuffConfigViewModelBase : CollectionViewModel<BuffConfigViewModel>
    {
        public BuffConfigViewModel Buff_Self;
        public BuffConfigViewModel Buff_Normal;
        public BuffConfigViewModel DeBuff_Normal;
        public BuffConfigViewModel Buff_Banquet;
        public BuffConfigViewModel Buff_Extra;
        public BuffConfigViewModel Buff_ExtraStack;
        public Dictionary<BuffTypeEnum, BaseBuffGroup> EmitedBuffGroupsDict;
        public BaseBuffGroup EmitedAllBuffGroup; // 最终汇总的Buff求和
        public BaseBuffGroup EmitedDeBuffGroup; // Debuff求和
        public Dictionary<BuffTypeEnum, NamedAttrs> BuffAttrsDescDict;

        public NamedAttrs BuffAttrsDesc; // 所有Buff求和的最终效果
        public NamedAttrs DebuffAttrsDesc;

        public BuffSpecialArg Arg; // 特殊Buff参数传递


        public AllBuffConfigViewModelBase(): base()
        {
            Arg = new BuffSpecialArg(0.0);
        }


        public AllBuffConfigSav Export()
        {
            var res = new AllBuffConfigSav()
            {
                Buff_Self = Buff_Self.Export(),
                Buff_Normal = Buff_Normal.Export(),
                DeBuff_Normal = DeBuff_Normal.Export(),
                Buff_Banquet = Buff_Banquet.Export(),
                Buff_Extra = Buff_Extra.Export(),
                Buff_ExtraStack = Buff_ExtraStack.Export()
            };
            return res;
        }

        public void Load(AllBuffConfigSav sav)
        {
            ActionUpdateOnce(_Load, sav);
        }

        protected void UpdateEmitedBuffGroups()
        {
            EmitedBuffGroupsDict.Clear();
            EmitedBuffGroupsDict.Add(BuffTypeEnum.Buff_Self, Buff_Self.EmitedBaseBuffGroup);
            EmitedBuffGroupsDict.Add(BuffTypeEnum.Buff_Normal, Buff_Normal.EmitedBaseBuffGroup);
            EmitedBuffGroupsDict.Add(BuffTypeEnum.Buff_Banquet, Buff_Banquet.EmitedBaseBuffGroup);
            EmitedBuffGroupsDict.Add(BuffTypeEnum.Buff_Extra, Buff_Extra.EmitedBaseBuffGroup);
            EmitedBuffGroupsDict.Add(BuffTypeEnum.Buff_ExtraStack, Buff_ExtraStack.EmitedBaseBuffGroup);

            EmitedAllBuffGroup = EmitedBuffGroupsDict.Values.Sum();

            EmitedDeBuffGroup = DeBuff_Normal.EmitedBaseBuffGroup;
        }

        protected void UpdateNamedAttrs()
        {
            BuffAttrsDescDict = EmitedBuffGroupsDict.ToDictionary(_ => _.Key, _ => _.Value?.ToNamedAttrs());
            BuffAttrsDesc = EmitedAllBuffGroup.ToNamedAttrs();
            BuffAttrsDesc.ParcelName("BUFF：");

            DebuffAttrsDesc = EmitedDeBuffGroup.ToNamedAttrs();
            DebuffAttrsDesc.ParcelName("DEBUFF：");
        }

        protected void _Load(AllBuffConfigSav sav)
        {
            Buff_Self.Load(sav.Buff_Self);
            Buff_Normal.Load(sav.Buff_Normal);
            DeBuff_Normal.Load(sav.DeBuff_Normal);
            Buff_Banquet.Load(sav.Buff_Banquet);
            Buff_Extra.Load(sav.Buff_Extra);
            Buff_ExtraStack.Load(sav.Buff_ExtraStack);
        }

        // 更新特殊BUFF
        protected void UpdateSpecial()
        {
            var res = Buff_Extra.GetPiaoHuangCover();
            Arg.PiaoHuangCover = res;
        }

    }

    public class AllBuffConfigSav
    {
        public Dictionary<string, BuffVMSav> Buff_Self;
        public Dictionary<string, BuffVMSav> Buff_Normal;
        public Dictionary<string, BuffVMSav> DeBuff_Normal;
        public Dictionary<string, BuffVMSav> Buff_Banquet;
        public Dictionary<string, BuffVMSav> Buff_Extra;
        public Dictionary<string, BuffVMSav> Buff_ExtraStack;
    }

    // 特殊BUFF参数传递
    public struct BuffSpecialArg
    {
        public double PiaoHuangCover;

        public BuffSpecialArg(double piaoHuangCover)
        {
            PiaoHuangCover = piaoHuangCover;
        }
    }

}

