# 资源生命周期管理模式

## 自动管理（无需手动释放）

```csharp
// Sprite：SetSprite 内置缓存池
_imgIcon.SetSprite("item_icon_001");
_imgIcon.SetSprite("item_icon_001", setNativeSize: true);
_imgIcon.SetSubSprite("ItemAtlas", "item_sword_01");

// GameObject：LoadGameObjectAsync 自动引用计数
var go = await GameModule.Resource.LoadGameObjectAsync("HeroPrefab", parent);
Destroy(go);  // 框架自动归还

// ❌ 禁止 LoadAssetAsync<Sprite> 或 LoadAssetAsync<GameObject> + Instantiate
```

## 手动管理（必须配对释放）

```csharp
private TextAsset _configData;

protected override async void OnRefresh()
{
    _configData = await GameModule.Resource.LoadAssetAsync<TextAsset>("level_data");
}

protected override void OnDestroy()
{
    if (_configData != null) { GameModule.Resource.UnloadAsset(_configData); _configData = null; }
}
```

## CancellationToken 取消加载

```csharp
private CancellationTokenSource _cts = new();

private async UniTaskVoid LoadAsync()
{
    try { var asset = await GameModule.Resource.LoadAssetAsync<Sprite>("icon", _cts.Token); }
    catch (OperationCanceledException) { /* 正常取消 */ }
}

protected override void OnDestroy() { _cts.Cancel(); _cts.Dispose(); }
```

## 并发与批量加载

```csharp
// 多类型并发
var (sprite, prefab, config) = await UniTask.WhenAll(
    GameModule.Resource.LoadAssetAsync<Sprite>("hero_icon"),
    GameModule.Resource.LoadGameObjectAsync("HeroModel"),
    GameModule.Resource.LoadAssetAsync<TextAsset>("hero_config")
);

// 同类型批量
var sprites = await UniTask.WhenAll(
    locations.Select(loc => GameModule.Resource.LoadAssetAsync<Sprite>(loc)));
```

## 场景切换资源整理

```csharp
await GameModule.Scene.LoadSceneAsync("BattleScene");
GameModule.Resource.UnloadUnusedAssets();  // 整理未使用资源
GameModule.Resource.ForceUnloadUnusedAssets(gcCollection: true);  // 强制整理+GC
```

核心 API 见 [resource-api.md](resource-api.md)。
