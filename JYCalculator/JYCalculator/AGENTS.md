# JYCalculator 项目（主 WPF 应用）

**父文档：** [../AGENTS.md](../AGENTS.md) —— 项目级约定、第三方技术栈及反模式见父文档。

## 概述
主 WPF 可执行项目。包含所有 UI（Views/ViewModels）、DPS 计算核心、配装优化器和领域专属模型。两个共享项目（`JX3CalculatorShared`、`JX3PZ`）均编译到此程序集中。

## 目录结构
```
JYCalculator/
├── App.xaml(.cs)           # 应用定义，自动生成 Main()
├── Views/                  # WPF XAML + 代码后置
│   ├── MainWindow.xaml     # 主窗口（三栏布局）
│   ├── UserControls/       # 可复用控件（SkillInfoGrid、BuffConfig 等）
│   └── Shared/             # 共享 XAML 辅助
├── ViewModels/             # MVVM 视图模型
│   ├── MainWindow/         # 主窗口 VM（最大，通过 partial class 拆分）
│   └── DebugWindow/        # 调试窗口 VM
├── Models/                 # 数据模型
│   ├── EventTriggerModel/  # 事件触发计算
│   ├── SkillNumModel/      # 技能数量模型
│   └── SkillSpecialEffectSolverModels/
├── Src/                    # 业务逻辑核心
│   ├── CalculatorShells/   # 顶层协调器（编排计算流程）
│   ├── Class/              # 领域类（FullCharacter、InitCharacter、SkillData）
│   ├── Data/               # 静态数据提供者 + 物品类
│   ├── DB/                 # 数据库层（IDB<TKey,TValue> 实现）
│   ├── DPSKernelShell/     # 核心 DPS 数学管道
│   └── DPSKernelOptimizer/ # 配装/技能优化
├── Globals/                # 应用常量（XFAppStatic）、枚举
├── Messages/               # CommunityToolkit.Mvvm 消息类型
├── Utils/                  # 工具类
├── Resource/               # 图标（835 个 PNG）、图片、数据文件
├── Properties/             # AssemblyInfo.cs、Annotations.cs（ReSharper）
└── costura-*/              # Costura 嵌入的原生 DLL（e_sqlite3）
```

## 功能定位速查

| 任务 | 位置 | 备注 |
|------|------|------|
| 启动流程 | `App.xaml.cs` | 构造：异常处理 → Serilog → `PreLoad()` → WPF Run |
| 主窗口逻辑 | `ViewModels/MainWindow/Shared/MainWindowViewModels.cs` | 约 800 行，`DoCalc()`、`BindViewModels()`、`MakeCommands()` |
| DPS 计算核心 | `Src/DPSKernelShell/` | `InitCharacter` → `FullCharacter` → `SkillDamage` → `ProfitDF` |
| 配装优化器 | `Src/DPSKernelOptimizer/` | `DPSKernelOptimizer` + `OptimizerShell` |
| 技能/奇穴配置 | `Models/Shared/QiXueConfigModelShared.cs` | 奇穴与秘籍天赋配置 |
| Buff 系统 | `Src/Class/` → `JYBuff`、`BaseBuff`、`BuffTrigger` | 游戏中 Buff 建模 |
| 数据库层 | `Src/DB/` | 具体 DB 类（`BuffDB`、`ItemDTDB` 等） |
| 计算器外壳 | `Src/CalculatorShells/CalculatorShellShared.cs` | 顶层计算编排 |
| 调试/开发工具 | `ViewModels/DebugWindow/`、`Views/MainWindow.xaml.cs` `PreProceed()` | `ConsoleMainProgram.ConsoleMain()` 仅在 DEBUG 中调用 |
| 应用版本/元数据 | `Globals/Globals.cs` → `XFAppStatic` | 版本号、XinFa="惊羽诀"、GameVersion、URL |
| 构建时间戳 | `Resource/Data/BuildDateTime.txt` | 由 PreBuildEvent 生成 |
| 清理脚本 | `CleanCache.bat` | `rd /s /q bin` + `rd /s /q obj` |

## 本项目特有约定

- **主窗口 VM 拆分**：`MainWindowViewModels.cs`（手写）+ `MainWindowViewModelsShared.cs`（自动生成/共享逻辑）
- **调试窗口**：`DebugWindowViewModel` 拆分为 `DebugWindowViewModel.cs` + `DebugWindowViewModelShared.cs`
- **计算管道命名**：`CalculatorShell` → `DPSKernel` → 优化器输出
- **文件扩展名**：保存的计算器状态文件使用 `.jyd`（见 `XFAppStatic.FileExtension`）
- **数据加载**：`App.xaml.cs` 中 `PreLoad()` 在应用启动时执行静态数据同步；数据文件为 `Resource/Data/` 下的 `.xlsx`/`.json`
- **游戏版本字符串**：`XFAppStatic.GameVersion` = "山海源流" —— 每次资料片更新需修改

## 本项目反模式

- **`CalculatorShellShared.cs:322` 中的 `if (false)`** —— 禁用了最优属性分配。优化器运行但结果被丢弃。
- **`ConsoleMainProgram.ConsoleMain()`** —— 死代码，在 MainWindow 构造函数中通过 `#if DEBUG` 调用，无实际功能。
- **11 处无跟踪的 TODO** —— 完整清单见根 AGENTS.md；最高风险：`EventTriggerModel.cs:23`（会心交互 Bug）、`SkillData.cs:67`（硬编码 PZ 系数为 0）。
- **版本绑定代码**——`Globals.cs:50`（"技改后需要修改"）和 `SkillData.cs:67` 与游戏资料片绑定，缺乏跟踪。
- **`App.xaml.cs` 用构造函数替代 `OnStartup`** —— 大量初始化放在构造函数而非约定的覆写方法中。
- **`Show()` 在 MainWindow 构造函数中调用** —— `MainWindow.xaml.cs:55` 显式 `this.Show()`，非标准 WPF 生命周期。
- **`DoCalc()` 在构造函数末尾同步触发** —— `MainWindow.xaml.cs:66`，首次 DPS 计算在构造函数中同步执行。
- **`Application.Current.Shutdown()` 在 `OnClosing` 中调用** —— `MainWindow.xaml.cs:184`，重入关闭流程。
- **版本号漂移** —— `AssemblyInfo.cs` 版本 `1.3.4.5`，`.csproj` ClickOnce 版本 `1.9.0.*`，两者不一致。
- **`ItemDT.cs` 3 个方法抛出 `NotImplementedException`** —— `ToStr()`、`Cat()`、`GetCatStrList()` 均未实现，影响调试。
- **`FodyWeavers.xml` 启用 5 个编织器** —— `ToString`、`PropertyChanged`、`Costura`、`Equals`、`MethodTimer`。

## 建议首先阅读的文件

1. `App.xaml.cs` —— 入口点，了解启动流程
2. `Globals/Globals.cs` → `XFAppStatic` —— 所有应用常量
3. `Src/CalculatorShells/CalculatorShellShared.cs` —— 顶层计算编排
4. `Src/DPSKernelShell/DPSKernelShell.cs` —— 核心 DPS 数学
5. `ViewModels/MainWindow/Shared/MainWindowViewModels.cs` —— 主视图模型
