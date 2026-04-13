# MCP 工具索引

## 核心原则：batch_execute 优先

批量操作比单次调用快 10~100 倍，多对象任务必须用 `batch_execute`：

```json
{
  "tool": "batch_execute",
  "commands": [
    { "tool": "manage_gameobject", "params": { "action": "create", "name": "Root" } },
    { "tool": "manage_gameobject", "params": { "action": "create", "name": "Child", "parent": "Root" } },
    { "tool": "manage_components", "params": { "action": "add", "target": "Child", "componentType": "Button" } }
  ],
  "failFast": true
}
```

- 默认每批最多 25 条命令（可配置，最大 100）
- `failFast: true` 遇到第一个错误即停止

## 目标定位方式（target）

| 方式 | 示例值 | 说明 |
|------|--------|------|
| 名称 | `"Canvas"` | 场景中第一个匹配 |
| 层级路径 | `"UIRoot/Canvas/Panel"` | 含 `/` 时按路径查找 |
| InstanceID | `12345` | 最可靠，不受重名影响 |
| Tag | `searchMethod: "by_tag"` | 查找指定 Tag 对象 |

`searchMethod`：`by_name`（默认）、`by_path`、`by_id`、`by_tag`

## 工具索引

| 工具名 | 用途 | 详细文档 |
|--------|------|---------|
| `batch_execute` | 批量执行命令 | 本文 |
| `manage_gameobject` | GO 创建/销毁/修改/父子 | [mcp-scene.md](mcp-scene.md) |
| `manage_scene` | 场景加载/保存/截图 | [mcp-scene.md](mcp-scene.md) |
| `find_gameobjects` | 按名/路径/Tag/Layer 查找 | [mcp-scene.md](mcp-scene.md) |
| `manage_components` | 组件添加/删除/属性 | [mcp-scene.md](mcp-scene.md) |
| `manage_prefabs` | Prefab 创建/检查/修改 | [mcp-ui-build.md](mcp-ui-build.md) |
| `manage_ui` | UGUI 元素操作 | [mcp-ui-build.md](mcp-ui-build.md) |
| `manage_script` | C# 脚本 CRUD/验证 | [mcp-asset.md](mcp-asset.md) |
| `apply_text_edits` | 文本精确编辑（需 SHA） | [mcp-asset.md](mcp-asset.md) |
| `validate_script` | 验证 C# 语法 | [mcp-asset.md](mcp-asset.md) |
| `get_sha` / `find_in_file` | SHA256 / 文件内容搜索 | [mcp-asset.md](mcp-asset.md) |
| `manage_asset` | 资源导入/搜索/信息 | [mcp-asset.md](mcp-asset.md) |
| `manage_scriptable_object` | SO 读写 | [mcp-asset.md](mcp-asset.md) |
| `manage_material` | 材质创建/属性设置 | [mcp-visual.md](mcp-visual.md) |
| `manage_shader` | Shader CRUD | [mcp-visual.md](mcp-visual.md) |
| `manage_texture` | 纹理导入设置 | [mcp-visual.md](mcp-visual.md) |
| `manage_vfx` | 粒子/VFX/Line/Trail | [mcp-visual.md](mcp-visual.md) |
| `manage_animation` | Animator/Clip/状态机 | [mcp-visual.md](mcp-visual.md) |
| `manage_editor` | PlayMode/Tag/Layer | [mcp-editor.md](mcp-editor.md) |
| `run_tests` / `get_test_job` | 启动测试/轮询结果 | [mcp-editor.md](mcp-editor.md) |
| `read_console` | 读取 Console 日志 | [mcp-editor.md](mcp-editor.md) |
| `execute_menu_item` | 执行 Unity 菜单命令 | [mcp-editor.md](mcp-editor.md) |
| `refresh_unity` | 刷新 AssetDatabase | [mcp-editor.md](mcp-editor.md) |

## 常见错误

| 错误 | 原因 | 解决 |
|------|------|------|
| `Target not found` | 名称不存在 | 先 `find_gameobjects` 确认 |
| `Scene has unsaved changes` | 未保存即切换场景 | 先 `manage_scene` save |
| `Prefab asset requires manage_prefabs` | 对 .prefab 用错工具 | 改用 `manage_prefabs` |
| `precondition_required` | 缺少 SHA | 先 `get_sha` |
| `stale_file` | 文件已被修改 | 重新 `get_sha` |
| `batch too large` | 单批超限 | 拆分多个 batch_execute |
