# BuildCompareModel 设计文档

**最后更新：** 2026-06-25
**状态：** 第一版装备比较已实现

## 目标

`BuildCompareModel` 用于比较多个"构建变体"的 DPS 差异。第一版服务于配装器的装备比较：用户点击"智能对比"按钮后，弹出一个 Syncfusion SfDataGrid 窗口，展示当前装备槽位当前筛选条件下每件候选装备替换后的 DPS、DPS 变化百分比，并支持排序。

该模型不能只绑定装备比较。未来还应支持小附魔、大附魔、五行石、五彩石、套装方案等构建变体比较。因此命名统一为 `BuildCompareModel`，而不是 `EquipCompareModel`。

## 目录约定

主计算侧代码和设计文档放在：

```text
JYCalculator/JYCalculator/Models/BuildCompareModel/
```

实际文件结构：

```text
Models/BuildCompareModel/
├── BuildCompareModel设计文档.md
├── BuildCompareModel.cs
├── BuildCompareArg.cs
├── BuildCompareConditionSnapshot.cs
├── BuildCompareCandidate.cs
├── BuildCompareResultItem.cs
├── BuildCompareResult.cs
└── BuildCompareDpsEvaluator.cs
```

JX3PZ 配装侧：

```text
JX3PZ/JX3PZ/Models/BuildCompareModel/
├── PzBuildToInitInputModel.cs
├── PzEquipBuildCompareCandidateProvider.cs
└── PzBuildVariantFactory.cs
```

窗口和 ViewModel：

```text
JX3PZ/JX3PZ/ViewModels/BuildCompareModel/
├── PzBuildCompareWindowViewModel.cs
└── PzBuildCompareRowViewModel.cs

JX3PZ/JX3PZ/Views/BuildCompareModel/
├── PzBuildCompareWindow.xaml
└── PzBuildCompareWindow.xaml.cs
```

此外还新增了以下基础设施文件：

```text
JYCalculator/JYCalculator/Models/InitInputModel/
├── InitInputModel.cs    # 从 JBBBPZPanel 生成 InitInputSav 的纯 Model 入口
└── InitInputSav.cs      # 从 InitInputViewModel 提取的序列化存储类

JX3CalculatorShared/JX3CalculatorShared/Src/Class/
└── CalcResult.cs        # 新增 FailReason 字段
```

## 边界划分

### JYCalculator.Models.BuildCompareModel

主项目负责 DPS 计算和通用比较逻辑，不关心 UI 控件，也不关心候选来自装备、附魔还是其他来源。

职责：

- 接收一组 `BuildCompareCandidate`。
- 接收一个 `BuildCompareConditionSnapshot`。
- 对每个候选构建独立计算 DPS。
- 输出 `BuildCompareResult`。
- 复用 `CalcResult`，收益字段允许为 `null`。
- 检测不支持的加速/奇穴（通过 `shell.FinalDPStxt.Contains("不支持")` 判定），标记 `Success = false` 并写入 `FailReason`。

不应做：

- 不直接读取 `ComboBox`、`DataGrid`、`PzTabItemViewModel`。
- 不修改主窗口 `MainWindowViewModels`。
- 不修改配装器当前选择。
- 不把排序、列宽、颜色等展示逻辑放进计算模型。

### JX3PZ.Models.BuildCompareModel

配装项目负责从当前配装状态生成候选构建。

第一版装备比较职责：

- 读取当前槽位 `EquipSelectViewModel.ItemsSourceView` 中筛选后的装备。
- 从当前 `PzPlanModel` 导出并冻结一份 `JBPZEquipSnapshotCollection`（构造时即冻结，后台不再读取可变 Plan）。
- 对每个候选装备生成一个虚拟配装快照。
- 只替换目标槽位的装备 `id`。
- 保留当前槽位的 `strength / enhance / enchant / embedding / stone`。
- 通过纯 Model 逻辑归一化不兼容强化配置。
- 虚拟配装不能依赖真实 WPF 控件的 `SelectedIndex -> SelectedItem` 联动；加载装备、附魔等配置时必须在 Model 层显式还原为装备、附魔、五行石、五彩石对象。
- `JBBB` 原始导入可以使用 JSON 中自带的角色属性生成 `InitCharacter`；但智能对比替换装备后，原始角色属性快照已经失效，必须从虚拟 BUILD 重新计算 `InitCharacter`。

### PzBuildToInitInputModel

