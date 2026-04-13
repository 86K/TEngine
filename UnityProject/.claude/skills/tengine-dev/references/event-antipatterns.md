# 事件系统常见错误

## 1. 忘记 GameEventHelper.Init()

```csharp
// ❌ GameEvent.Get<T>() 全部无响应，无报错，极难排查
public static void Entrance(Assembly[] assemblies)
{
    // GameEventHelper.Init();  ← 忘记调用
    GameApp_RegisterSystem.Register();
}

// ✅ 必须最先调用
GameEventHelper.Init();
```

## 2. UI 外部使用 AddEventListener（内存泄漏）

```csharp
// ❌ 退出窗口不会自动清理
void SomeMethod()
    => GameEvent.AddEventListener<int>(IBattleEvent_Event.OnHpChanged, OnHpChanged);

// ✅ UIWindow 中用 AddUIEvent（自动清理）
protected override void RegisterEvent()
    => AddUIEvent<int>(IBattleEvent_Event.OnHpChanged, OnHpChanged);

// ✅ 非 UI 类用 GameEventMgr
private readonly GameEventMgr _eventMgr = new();
public void Dispose() => _eventMgr.Clear();
```

## 3. 手写事件 ID 常量

```csharp
// ❌ 手写 int 常量，容易重复/拼错
public const int OnHpChanged = 1001;

// ✅ Source Generator 自动生成
AddUIEvent(IBattleEvent_Event.OnHpChanged, OnHpChanged);
```

## 4. 事件回调签名不匹配

```csharp
// ❌ 事件发送 int，回调接收 string → 运行时异常
GameEvent.Send<int>(IBattleEvent_Event.OnHpChanged, hp);
AddUIEvent<string>(IBattleEvent_Event.OnHpChanged, OnHp);

// ✅ 接口事件模式可编译期检查
GameEvent.Get<IBattleEvent>().OnHpChanged(hp); // 类型安全
```

## 5. 非 UI 类忘记移除监听

```csharp
// ❌ 销毁时不移除，回调引用已释放对象
public class PlayerSystem
{
    public void Init() => GameEvent.AddEventListener(IPlayerEvent_Event.OnDead, OnDead);
    // 没有 RemoveEventListener → 泄漏
}

// ✅ 使用 GameEventMgr 批量清理
private readonly GameEventMgr _eventMgr = new();
public void Init()    => _eventMgr.AddEvent(IPlayerEvent_Event.OnDead, OnDead);
public void Dispose() => _eventMgr.Clear();
```

核心 API 见 [event-system.md](event-system.md)。
