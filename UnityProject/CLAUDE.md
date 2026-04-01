# CLAUDE.md

请使用中文写提案和回答
这个文件为 Claude Code (claude.ai/code) 提供指导，用于处理此代码库中的代码。

TEngine 基于 HybridCLR + YooAsset + UniTask + Luban 构建。

## 核心规则

### 强制流程（适用于所有工作：propose/apply/直接编码）

**任何代码/文档输出前必须：**

1. **先读文档** - 使用 Read 工具读取相关规范
2. **显式声明** - 在输出中列出已读文档和关键规范
3. **再输出** - 然后才编写代码/方案

## 📚 文档来源

> **唯一知识库来源：`repowiki/zh/content/`**
>
> 工作流中的所有文档引用、规范查阅、维护说明均 **仅使用** `repowiki/zh/content/` 目录下的文档。

## 核心原则

1. **异步优先**：IO 操作用 `UniTask`，禁止同步加载/Coroutine
2. **模块访问**：通过 `GameModule.XXX` 访问，而非 `ModuleSystem.GetModule<T>()`
3. **资源必须释放**：`LoadAssetAsync` 对应 `UnloadAsset`，GameObject 用 `LoadGameObjectAsync`
4. **热更边界**：`GameScripts/Main` 不热更，`GameScripts/HotFix/` 全部热更
5. **事件解耦**：模块间用 `GameEvent`，UI 内部用 `AddUIEvent`

## 文档参考

详细文档见 `.claude/.claude/skills/tengine-dev/references/`：

**核心开发**：
- [architecture.md](.claude/skills/tengine-dev/references/architecture.md) - 项目结构/启动流程
- [modules.md](.claude/skills/tengine-dev/references/modules.md) - 模块 API（Timer/Scene/Audio/Fsm）
- [ui-development.md](.claude/skills/tengine-dev/references/ui-development.md) - UI 开发
- [event-system.md](.claude/skills/tengine-dev/references/event-system.md) - 事件系统
- [resource-management.md](.claude/skills/tengine-dev/references/resource-management.md) - 资源加载
- [hotfix-development.md](.claude/skills/tengine-dev/references/hotfix-development.md) - 热更代码
- [luban-config.md](.claude/skills/tengine-dev/references/luban-config.md) - 配置表
- [conventions.md](.claude/skills/tengine-dev/references/conventions.md) - 代码规范
- [troubleshooting.md](.claude/skills/tengine-dev/references/troubleshooting.md) - 问题排查

**Unity Editor 自动化（unity-skills）**：
- [unity-mcp-guide.md](.claude/skills/tengine-dev/references/unity-mcp-guide.md) - MCP 工具索引
- [ui-prefab-builder.md](.claude/skills/tengine-dev/references/ui-prefab-builder.md) - UI Prefab 拼接
- [scene-gameobject.md](.claude/skills/tengine-dev/references/scene-gameobject.md) - 场景/GameObject 操作
- [script-asset-workflow.md](.claude/skills/tengine-dev/references/script-asset-workflow.md) - 脚本/资源管理


## wiki参考

详细wiki见 `repowiki/zh/content/`：
- [index.md](repowiki/zh/content/index.md) - wiki页签目录

---

## 🔧 自我优化机制

### 问题记录
当在开发过程中发现问题时，需要记录到 `.claude/memory/` 目录：
- 创建 `.claude/memory/problem_记录日期.md` 文件
- 记录问题现象、原因分析、解决方案

### 文档同步
当发现文档与代码不一致时，使用 skill `wiki-synchelper` 同步：
```bash
/wiki:sync
```

### 优化触发条件
- 代码实现与 wiki 文档描述不符
- 发现框架潜在问题或 bug
- 文档缺失或过时
- 规范需要更新