`PzBuildToInitInputModel` 是 JX3PZ 侧的纯 Model 转换器，负责从 `JBPZEquipSnapshotCollection` 直接计算 `InitInputSav`。

职责：

- 输入 `JBPZEquipSnapshotCollection` 和可选标题。
- 直接查询 `StaticPzData` / `StaticXFData`，把每个槽位快照还原成 `EquipSnapShotModel`。
- 根据候选装备自身的孔位定义和 `embedding` 等级还原五行石属性。
- 对小附魔、大附魔做兼容性判断，不兼容时置空（通过 `equip.FitEnhance()` / `equip.FitBigFM()`）。
- 构造 `PzPlanModel` 并执行 `Calc()`、`CalcSet()`、`GetTotalEntryCollection()`、`GetCharacterPanel()`。
- 通过 `PzToXinFaInputAdapter` 输出 `InitInputSav`。

不应做：

- 不创建 `PzTabItemViewModel`。
- 不创建或读取 WPF `CollectionViewSource`。
- 不依赖 `SelectedIndex`、`SelectedItem` 或任何 UI 绑定副作用。
- 不发送 `PzChangedMessage` / `PzPlanMessage`。

## 核心数据模型

### BuildCompareCandidate

描述一个待比较构建变体。

```csharp
public class BuildCompareCandidate
{
    public string Key { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public int Level { get; set; }
    public int IconID { get; set; }          // 装备图标ID
    public QualityEnum Quality { get; set; }  // 品质（用于着色）
    public string AttributeDesc { get; set; }
    public bool IsCurrent { get; set; }
    public InitInputSav InitInput { get; set; }
    public Func<InitInputSav> CreateInitInput { get; set; }
}
```

说明：

- `Key` 用于去重和定位，例如装备 `EID`。
- `Category` 用于区分装备、小附魔、大附魔等。
- `IconID` / `Quality` 用于 UI 展示装备图标和品质色。
- `InitInput` 是候选变体归一化后的计算输入。
- `CreateInitInput` 用于延迟生成候选输入，避免打开窗口时同步计算全部候选。

### BuildCompareConditionSnapshot

描述 DPS 计算所需的非 BUILD 条件快照。

```csharp
public class BuildCompareConditionSnapshot
{
    public QiXueConfigModel QiXue { get; set; }
    public Target Target { get; set; }
    public AbilityItem Ability { get; set; }
    public ZhenFa Zhen { get; set; }
    public FightTimeSummary FightTime { get; set; }
    public BuffShellInput Buffs { get; set; }
    public SkillDataDF SkillDataDFMiJiQiXue { get; set; }
    public SkillBuild[] RecipeCompatibleSkillBuilds { get; set; }
    public BuffSpecialArg BuffSpecial { get; set; }
    public bool TargetAllWaysFullHP { get; set; }
    public double ShortTimeBonus { get; set; }
    public double BaseDps { get; set; }

    public static BuildCompareConditionSnapshot FromMainModel(MainWindowModel model);
    public CalculatorShellArg CreateCalculatorShellArg(InitInputViewModel initInput);
    public FightTimeSummary CopyFightTime();
}
```

快照要求：

- `Target` 必须使用 `Copy()`。
- `SkillDataDFMiJiQiXue` 必须使用 `Copy()`。
- `FightTimeSummary` 必须新建实例（`CopyFightTime()`），不能复用主界面绑定对象。
- `CreateCalculatorShellArg()` 生成 `EnableProfit = false`、`EnableOptimization = false` 的参数。
- `BaseDps` 使用当前主计算结果 `CalcResult.FinalDPS`，不重复计算基准。
- `Buffs` 通过 `new BuffShellInput(...)` 从主界面提取快照值，不持有 ViewModel 引用。

### BuildCompareResultItem

```csharp
public class BuildCompareResultItem
{
    public BuildCompareCandidate Candidate { get; set; }
    public bool Success { get; set; }
    public double Dps { get; set; }
    public double DeltaDps { get; set; }
    public string ErrorMessage { get; set; }
    public CalcResult CalcResult { get; set; }
}
```

说明：

- `DeltaDps = Dps - condition.BaseDps`。
- 失败项（`Success = false`）保留在表格中；`CalcResult.FailReason` 携带具体原因（如"不支持的加速！"）。
- `CalcResult.ProfitOrderDesc` 和 `CalcResult.DamageDeriv` 在轻量模式下允许为 `null`。

### CalcResult（共享基础设施改动）

