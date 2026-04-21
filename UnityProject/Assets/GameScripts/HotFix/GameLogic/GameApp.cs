using System.Collections.Generic;
using System.Reflection;
using Fantasy;
using GameLogic;
#if ENABLE_OBFUZ
using Obfuz;
#endif
using TEngine;
using Log = TEngine.Log;

#pragma warning disable CS0436

/// <summary>
/// 游戏App。
/// </summary>
#if ENABLE_OBFUZ
[ObfuzIgnore(ObfuzScope.TypeName | ObfuzScope.MethodName)]
#endif
public partial class GameApp
{
    private static List<Assembly> _hotfixAssembly;

    /// <summary>
    /// 热更域App主入口。
    /// </summary>
    /// <param name="objects"></param>
    public static void Entrance(object[] objects)
    {
        GameEventHelper.Init();
        _hotfixAssembly = (List<Assembly>)objects[0];
        Log.Warning("======= 看到此条日志代表你成功运行了热更新代码 =======");
        Log.Warning("======= Entrance GameApp =======");
        Utility.Unity.AddDestroyListener(Release);
        Log.Warning("======= StartGameLogic =======");
        StartGameLogic();
    }
    
    private static void StartGameLogic()
    {
        // 初始化项目
        GameManager.Instance.Initialize();
        
        // 1.直接打开第一个界面
        // GameModule.UI.ShowUIAsync<UILogin>();
        
        // 2.等待一些异步操作完成后，再打开第一个界面
        var testUILoading = new TestUILoading();
        testUILoading.StartLoading(() =>
        {
            GameModule.UI.ShowUIAsync<UILogin>();
        }).Forget();
    }
    
    private static void Release()
    {
        SingletonSystem.Release();
        Log.Warning("======= Release GameApp =======");
    }
}
