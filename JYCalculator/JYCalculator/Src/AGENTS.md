# Src/ — 业务逻辑核心

**父文档：** [../AGENTS.md](../AGENTS.md) —— 项目级约定、反模式见父文档。

## 概述
DPS 计算、配装优化、技能循环评估的全部业务逻辑。包含计算管道、领域类、数据库层、数据加载和优化器。

## 目录结构
```
Src/
├── CalculatorShells/          # 顶层计算协调器
│   └── CalculatorShellShared.cs   # 编排：InitCharacter → FullCharacter → SkillDamage → ProfitDF
├── Class/                     # 领域类
│   ├── Shared/                # 共享领域类（27 文件）
│   ├── SkillCount/            # 技能计数模型
│   └── EquipSpecialEffectEntrySolvers/  # 装备特效求解器
├── Data/                      # 静态数据提供者 + 物品类
├── DB/                        # 数据库层（IDB<TKey,TValue> 实现）
│   └── Shared/                # 共享 DB 类（17 文件）
├── DPSKernelShell/            # 核心 DPS 数学管道
│   └── Shared/                # DPS 计算共享逻辑
└── DPSKernelOptimizer/        # 配装/技能优化器
    └── Shared/                # 优化器共享逻辑
```

## 功能定位速查

| 任务 | 位置 | 备注 |
|------|------|------|
| 计算编排入口 | `CalculatorShells/CalculatorShellShared.cs` | 顶层协调器，编排全流程 |
| DPS 核心管道 | `DPSKernelShell/` | `InitCharacter` → `FullCharacter` → `SkillDamage` → `ProfitDF` |
| 优化器 | `DPSKernelOptimizer/` | `DPSKernelOptimizer` + `OptimizerShell` |
| 角色初始化 | `Class/Shared/InitCharacter.cs` | 属性、装备、Buff 初始化 |
| 完整角色模型 | `Class/Shared/FullCharacter.cs` | 聚合所有属性维度 |
| 技能数据 | `Class/SkillData.cs` | 技能系数、破招、CD |
| Buff 系统 | `Class/Shared/JYBuff.cs`、`BaseBuff` | 继承自共享项目 `BaseBuff` |
| 装备特效 | `Class/EquipSpecialEffectEntrySolvers/` | 装备特殊效果计算 |
| 技能计数 | `Class/SkillCount/` | 技能释放次数模型 |
| 数据库层 | `DB/Shared/` | `BuffDB`、`ItemDTDB` 等具体实现 |
| 静态数据 | `Data/StaticXFData.cs` | 全局可变单例，保存所有运行时数据 |
| 数据加载 | `Data/XFDataLoader.cs` | 从 Excel/JSON/SQLite 加载 |

## 计算管道数据流

```
用户输入（配装/Buff/奇穴/技能循环）
  → CalculatorShellShared 编排
    → InitCharacter（角色初始化）
    → FullCharacter（属性聚合）
    → SkillDamage（技能伤害计算）
    → ProfitDF（收益矩阵）
  → 输出 DPS 结果
```

## 本目录反模式

- **`CalculatorShellShared.cs:322` 的 `if (false)` 死代码** —— `DPSKernelOptimizer` 创建并计算但结果被丢弃，优化器代码编译进 EXE 但不可达
- **`StaticXFData` 全局可变单例** —— `public static class`，各处直接调用 `StaticXFData.Data` / `.DB`，无依赖注入
- **`SkillData.cs:67` 硬编码 PZ 系数为 0** —— `[TODO] 这里暂时认为非破招技能为0`，与资料片绑定
- **`DPSKernelOptimizer.cs:197` 优化器未完成** —— `[TODO] 完成最优化`
- **`EventTriggerModel.cs:23` 会心交互 Bug** —— `[TODO]当前为临时解决方案，未考虑装备特效可能改变会心`

## 建议首先阅读的文件

1. `CalculatorShells/CalculatorShellShared.cs` —— 计算编排入口
2. `DPSKernelShell/DPSKernelShell.cs` —— 核心 DPS 数学
3. `Class/Shared/InitCharacter.cs` —— 角色初始化
4. `Class/Shared/FullCharacter.cs` —— 属性聚合模型
5. `Data/StaticXFData.cs` —— 全局数据访问点