```csharp
public class CalcResult
{
    public bool Success;
    public double FinalDPS;
    public string ProfitOrderDesc;
    public DamageDerivBase DamageDeriv;
    public string FailReason;   // 新增：失败原因
}
```

### BuildCompareModel

```csharp
public class BuildCompareModel
{
    public BuildCompareArg Arg { get; }
    public BuildCompareResult Result { get; private set; }

    public BuildCompareResult Calc();          // 批量计算
    public BuildCompareResultItem CalcCandidate(BuildCompareCandidate candidate); // 单候选计算
}
```

### BuildCompareArg

```csharp
public class BuildCompareArg
{
    public BuildCompareConditionSnapshot Condition { get; set; }
    public IList<BuildCompareCandidate> Candidates { get; set; }
}
```

## DPS 计算入口

### BuildCompareDpsEvaluator

职责：

- 输入 `InitInputSav`。
- 使用隔离的计算上下文计算 DPS。
- 创建局部 `CalculatorShell` + 局部 `InitInputViewModel`，不共享任何可变状态。
- 检测不支持的加速/奇穴：`shell.FinalDPStxt?.Contains("不支持")` 时标记失败并记录 `FailReason`。
- 输出 `CalcResult`。

重要约束：

- 不允许把主窗口的 `InitInputViewModel` 传入候选计算。
- 不允许复用主窗口的 `FightTimeSummary` 实例。
- 不允许候选计算触发 `GlobalContext.IsPZSyncWithCalc` 改动。
- 不允许候选计算发送 `PzPlanMessage` 或其他会修改 UI 状态的消息。

当前实现使用临时适配器：

```text
InitInputSav
  -> CreateRuntimeInitInput(sav) 创建临时 InitInputViewModel（局部对象，不挂全局上下文）
  -> CalculatorShell(initInput, condition.Copy*(), arg{EnableProfit=false})
  -> shell.Calc()
  -> GetCalcResult() + FailReason 检测
```

## 第一版装备比较流程

```text
用户点击当前槽位的"智能对比"
  -> PzEquipBuildCompareCandidateProvider 冻结 BUILD 快照 + 遍历 ItemsSourceView
  -> PzBuildVariantFactory 为每件装备生成虚拟配装快照（只替换目标槽位装备 id）
  -> PzBuildToInitInputModel 将虚拟配装转换为 InitInputSav
  -> BuildCompareConditionSnapshot.FromMainModel() 冻结主界面非 BUILD 条件
  -> PzBuildCompareWindowViewModel 创建行 VM + StartCalcAsync()
  -> 表格先渲染候选行（状态"等待"），后台顺序计算
  -> BuildCompareDpsEvaluator.Calc() 对每个候选轻量 DPS
  -> 逐行刷新 DPS / 差异 / 状态
```

候选范围：

- 使用当前 `EquipSelectViewModel.ItemsSourceView`。
- 不遍历全装备库。
- 当前装备必须包含在结果中（`IsCurrent = true`），按品级降序排列。
- 候选计算基于打开窗口时捕获的 BUILD 快照，不在后台线程读取可变的 `PzPlanModel`。

基准 DPS：

- 使用当前主计算已得到的 `CalcResult.FinalDPS`。
- 不重新计算基准。

候选 DPS：

- 使用 `EnableProfit = false`。
- 使用 `EnableOptimization = false`。
- 只取 `CalcResult.FinalDPS`。

## 执行策略

第一版采用**单线程顺序计算**：

- 窗口构造时先生成候选行，立即展示表格。
- `StartCalcAsync()` → `await Task.Yield()` 让 UI 先渲染 → `foreach` + `await Task.Run(...)` 逐行计算。
- 每行完成即回 UI 线程刷新 `Dps` / `DeltaDps` / `Status`。
- 进度条 + 计数器实时更新，完成后显示总耗时。

未来改为并行只需将 `foreach` 替换为 `SemaphoreSlim + Task.WhenAll`。

并行安全边界（已满足）：

- `PzBuildToInitInputModel` 不创建 ViewModel，不触发 UI 绑定，不发送消息。
- `PzBuildToInitInputModel` 只读取 `StaticPzData` / `StaticXFData`，不写入静态数据库。
- `BuildCompareDpsEvaluator` 为每个候选创建局部 `CalculatorShell` 和局部 `InitInputViewModel`。
- `BuildCompareConditionSnapshot` 保存无相互影响的稳定快照；已知会被修改的对象（FightTime、Target、SkillDataDF）已 Copy。
- `QiXue`、`Ability`、`Zhen`、`Buffs` 在 `Calc()` 中只读，线程安全。
- 行状态更新通过 `await` 自然回到 WPF UI 线程。

