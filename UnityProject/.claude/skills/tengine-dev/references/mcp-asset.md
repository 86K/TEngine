# MCP 脚本与资源管理

## manage_script：C# 脚本管理

| action | 说明 | 关键参数 |
|--------|------|---------|
| `create` | 创建脚本 | `name`, `path`, `contents`, `namespace` |
| `delete` | 删除脚本 | `name`, `path` |
| `get_sha` | 获取 SHA256 | `name`, `path`（编辑前必须先获取） |
| `validate` | 验证语法 | `name`, `path`, `level`（basic/standard/comprehensive/strict） |

```json
{ "tool": "manage_script", "params": {
  "action": "create", "name": "BattleMainUI",
  "path": "Assets/GameScripts/HotFix/GameLogic/UI/Battle",
  "contents": "using TEngine;\nnamespace GameLogic\n{\n    [Window(UILayer.UI, \"BattleMainUI\")]\n    public class BattleMainUI : UIWindow { }\n}",
  "namespace": "GameLogic"
} }
```

UIWindow/UIWidget 骨架模板见 [ui-patterns.md](ui-patterns.md)。

## apply_text_edits：精确文本编辑

**推荐流程：读取 → get_sha → 精确编辑**，避免全量覆写风险。

```json
{ "tool": "apply_text_edits", "params": {
  "name": "BattleMainUI",
  "path": "Assets/GameScripts/HotFix/GameLogic/UI/Battle",
  "precondition_sha256": "<上一步的sha256>",
  "edits": [
    { "startLine": 15, "startCol": 1, "endLine": 18, "endCol": 1,
      "newText": "        protected override void OnRefresh()\n        {\n            RefreshHp(PlayerData.Hp);\n        }\n" }
  ],
  "options": { "refresh": "debounced", "validate": "standard" }
} }
```

规则：`precondition_sha256` 必须匹配当前文件 | 行列从 1 开始 | 多编辑区域不能重叠 | `refresh: "debounced"` 延迟合并编译

| 错误码 | 处理 |
|--------|------|
| `precondition_required` | 先调用 `get_sha` |
| `stale_file` | 重新获取 SHA |
| `overlap` | 按行号降序排列编辑项 |

## manage_asset：资源文件管理

| action | 说明 | 关键参数 |
|--------|------|---------|
| `search` | 搜索资源 | `query`, `type`（Prefab/Texture2D/AudioClip/...）, `path` |
| `get_info` | 获取资源信息 | `path`（返回类型、GUID、大小、依赖项） |
| `move` | 移动资源 | `path`, `newPath` |
| `rename` | 重命名 | `path`, `newName` |
| `duplicate` | 复制资源 | `path`, `newPath` |
| `delete` | 删除资源 | `path` |
| `create_folder` | 创建文件夹 | `path` |

```json
{ "tool": "manage_asset", "params": { "action": "search", "query": "BattleMainUI", "type": "Prefab", "path": "Assets/AssetRaw/UI/Prefabs" } }
```

刷新：`refresh_unity`（manage_script 操作后自动刷新，无需手动）

## manage_scriptable_object：SO 读写

| action | 说明 | 关键参数 |
|--------|------|---------|
| `read` | 读取字段 | `path` |
| `write` | 修改字段 | `path`, `properties` |
| `create` | 创建 SO 实例 | `typeName`, `path` |

```json
{ "tool": "manage_scriptable_object", "params": {
  "action": "write", "path": "Assets/GameScripts/HotFix/GameData/GameSettings.asset",
  "properties": { "maxPlayers": 4, "defaultHealth": 100 }
} }
```

## TEngine 脚本路径约定

| 类型 | 路径 |
|------|------|
| UIWindow/Widget | `Assets/GameScripts/HotFix/GameLogic/UI/<模块名>/` |
| 生成代码（绑定） | `Assets/GameScripts/HotFix/GameLogic/UI/Gen/` |
| 模块代码 | `Assets/GameScripts/HotFix/GameLogic/Module/<ModuleName>/` |
| 事件接口 | `Assets/GameScripts/HotFix/GameLogic/Event/` |
| GameProto（Luban） | `Assets/GameScripts/HotFix/GameProto/`（自动生成，勿手改） |
