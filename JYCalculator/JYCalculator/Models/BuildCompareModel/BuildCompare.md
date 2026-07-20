# BuildCompare 智能对比功能 — 开发总结

**日期：** 2026-06-25
**状态：** 第一版装备比较已完成

---

## 功能概述

配装器当前槽位的"智能对比"：点击按钮或按 `Alt+C`，弹出独立窗口，用 SfDataGrid 展示当前筛选条件下每件候选装备替换后的 DPS、差异百分比，支持排序、筛选、装备 ToolTip。

- 窗口先展示候选列表，后台并行计算逐行刷新
- 加速不支持的装备保留在表中，标注"不支持的加速！"
- 当前装备显示蓝色"当前"徽章，悬停显示游戏品质 ToolTip
- DPS 变化百分比红（降低）绿（提升）着色

## 文件清单

### 主计算侧 — `JYCalculator.Models.BuildCompareModel/`

| 文件 | 职责 |
|------|------|
| `BuildCompareModel.cs` | 批量/单候选 DPS 比较调度 |
| `BuildCompareArg.cs` | 比较参数（条件 + 候选列表） |
| `BuildCompareCandidate.cs` | 候选变体数据模型（含 IconID/Quality/EquipShowVM） |
| `BuildCompareConditionSnapshot.cs` | 主界面非 BUILD 条件的冻结快照 |
| `BuildCompareDpsEvaluator.cs` | 单候选轻量 DPS 计算（创建隔离 CalculatorShell） |
| `BuildCompareResultItem.cs` | 单候选计算结果 |
| `BuildCompareResult.cs` | 结果集合 |
| `BuildCompareModel设计文档.md` | 架构设计文档 |
| `BuildCompare.md` | 本文档 |

### 配装侧 — `JX3PZ.Models.BuildCompareModel/`

| 文件 | 职责 |
|------|------|
| `PzBuildToInitInputModel.cs` | 纯 Model 转换：JBPZEquipSnapshotCollection → InitInputSav |
| `PzBuildVariantFactory.cs` | 虚拟配装快照生成（复制+替换目标槽位装备 ID） |
| `PzEquipBuildCompareCandidateProvider.cs` | 遍历筛选结果生成候选列表 |

### UI 层 — `JX3PZ.ViewModels/` & `JX3PZ.Views/`

| 文件 | 职责 |
|------|------|
| `PzBuildCompareWindowViewModel.cs` | 窗口 VM：进度/耗时/并行调度 |
| `PzBuildCompareRowViewModel.cs` | 行 VM：Dps/DeltaDpsPct/DeltaDpsColor/Status/EquipShowVM |
| `PzBuildCompareWindow.xaml` | 窗口界面（SfDataGrid + 状态栏卡片） |
| `PzBuildCompareWindow.xaml.cs` | 窗口代码后置 |

### 入口改动

| 文件 | 改动 |
|------|------|
| `EquipSelect.xaml` | "装备选择"标签下方添加蓝色"智能对比 Alt+C"按钮（RelativeSource 绑定到 PzTabItem.OpenBuildCompareCmd） |
| `PzTabItemViewModel.cs` | 新增 `OpenBuildCompareCmd`（RelayCommand → new PzBuildCompareWindow） |
| `PzMainWindow.xaml` | `Window.InputBindings`：`Alt+C` → `OpenBuildCompareCmd`；`TabControl.SelectedIndex` 绑定 |
| `PzMainWindowViewModels.cs` | 新增 `OpenBuildCompareCmd` + `SelectedIndex`，Alt+C 对当前选中槽位弹出窗口 |

### 基础设施改动

| 文件 | 改动 |
|------|------|
| `CalcResult.cs` | 新增 `FailReason` 字段 |
| `CalculatorShellArg.cs` | 新增 `EnableProfit` 标志（对比计算关闭收益计算） |
| `CalculatorShellShared.cs` | `CalcDPSKernelShell()` 条件执行 `CalcProfit()`；`GetCalcResult()` 可空处理 |
| `InitInputModel.cs` / `InitInputSav.cs` | 从 `InitInputViewModel` 提取纯 Model 入口 |
| `JBBBPZPanel.cs` | 通过 `InitInputModel.Calc()` 生成 InitInputSav |

## 核心数据流