## 兼容性策略

统一走纯 Model 归一化路径：

```text
当前配装导出 JBPZEquipSnapshotCollection
  -> 复制目标槽位当前快照（CopySnapshot 深拷贝各字段）
  -> 只替换目标槽位装备 id
  -> 构造虚拟配装快照集合
  -> PzBuildToInitInputModel（纯 Model）：StaticPzData/StaticXFData 还原 EquipSnapShotModel[]
  -> PzPlanModel.Calc() / CalcSet() / GetCharacterPanel()
  -> PzToXinFaInputAdapter → InitInputSav
```

精炼、小附魔、大附魔、五行石、五彩石的兼容性处理与真实导入配装保持一致。

## UI 展示

### 窗口控件

- 顶部：基准DPS 数值 + 进度条 + 计数器（`3/15`）+ 总耗时
- 表格：Syncfusion `SfDataGrid`，`ColumnSizer="Star"`，`AllowSorting="True"`
- 行高 34px，纯水平网格线

### 列定义

| 列名 | 类型 | 绑定 | 排序 | 备注 |
|------|------|------|:--:|------|
| 装备 | GridTemplateColumn | IconID(图标) + Name(品质色) | Name | 当前装备显示(当前)灰色标记 |
| 品级 | GridNumericColumn | Level | ✓ | |
| 属性标签 | GridTextColumn | AttributeDesc | ✓ | |
| DPS | GridNumericColumn | Dps | ✓ | 无效行显示 NaN（空白） |
| 差异 | GridTemplateColumn | DeltaDpsPct(百分比+着色) | DeltaDps | 绿(+)/红(-)/灰(0)；无效行显示 `-` |
| 状态 | GridTextColumn | Status | ✓ | 等待/计算中/完成/不支持的加速！/不支持的奇穴/秘籍！ |

### 着色规则

- 装备名称：`QualityToColorConverter`（品质色）
- DPS变化：`DeltaDpsColor`（绿 `#229954` / 红 `#C0392B` / 灰）
- 当前标记：灰色小号字体 `"(当前)"`

### ViewModel 模式

两个 ViewModel 均继承 `ObservableObject`（CommunityToolkit.Mvvm），配合 Fody.PropertyChanged 自动织入：
- 全部 auto-property，无 backing field
- `[AlsoNotifyFor]` 标注依赖计算属性
- `OnPropertyChanged()` 仅在必要时手动调用

## 错误处理

单个候选失败不应中断整个比较。

- `BuildCompareDpsEvaluator` 检测不支持的加速/奇穴，写入 `CalcResult.FailReason`。
- 失败行保留在表格中，`Status` 列显示具体原因（如"不支持的加速！"）。
- DPS 和差异列为空或显示 `-`。
- 异常捕获写入 `ErrorMessage`，状态显示"无效"（兜底）。

## 后续扩展点

### 附魔比较

新增 provider：`PzEnhanceBuildCompareCandidateProvider`。候选变体只替换当前槽位的 `enhance`。

### 大附魔比较

新增 provider：`PzBigFmBuildCompareCandidateProvider`。候选变体只替换当前槽位的 `enchant`。

### 五行石比较

新增 provider：`PzDiamondBuildCompareCandidateProvider`。候选变体只替换当前槽位的 `embedding`。

### 原 ComboBox 实时对比

当独立窗口方案稳定后，可以把 `BuildCompareResult` 缓存回当前槽位 VM，由 `EquipItemView` 读取展示。

## 验收标准（第一版）

- [x] 当前槽位出现"智能对比"入口（PzTabItem.xaml 按钮 + RelayCommand）
- [x] 点击后打开独立窗口（Syncfusion SfDataGrid）
- [x] 窗口表格展示当前筛选候选装备（含图标 + 品质色）
- [x] 每行显示 DPS 和 DPS 变化百分比（红绿着色）
- [x] 点击列头可以排序
- [x] 比较过程不改变当前主窗口输入 / 配装选择
- [x] 基准 DPS 使用当前主计算结果，候选 DPS 使用轻量计算
- [x] 加速不支持的装备保留在表中，标注"不支持的加速！"
- [x] 进度条 + 实时计数器 + 总耗时
- [x] ViewModel 使用 ObservableObject + Fody.PropertyChanged 模式
