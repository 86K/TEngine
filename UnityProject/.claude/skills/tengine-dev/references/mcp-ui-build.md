# MCP UI Prefab 拼接工作流

## 前缀与 MCP 创建工具对照

完整前缀→C#类型绑定见 [naming-rules.md](naming-rules.md#ui-节点命名规范)。

| 前缀 | MCP 创建方式 |
|------|------------|
| `m_btn_` | `manage_ui` action=`create_button` |
| `m_img_` | `manage_ui` action=`create_image` |
| `m_text_` | `manage_ui` action=`create_text` |
| `m_tmp_` | `manage_ui` action=`create_text`（自动检测 TMP） |
| `m_slider_` | `manage_ui` action=`create_slider` |
| `m_toggle_` | `manage_ui` action=`create_toggle` |
| `m_input_` | `manage_ui` action=`create_inputfield` |
| `m_go_/m_tf_/m_rect_` | `manage_gameobject` action=`create` |
| `m_rimg_/m_scroll_/m_scrollBar_` | `manage_gameobject` + `manage_components` add 对应类型 |
| `m_grid_/m_hlay_/m_vlay_/m_canvasGroup_` | `manage_gameobject` + `manage_components` add 对应类型 |
| `m_item_` | `manage_gameobject` + 挂对应 Widget 脚本 |

## Prefab 结构要求

UIModule 加载时强制检查根节点必须有 Canvas：

```
XxxUI.prefab（根节点）
├── [Canvas] ← 必须
├── [CanvasScaler] ← 强烈建议
├── [GraphicRaycaster] ← 交互必须
└── 子节点（m_btn_/m_tmp_/m_tf_/...）
```

存放：`Assets/AssetRaw/UI/Prefabs/<PrefabName>.prefab`

## Canvas 适配

| 参数 | 推荐值 |
|------|-------|
| UI Scale Mode | Scale With Screen Size |
| Reference Resolution | 1920 × 1080 |
| Screen Match Mode | Match Width Or Height |
| Match | 0.5 |

锚点规则：全屏→四角拉伸 | 弹窗→中心锚点+固定尺寸 | HUD→锚定对应边

## 标准工作流

```
1. batch_execute 创建根节点（Canvas+Scaler+Raycaster）+ 所有 UI 子节点
2. manage_prefabs action=create_from_gameobject → 保存为 Prefab
3. manage_gameobject action=delete → 清理场景临时 GO
```

详见下方骨架模板。

## manage_ui 速查

通用参数：`name`、`parent`、`x`/`y`、`width`/`height`

| action | 特有参数 |
|--------|---------|
| `create_canvas` | `renderMode` |
| `create_panel` | `r/g/b/a` |
| `create_button` | `text` |
| `create_text` | `text`, `fontSize`, `r/g/b/a` |
| `create_image` | `spritePath`, `r/g/b/a` |
| `create_inputfield` | `placeholder` |
| `create_slider` | `minValue`, `maxValue`, `value` |
| `create_toggle` | `label`, `isOn` |
| `ui_set_text` / `ui_set_anchor` | `text` / `preset`（TopLeft/MiddleCenter/StretchAll） |
| `ui_layout_children` | `layout`, `spacing` |

## 骨架模板

### 全屏窗口
```json
{ "tool": "batch_execute", "commands": [
  { "tool": "manage_gameobject", "params": { "action": "create", "name": "FSUI", "componentType": "Canvas" } },
  { "tool": "manage_components", "params": { "action": "add", "target": "FSUI", "componentType": "CanvasScaler" } },
  { "tool": "manage_components", "params": { "action": "add", "target": "FSUI", "componentType": "GraphicRaycaster" } },
  { "tool": "manage_ui", "params": { "action": "create_panel", "name": "m_tf_Bg", "parent": "FSUI", "width": 1920, "height": 1080, "r": 0, "g": 0, "b": 0, "a": 0.8 } },
  { "tool": "manage_ui", "params": { "action": "create_text", "name": "m_tmp_Title", "parent": "FSUI", "text": "标题", "fontSize": 48, "y": 400 } },
  { "tool": "manage_ui", "params": { "action": "create_button", "name": "m_btn_Close", "parent": "FSUI", "text": "×", "x": 600, "y": 400 } },
  { "tool": "manage_gameobject", "params": { "action": "create", "name": "m_tf_Content", "parent": "FSUI" } }
], "failFast": true }
```

### 弹窗
```json
{ "tool": "batch_execute", "commands": [
  { "tool": "manage_gameobject", "params": { "action": "create", "name": "PopUI", "componentType": "Canvas" } },
  { "tool": "manage_components", "params": { "action": "add", "target": "PopUI", "componentType": "CanvasScaler" } },
  { "tool": "manage_components", "params": { "action": "add", "target": "PopUI", "componentType": "GraphicRaycaster" } },
  { "tool": "manage_ui", "params": { "action": "create_panel", "name": "m_img_Mask", "parent": "PopUI", "width": 1920, "height": 1080, "a": 0.5 } },
  { "tool": "manage_ui", "params": { "action": "create_panel", "name": "m_tf_Win", "parent": "PopUI", "width": 600, "height": 400, "r": 0.2, "g": 0.2, "b": 0.2 } },
  { "tool": "manage_ui", "params": { "action": "create_text", "name": "m_tmp_Title", "parent": "m_tf_Win", "text": "提示", "fontSize": 32, "y": 150 } },
  { "tool": "manage_ui", "params": { "action": "create_button", "name": "m_btn_OK", "parent": "m_tf_Win", "text": "确认", "x": 100, "y": -150 } },
  { "tool": "manage_ui", "params": { "action": "create_button", "name": "m_btn_Cancel", "parent": "m_tf_Win", "text": "取消", "x": -100, "y": -150 } }
], "failFast": true }
```

### 列表（带 ScrollView）
```json
{ "tool": "batch_execute", "commands": [
  { "tool": "manage_gameobject", "params": { "action": "create", "name": "ListUI", "componentType": "Canvas" } },
  { "tool": "manage_components", "params": { "action": "add", "target": "ListUI", "componentType": "CanvasScaler" } },
  { "tool": "manage_components", "params": { "action": "add", "target": "ListUI", "componentType": "GraphicRaycaster" } },
  { "tool": "manage_gameobject", "params": { "action": "create", "name": "m_scroll_List", "parent": "ListUI" } },
  { "tool": "manage_components", "params": { "action": "add", "target": "m_scroll_List", "componentType": "ScrollRect" } },
  { "tool": "manage_gameobject", "params": { "action": "create", "name": "m_tf_Content", "parent": "m_scroll_List" } },
  { "tool": "manage_components", "params": { "action": "add", "target": "m_tf_Content", "componentType": "VerticalLayoutGroup" } }
], "failFast": true }
```

## Prefab headless 编辑

不打开 Prefab Stage，直接修改 .prefab 文件：
```json
{ "tool": "manage_prefabs", "params": {
  "action": "modify_contents",
  "prefabPath": "Assets/AssetRaw/UI/Prefabs/XxxUI.prefab",
  "target": "m_btn_Back",
  "componentProperties": { "RectTransform": { "localPosition": { "x": -400, "y": 250, "z": 0 } } }
} }
```

查看层级：`manage_prefabs` action=`get_hierarchy`
