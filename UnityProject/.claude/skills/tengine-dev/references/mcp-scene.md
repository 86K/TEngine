# MCP 场景与 GameObject 操作

## manage_scene

| action | 说明 | 关键参数 |
|--------|------|---------|
| `get_active` | 当前场景信息 | — |
| `get_hierarchy` | 场景层级（分页） | `parent`, `pageSize`, `cursor`, `maxDepth` |
| `save` | 保存当前场景 | — |
| `load` | 加载场景 | `name`, `path`（需先 save） |
| `create` | 创建新场景 | `name`, `path` |
| `screenshot` | 截图到 Assets/Screenshots/ | `fileName`, `superSize` |
| `get_build_settings` | Build Settings 场景列表 | — |

`get_hierarchy` 返回含 `next_cursor`，`truncated=true` 时需翻页。

```json
{ "tool": "manage_scene", "params": { "action": "get_hierarchy", "parent": "UIRoot", "pageSize": 100 } }
```

## manage_gameobject

| action | 说明 | 关键参数 |
|--------|------|---------|
| `create` | 创建 GO | `name`, `parent`, `position`, `componentType`, `primitiveType` |
| `modify` | 修改属性 | `target`, `position`, `rotation`, `scale`, `setActive`, `name`, `parent` |
| `delete` | 删除 GO | `target` |
| `duplicate` | 复制 GO | `target`, `name`, `position` |
| `move_relative` | 相对移动 | `target`, `deltaPosition`, `space`（World/Self） |

`primitiveType`：Cube/Sphere/Plane/Cylinder/Capsule/Quad

创建示例：
```json
{ "tool": "manage_gameobject", "params": {
  "action": "create", "name": "Floor", "primitiveType": "Plane",
  "position": { "x": 0, "y": 0, "z": 0 }, "scale": { "x": 10, "y": 1, "z": 10 }
} }
```

修改父子关系：`manage_gameobject` action=`modify`，`parent`=`"NewParent"`，`worldPositionStays`=`true`

## find_gameobjects

| searchMethod | 说明 | 常用参数 |
|-------------|------|---------|
| `by_name` | 按名称 | `query`, `includeInactive`, `maxResults` |
| `by_path` | 按层级路径 | `query`（如 `"Root/Parent/Child"`） |
| `by_tag` | 按 Tag | `query`, `maxResults` |
| `by_layer` | 按 Layer | `query` |

返回：`name`、`instanceID`、`path`、`componentTypes`、`activeSelf`

```json
{ "tool": "find_gameobjects", "params": { "searchMethod": "by_name", "query": "Enemy", "includeInactive": true, "maxResults": 50 } }
```

## manage_components

| action | 说明 | 关键参数 |
|--------|------|---------|
| `add` | 添加组件 | `target`, `componentType`, `properties` |
| `remove` | 删除组件 | `target`, `componentType` |
| `set_property` | 设置组件属性 | `target`, `componentType`, `properties` |

```json
{ "tool": "manage_components", "params": {
  "action": "add", "target": "Player", "componentType": "Rigidbody",
  "properties": { "mass": 1.5, "useGravity": true }
} }
```

常用组件类型：`Rigidbody`/`Rigidbody2D`、`BoxCollider`/`SphereCollider`、`MeshRenderer`/`SpriteRenderer`、`Light`、`Camera`、`AudioSource`、`Animator`、`NavMeshAgent`、`ParticleSystem`、`Canvas`/`CanvasScaler`/`GraphicRaycaster`、`VerticalLayoutGroup`/`HorizontalLayoutGroup`

## TEngine 场景约定

| 约定 | 说明 |
|------|------|
| **UIRoot** | 场景必须存在，`UIModule.OnInit()` 自动查找 |
| **场景路径** | `Assets/Scenes/` 或 `Assets/AssetRaw/Scenes/` |
| **层级** | GameRoot → Logic / UI / Effect |
| **禁止场景中直接放 UI** | UI 通过 `UIModule.ShowUIAsync` 动态加载 |
