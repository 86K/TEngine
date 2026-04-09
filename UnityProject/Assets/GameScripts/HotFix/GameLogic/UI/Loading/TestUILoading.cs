using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TEngine;

namespace GameLogic
{
    public class TestUILoading
    { 
        public async UniTaskVoid StartLoading(Action onComplete)
        {
            // 构建你的任务列表，现在使用 LoadingTask
            var tasks = new List<LoadingTask>
            {
                // 1. 单个任务
                LoadingTask.Create("加载玩家数据", LoadPlayerDataAsync),
                
                // 2. 单个任务
                LoadingTask.Create("加载游戏配置", LoadGameConfigAsync),
                
                // 3. 任务组（嵌套任务）
                LoadingTask.CreateGroup("加载资源", new List<LoadingTask>
                {
                    LoadingTask.Create("加载UI资源", LoadUIAtlasAsync),
                    LoadingTask.Create("加载音效", LoadAudioAsync),
                    LoadingTask.CreateGroup("加载场景物件", new List<LoadingTask>
                    {
                        LoadingTask.Create("加载树木模型", LoadTreeModelAsync),
                        LoadingTask.Create("加载石头模型", LoadRockModelAsync),
                    })
                }),
                
                LoadingTask.Create("跳转到UI场景", LoadScene)
            };

            // 调用 LoadingHelper
            await LoadingHelper.RunWithLoadingAsync(tasks, () =>
            {
                Log.Info("所有加载任务完成，进入游戏！");
                // 执行加载完成后的逻辑，比如跳转场景
                onComplete?.Invoke();
            });
        }

        // --- 下面是模拟的异步加载方法 ---
        private async UniTask LoadPlayerDataAsync()
        {
            Log.Info("开始加载玩家数据...");
            await UniTask.Delay(1000);
            Log.Info("玩家数据加载完成！");
        }

        private async UniTask LoadGameConfigAsync()
        {
            Log.Info("开始加载游戏配置...");
            await UniTask.Delay(1500);
            Log.Info("游戏配置加载完成！");
        }

        private async UniTask LoadUIAtlasAsync()
        {
            Log.Info("开始加载UI图集...");
            await UniTask.Delay(500);
            Log.Info("UI图集加载完成！");
        }

        private async UniTask LoadAudioAsync()
        {
            Log.Info("开始加载音效...");
            await UniTask.Delay(800);
            Log.Info("音效加载完成！");
        }

        private async UniTask LoadTreeModelAsync()
        {
            Log.Info("开始加载树木模型...");
            await UniTask.Delay(200);
            Log.Info("树木模型加载完成！");
        }

        private async UniTask LoadRockModelAsync()
        {
            Log.Info("开始加载石头模型...");
            await UniTask.Delay(300);
            Log.Info("石头模型加载完成！");
        }

        private async UniTask LoadScene()
        {
            Log.Info("开始加载'UI'场景");
            await GameModule.Scene.LoadSceneAsync("UI");
            Log.Info("'UI'场景加载完成！");
        }
    }
}