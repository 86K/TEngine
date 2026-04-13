---
name: tengine-dev
description: TEngine Unity 游戏框架开发指导。(1)模块系统(2)UI开发(3)事件系统(4)资源管理(5)热更代码(6)Luban配置(7)异步编程(8)代码规范(9)MCP操作。关键词：TEngine,UIWindow,UIWidget,GameModule,ResourceModule,YooAsset,HybridCLR,UniTask,EventInterface,GameEvent,Luban,ConfigSystem,GameApp,unity-mcp,batch_execute。不触发：L1任务（typo/注释/改名）
---

# TEngine 开发技能

TEngine 基于 HybridCLR + YooAsset + UniTask + Luban 构建。

## 场景导航

| 做什么 | 查什么 | 层级 |
|--------|--------|------|
| 项目架构 / 启动流程 | [architecture.md](references/architecture.md) | 核心 |
| 模块 API 速查 | [modules.md](references/modules.md) | 核心 |
| UI 开发（生命周期 / 层级 / 属性）| [ui-lifecycle.md](references/ui-lifecycle.md) | 核心 |
| 事件系统（两种模式 / 核心接口）| [event-system.md](references/event-system.md) | 核心 |
| 资源加载 / 卸载 | [resource-api.md](references/resource-api.md) | 核心 |
| 热更代码 / HybridCLR / GameApp | [hotfix-workflow.md](references/hotfix-workflow.md) | 核心 |
| Luban 配置表 | [luban-config.md](references/luban-config.md) | 核心 |
| 代码规范 / 命名约定 | [naming-rules.md](references/naming-rules.md) | 核心 |
| UI 进阶模式（Widget 模板 / 节点绑定）| [ui-patterns.md](references/ui-patterns.md) | 进阶 |
| 事件避坑 | [event-antipatterns.md](references/event-antipatterns.md) | 进阶 |
| 资源管理模式 / 生命周期 | [resource-patterns.md](references/resource-patterns.md) | 进阶 |
| MCP 工具索引 / batch_execute | [mcp-index.md](references/mcp-index.md) | MCP |
| 拼 UI Prefab | [mcp-ui-build.md](references/mcp-ui-build.md) | MCP |
| 操作场景 / GameObject | [mcp-scene.md](references/mcp-scene.md) | MCP |
| 创建脚本 / 资源管理 | [mcp-asset.md](references/mcp-asset.md) | MCP |
| 材质 / Shader / VFX / 动画 | [mcp-visual.md](references/mcp-visual.md) | MCP |
| 编辑器控制 / 测试 / 日志 | [mcp-editor.md](references/mcp-editor.md) | MCP |
| 问题排查 | [troubleshooting.md](references/troubleshooting.md) | 排障 |

## 核心原则

1. **异步优先**：IO 操作用 `UniTask`，禁止同步加载/Coroutine
2. **模块访问**：GameLogic层通过 `GameModule.XXX` 访问，而非 `ModuleSystem.GetModule<T>()`
3. **资源必须释放**：`LoadAssetAsync` 对应 `UnloadAsset`，GameObject 用 `LoadGameObjectAsync`
4. **热更边界**：`GameScripts/Main` 不热更，`GameScripts/HotFix/` 全部热更
5. **事件解耦**：模块间用 `GameEvent`，UI 内部用 `AddUIEvent`

## 使用提示

- **代码优先于文档**：若本 skill references 中的 API 与项目实际代码不符，**以代码为准**。发现冲突时在输出中标注。
- **程序集分层**：详见 [architecture.md](references/architecture.md)
