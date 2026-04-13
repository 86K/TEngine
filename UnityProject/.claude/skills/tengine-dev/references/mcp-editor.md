# MCP 编辑器控制与调试

## manage_editor：编辑器控制

### PlayMode

| action | 说明 |
|--------|------|
| `play` | 进入 Play Mode（`waitForCompletion: true` 等编译后进入） |
| `pause` | 暂停/恢复 |
| `stop` | 退出 Play Mode |

### Tag/Layer 管理

| action | 说明 | 参数 |
|--------|------|------|
| `add_tag` / `remove_tag` / `list_tags` | Tag CRUD | `tagName` |
| `add_layer` / `remove_layer` / `list_layers` | Layer CRUD | `layerName`（自动选 8~31 空闲槽） |

## execute_menu_item：执行菜单命令

```json
{ "tool": "execute_menu_item", "params": { "menuItem": "Assets/Refresh" } }
```

TEngine 常用菜单：

| 操作 | menuItem 路径 |
|------|-------------|
| 刷新资源 | `Assets/Refresh` |
| 保存项目 | `File/Save Project` |
| 清理缓存 | `Tools/YooAsset/Clear Build Cache` |
| 生成 UI 脚本 | `Tools/UIScriptGenerator/Generate Selected` |
| Luban 配置生成 | `Tools/Luban/Generate` |
| HybridCLR 生成 | `HybridCLR/Generate/All` |

## run_tests / get_test_job：自动化测试

```json
{ "tool": "run_tests", "params": { "mode": "EditMode", "filter": "GameLogic.Tests" } }
```

返回 `job_id`，后台运行。每 3 秒轮询：

```json
{ "tool": "get_test_job", "params": { "job_id": "<job_id>" } }
```

`status`：`running` → `completed` / `failed`

清理卡住任务：`run_tests` params=`{ "clear_stuck": true }`

## read_console：控制台日志

```json
{ "tool": "read_console", "params": { "count": 30, "logLevel": "Error" } }
```

`logLevel`：`All`、`Log`、`Warning`、`Error`、`Exception`

支持 `filter` 关键词过滤：`{ "count": 20, "filter": "BattleMainUI", "logLevel": "All" }`

## refresh_unity

```json
{ "tool": "refresh_unity", "params": {} }
```

`manage_script` 操作后自动刷新，通常无需手动。

## 调试工作流

### 场景运行调试

```
1. manage_scene action=save 保存场景
2. manage_editor action=play 进入运行
3. read_console logLevel=Error count=20 检查错误
4. manage_editor action=stop 退出
5. apply_text_edits 修复 → 等编译 → 重新运行
```

### 编译错误排查

```
1. 修改脚本后等待编译
2. read_console logLevel=Error count=30
3. 根据错误行号 apply_text_edits 修复
4. read_console 确认无新错误
```

### TEngine 热更重新生成

```json
{ "tool": "batch_execute", "commands": [
  { "tool": "execute_menu_item", "params": { "menuItem": "HybridCLR/Generate/All" } },
  { "tool": "refresh_unity", "params": {} }
], "failFast": true }
```
