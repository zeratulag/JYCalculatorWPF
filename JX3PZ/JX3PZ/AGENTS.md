# JX3PZ — 配装面板模块

**父文档：** [../AGENTS.md](../AGENTS.md) —— 项目级约定、反模式见父文档。

## 概述
配装（PZ）子系统。处理装备选择、面板属性计算、五行石/精炼/镶嵌配置。通过 `.shproj` 共享项目**直接编译进** `JYCalculator.exe`，无独立 DLL。

## 目录结构
```
JX3PZ/
├── Globals/              # 常量（PzConst）、枚举（PzEnums）、全局上下文
├── Messages/             # CommunityToolkit.Mvvm 消息
├── Models/
│   ├── AttributeEntry/   # 属性条目集合
│   ├── CharacterPanel/   # 角色面板（攻击/会心/破防等各维度）
│   ├── Equip/            # 装备等级分数、五行石/精炼模型
│   ├── PanelAttributeSlots/  # 16 种面板属性槽位（每个属性一个 slot）
│   ├── Set/              # 套装效果模型
│   ├── PzMainSav.cs      # 配装方案存档
│   └── PzOneKeyEnhanceModel.cs  # 一键强化
├── Resource/             # 样式 XAML（PzApp.xaml 等 5 个）
├── Src/
│   ├── Class/            # 装备类（Armor, WeaponBase），配装集合属性
│   ├── Data/             # 五行石/镶嵌数据提供者
│   └── StaticPzData.cs   # 静态配装数据入口
├── ViewModels/
│   ├── AttributeEntry/   # 属性条目 VM
│   ├── EquipShow/        # 装备展示 VM（EquipShowBox, EquipShowSet 等）
│   ├── PanelAttributeSlots/  # 面板属性槽位 VM
│   └── PzTab/            # 配装页签 VM（EquipSelect, EquipEnhance 等）
└── Views/
    ├── UserControls/
    │   ├── EquipShow/    # 装备展示控件
    │   ├── Items/        # 小部件（ComboBox, TextBlock, Slider 等）
    │   ├── PzEquip/      # 配装装备控件
    │   └── PzOverview/   # 配装概览控件
    └── PzMainWindow.xaml # 配装主窗口
```

## 功能定位速查

| 任务 | 位置 | 备注 |
|------|------|------|
| 配装主窗口 | `Views/PzMainWindow.xaml` + `ViewModels/PzMainWindowViewModels.cs` | 配装器主入口 |
| 装备筛选/选择 | `ViewModels/PzTab/EquipSelectViewModel.cs` | 按部位/品级/来源筛选 |
| 面板属性计算 | `Models/CharacterPanel/` | `CharacterPanel.cs` 聚合各子面板 |
| 属性槽位系统 | `Models/PanelAttributeSlots/` | 16 个 slot（攻击/会心/破防/无双/加速等） |
| 五行石/镶嵌 | `Src/Data/DiamondDatabase.cs` + `Models/Equip/EquipStoneModel.cs` | 默认等级 `PzConst.DEFAULT_DIAMON_LEVEL` |
| 精炼系统 | `Models/Equip/EquipStrengthModel.cs` | 最高 8 级 `MAX_STRENGTH_LEVEL` |
| 套装效果 | `Models/Set/PzSetModel.cs` + `SetCountItem.cs` | 门派套装属性组合 |
| 一键强化 | `Models/PzOneKeyEnhanceModel.cs` | 自动填充最优五行石 |
| 方案存档/加载 | `Models/PzMainSav.cs` | JSON 序列化 |
| 全局上下文 | `Globals/GlobalViewModels.cs` → `GlobalContext` | 跨项目 View/VM 注册表 |

## 本项目特有约定

- **命名空间**: `JX3PZ.*`（与目录结构一致）
- **`GlobalContext`**: 尽管定义在 `JX3PZ.Globals`，但命名空间为 `JYCalculator.Globals`——直接引用 JYCalculator 类型（`MainWindow`、`MainWindowViewModels`）
- **`PzGlobalContext`**: 命名空间 `JX3PZ.Globals`——仅引用 JX3PZ 内部类型
- **属性槽位模式**: 每个面板属性维度（攻击/会心/破防等）对应一个 `Panel*Slot` 类，通过 `IPanelAttributeSlot` 接口统一
- **装备等级筛选**: `PzConst.MIN_EQUIP_LEVEL`(19000) ~ `MAX_EQUIP_LEVEL`(40000)，默认 27000~36000
- **样式资源**: 5 个 XAML 资源字典通过 `PzApp.xaml` 加载，合并到主 `App.xaml` 中

## 反模式

- **全局上下文中的跨项目耦合**——`GlobalViewModels.cs` 同时引用 `JYCalculator.Views` 和 `JYCalculator.ViewModels`，配装子系统不能独立于主项目存在
- **`GlobalContext` 作为服务定位器**——`GlobalContext.ViewModels` / `.Views` 保存对所有 View 和 ViewModel 的全局静态引用，替代 DI
- **`PzGlobalContext` vs `GlobalContext`**——两个上下文类，命名空间不同（`JX3PZ.Globals` vs `JYCalculator.Globals`），易混淆
- **硬编码版本系数**——`Globals.cs:94` → `ShanHaiDiamondCoef = 37400.0 / 27800.0`，每次资料片需手动修改
- **`ReSharper disable`**——`PzEnums.cs` 因中文枚举成员名需要 `// ReSharper disable InconsistentNaming`
- **残存的已注释代码**——`EquipSelectViewModel.cs:75` 有一行被注释掉的筛选逻辑，标注"暂时不支持"

## 关键数据流

```
用户选择装备
  → EquipSelectViewModel 筛选
  → EquipStoneModel + EquipStrengthModel 计算五行石/精炼
  → CharacterPanel 聚合各 PanelAttributeSlot
  → 最终属性值输出到主窗口
```