```
用户点击"智能对比"或按 Alt+C
  │
  ▼ PzBuildCompareWindowViewModel 构造（UI 线程）
  │
  ├── BuildCompareConditionSnapshot.FromMainModel()   ← 冻结条件快照
  ├── PzEquipBuildCompareCandidateProvider
  │     ├── 构造时 ExportJBPZEquipSnapshotCollection() ← 冻结 BUILD
  │     └── GetCandidates() ← 遍历 EquipSelectVM.ItemsSourceView
  │           └── CreateCandidate(Equip equip)
  │                 ├── BuildCompareCandidate { IconID, Quality, EquipShowVM, ... }
  │                 └── CreateInitInput = () => Factory.CreateEquipInitInput(...)
  │
  ├── 生成 PzBuildCompareRowViewModel 行（先渲染表格）
  │
  ▼ StartCalcAsync() — SemaphoreSlim(min(6, CPU-1)) 限定并发
  │
  │  每候选并行（CreateInitInput 延迟执行）:
  │  ├── PzBuildVariantFactory.CreateEquipInitInput()
  │  │     └── PzBuildToInitInputModel.Calc()   ← 纯 Model 路径
  │  │           ├── StaticPzData/StaticXFData 还原装备/附魔/五行石/五彩石
  │  │           ├── PzPlanModel.Calc() / CalcSet() / GetCharacterPanel()
  │  │           └── PzToXinFaInputAdapter → InitInputSav
  │  │
  │  └── BuildCompareDpsEvaluator.Calc(InitInputSav)
  │        ├── CreateRuntimeInitInput(sav) → 临时 InitInputViewModel
  │        ├── CalculatorShell(initInput, condition..., arg{EnableProfit=false})
  │        ├── shell.Calc() → 检测 IsSupport + FinalDPStxt.Contains("不支持")
  │        └── CalcResult { Success, FinalDPS, FailReason }
  │
  ▼ UI 线程逐行刷新 Dps / DeltaDpsPct / DeltaDpsColor / Status
```

## 线程安全保证

| 关注点 | 状态 |
|--------|------|
| `QiXue` / `Ability` / `Zhen` / `Buffs` | `Calc()` 期间只读，直接引用传递安全 |
| `FightTime` / `Target` / `SkillDataDF` | 已 `Copy()` |
| `CalculatorShell` | 每候选独立创建 |
| `InitInputViewModel` | 局部临时创建，不挂全局上下文 |
| `PzBuildToInitInputModel` | 只读 `StaticPzData` / `StaticXFData`，不写 |
| `Completed++` / `OnPropertyChanged` | `await` 回到 UI 线程执行 |
| `BigFMConfigViewModel()` | 写 `GlobalContext.BigFMSlots`（值相同）— 单线程无竞态 |

## SfDataGrid 列定义

| 列 | 类型 | MappingName | 排序 | 特点 |
|----|------|:----------:|:--:|------|
| 装备 | GridTemplateColumn | Name | ✓ | 图标(26px) + 品质色名称 + 蓝色"当前"徽章 |
| 品级 | GridNumericColumn | Level | ✓ | 居中对齐，Width=65 |
| 属性 | GridTextColumn | AttributeDesc | ✓ | — |
| DPS | GridNumericColumn | Dps | ✓ | 无效行留空 |
| 差异 | GridTemplateColumn | DeltaDps | ✓ | `+1.30%`格式，红/绿/灰色 |
| 状态 | GridTextColumn | Status | ✓ | ✓/等待/计算中/不支持的加速！ |

## 快捷键

| 快捷键 | 作用域 | 功能 |
|--------|--------|------|
| `Alt+C` | 配装器窗口 | 对当前选中槽位弹出智能对比窗口 |

## 构建

必须使用 VS 2022 自带的 MSBuild：
```
D:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe
```

`dotnet build` / `dotnet msbuild` 对旧式 .NET Framework .csproj 不兼容。

## ViewModel 模式

两个新 ViewModel 均继承 `ObservableObject`（CommunityToolkit.Mvvm），配合 Fody.PropertyChanged 自动织入：
- 全部 auto-property，无 backing field
- `[AlsoNotifyFor]` 标注依赖计算属性
- `OnPropertyChanged()` 仅在必要时手动调用
