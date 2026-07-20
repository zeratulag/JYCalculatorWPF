# JX3CalculatorShared — 共享基础设施

**父文档：** [../AGENTS.md](../AGENTS.md) —— 项目级约定、反模式见父文档。

## 概述
跨项目共享基类、数据库层、公共 View/ViewModel、工具类。通过 `.shproj` 共享项目**直接编译进** `JYCalculator.exe`，无独立 DLL。提供项目间复用但不产生程序集边界的 "源码级共享"。

## 目录结构
```
JX3CalculatorShared/
├── Common/               # Annotations.cs（ReSharper 注解）
├── Globals/              # GlobalEnums.cs、共享常量（AppStatic、StaticConst）
├── Messages/             # CommunityToolkit.Mvvm 消息（CalcMessage、StaticMessager）
├── Models/
│   ├── QiXueConfigModel.cs       # 奇穴配置模型
│   ├── MiJiConfigModel.cs        # 秘籍配置模型
│   └── SkillInfoItemModel.cs     # 技能信息项
├── Src/
│   ├── CalculatorShells/  # 计算器外壳基类（AbsCalculatorShell）
│   ├── Class/             # 共享领域类（55 文件）
│   │   ├── BaseBuff.cs         # Buff 基类
│   │   ├── BuffTrigger.cs      # Buff 触发机制
│   │   ├── EquipOption.cs      # 装备特效选项
│   │   ├── Target.cs           # 目标模型（等级/Buff）
│   │   └── SkillDataBase.cs    # 技能数据基类
│   ├── Data/              # 数据加载器基类、CalcSetting 基类、物品类
│   ├── DB/                # 数据库基类（BuffDBBase、ItemDTDBBase 等）
│   ├── DPSKernelOptimizer/  # 优化器基类
│   └── MingYiTools/       # 茗伊插件数据导入工具（ImportTool 等）
├── Utils/
│   ├── DpsUtils.cs        # DPS 计算工具
│   ├── ExtensionTool.cs   # 通用扩展方法
│   ├── FuncTool.cs        # 函数工具（RunTime 计时等）
│   ├── GlobalTool.cs      # 全局工具
│   │── LuaUtils/          # Lua 表解析工具（JX3 数据导出格式）
│   └── PgTool.cs          # PGK 扩展辅助
├── ViewModels/
│   ├── Common/AbsViewModel.cs  # MVVM 基类（消息注册、_DEBUG 钩子）
│   ├── MainWindow/             # 主窗口 VM 部件（InitInput、Buff、FightOption 等）
│   └── Commands.cs             # 共享命令
└── Views/
    ├── Resources/               # 11 个 XAML 资源字典（Styles.xaml、Converters.xaml 等）
    ├── UserControls/BuffConfig  # Buff 配置控件
    ├── UserControls/Items/      # 可复用 UI 部件
    └── Dialogs/                 # 对话框（AboutDialog 等）
```

## 功能定位速查

| 任务 | 位置 | 备注 |
|------|------|------|
| MVVM 基类 | `ViewModels/Common/AbsViewModel.cs` | `ObservableObject` 子类，消息注册/取消注册、`_DEBUG()` 钩子 |
| Buff 系统 | `Src/Class/BaseBuff.cs` + `BuffTrigger.cs` | 所有 Buff 类型的基类，触发条件建模 |
| 技能数据基类 | `Src/Class/SkillDataBase.cs` | 技能系数、破招、CD 等基础属性 |
| 目标模型 | `Src/Class/Target.cs` | 等级、防御、Buff 计算 |
| 数据库基类 | `Src/DB/` | `BuffDBBase`、`ItemDTDBBase`、`QiXueDBBase`、`ZhenFaDBBase`、`BigFMDBBase` |
| 数据加载 | `Src/Data/DataLoader.cs` | MiniExcel 读取 Excel → 结构化数据 |
| 计算设置 | `Src/Data/CalcSettingBase.cs` | 计算参数（战斗时间/目标/奇穴组合） |
| 共享 XAML 资源 | `Views/Resources/Styles.xaml` | 所有 `Style.*` 键定义 |
| XAML 转换器 | `Views/Resources/Converters.xaml` | 值转换器集合 |
| 茗伊数据导入 | `Src/MingYiTools/ImportTool.cs` | 从游戏插件导出数据导入计算器 |
| 工具扩展 | `Utils/ExtensionTool.cs` | `SafeForEach`、`SafeSelect` 等集合安全操作 |
| 计时工具 | `Utils/FuncTool.cs` | `RunTime(action, name)` 性能计时 |

## 本项目特有约定

- **命名空间**: `JX3CalculatorShared.*`（与目录结构一致）
- **基类模式**: 本项目中定义 `Abs`/`Base` 类，JYCalculator 项目继承并添加领域特定逻辑
- **DB 层模式**: `IDB<TKey, TValue>` 接口 + 具体 `*DBBase` 类 + JYCalculator 中的具体 `*DB` 子类
- **`.projitems` 驱动编译**: 所有源文件通过 `JX3CalculatorShared.projitems` 列出，由 `JYCalculator.csproj` 的 `<Import>` 引入
- **XAML 样式**: 所有共享样式遵循 `Style.<Type><Variant>` 命名（如 `Style.CenterComboBox`、`Style.ReadOnlyDataGrid`）
- **`#region` 使用中文**: `#region 成员`、`#region 构造`、`#region 方法`

## 反模式

- **`AbsViewModel._DEBUG()` 钩子**——`#if DEBUG` 下的虚方法，6+ 子类覆写，调试代码嵌入生产类层次
- **`ZhenFaDBBase.cs:22` 用 `XinFaTag == "JY"` 分支**——共享项目中的心法特定逻辑，破坏共享抽象
- **`BaseBuff.cs:85-93` 大段注释代码**——`ParseAbsBuffItem()` 方法 9 行被注释
- **`LuaUtils.cs:63-132` 70 行注释掉的替代实现**——`ParseLuaTableString1()` + `ConvertDynValue()`
- **`ImportTool.cs:88` `[TODO]：完善算法`**——茗伊导入算法未完成
- **`EquipOption.cs:270` `TODO: 新增更多的描述`**——装备特效描述不完整
- **`Annotations.cs` 全文件禁用 XML 文档警告**——`pragma warning disable 1591` + 7 个 ReSharper 抑制
- **`Converter.cs` 13 个 `ConvertBack` 抛出 `NotImplementedException`**——所有值转换器仅支持单向绑定
- **`AbsViewModel` 自定义 `InputChanged`/`OutputChanged` 事件级联**——非标准 `INotifyPropertyChanged`，手动双事件传播系统
- **Fody `[DoNotNotify]` 与 CommunityToolkit `ObservableObject` 混用**——两套属性变更通知机制并存
- **`Commands.cs` 定义 `RoutedUICommand`**——与主项目的 `RelayCommand`（CommunityToolkit）模式不一致

## 建议首先阅读的文件

1. `ViewModels/Common/AbsViewModel.cs` —— MVVM 基类，理解消息和生命周期
2. `Src/Class/BaseBuff.cs` —— Buff 系统核心基类
3. `Src/Data/DataLoader.cs` —— 数据加载，理解 JX3 数据如何进入计算器
4. `Views/Resources/Styles.xaml` —— 所有共享 UI 样式
5. `Globals/GlobalEnums.cs` —— 共享枚举（DamageTypeEnum、QualityEnum 等）
