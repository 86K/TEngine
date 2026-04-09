using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace GameLogic
{
    public static class LoadingHelper
    {
        /// <summary>
        /// 执行加载任务列表，显示加载界面，并追踪进度。
        /// </summary>
        /// <param name="rootTasks">根任务列表（可以包含任务组）。</param>
        /// <param name="onComplete">所有任务完成后的回调。</param>
        public static async UniTask RunWithLoadingAsync(List<LoadingTask> rootTasks, Action onComplete)
        {
            // 1. 将任务树“展平”，得到所有需要被依次执行的原子任务
            var allAtomicTasks = new List<LoadingTask>();
            foreach (var task in rootTasks)
            {
                allAtomicTasks.AddRange(task.Flatten());
            }
            int totalTasks = allAtomicTasks.Count;
            
            if (totalTasks == 0)
            {
                onComplete?.Invoke();
                return;
            }

            // 2. 显示 UILoading 窗口
            GameModule.UI.ShowUIAsync<UILoading>();
            
            var loadingUI = await GameModule.UI.GetUIAsyncAwait<UILoading>();

            // 3. 创建进度报告器
            var progress = new Progress<float>(p => loadingUI.SetProgress(p));

            // 4. 顺序执行所有原子任务，并在每个任务完成后报告进度
            for (int i = 0; i < allAtomicTasks.Count; i++)
            {
                var task = allAtomicTasks[i];
                await task.ExecuteAsync();
                
                // 计算并报告进度
                float currentProgress = (i + 1) / (float)totalTasks;
                ((IProgress<float>)progress).Report(currentProgress);
            }

            // 5. 所有任务完成，执行回调并关闭加载窗口
            loadingUI.SetProgress(1f);
            await UniTask.Delay(300);
            onComplete?.Invoke();
            GameModule.UI.CloseUI<UILoading>();
        }
    }
}