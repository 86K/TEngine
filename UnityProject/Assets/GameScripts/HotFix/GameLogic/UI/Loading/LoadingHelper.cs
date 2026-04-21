using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public static class LoadingHelper
    {
        private const int WaitCompleteTimeoutMs = 3000;

        /// <summary>
        /// 执行加载任务列表，显示加载界面，并追踪进度。
        /// </summary>
        /// <param name="rootTasks">根任务列表（可以包含任务组）。</param>
        /// <param name="onComplete">所有任务完成后的回调。</param>
        public static async UniTask RunWithLoadingAsync(List<LoadingTask> rootTasks, Action onComplete)
        {
            // 1. 将任务树"展平"，得到所有需要被依次执行的原子任务
            var allAtomicTasks = new List<LoadingTask>();
            foreach (var task in rootTasks)
                allAtomicTasks.AddRange(task.Flatten());
            int totalTasks = allAtomicTasks.Count;

            if (totalTasks == 0) { onComplete?.Invoke(); return; }

            // 2. 显示 UILoading 窗口
            GameModule.UI.ShowUIAsync<UILoading>();

            var loadingUI = await GameModule.UI.GetUIAsyncAwait<UILoading>();
            if (loadingUI == null)
                throw new GameFrameworkException("UILoading load failed.");

            // 3. 创建进度报告器
            var progress = new Progress<float>(loadingUI.SetProgress);

            // 4. 顺序执行所有原子任务，并在每个任务完成后报告进度
            for (int i = 0; i < allAtomicTasks.Count; i++)
            {
                await allAtomicTasks[i].ExecuteAsync();
                float currentProgress = (i + 1) / (float)totalTasks;
                ((IProgress<float>)progress).Report(currentProgress);
            }

            // 5. 确保目标进度到 1.0，等待平滑动画走完，带超时保护
            loadingUI.SetProgress(1f);
            float elapsed = 0f;
            while (loadingUI.CurrentProgress < 1f)
            {
                await UniTask.Yield();
                elapsed += Time.deltaTime;
                if (elapsed * 1000f >= WaitCompleteTimeoutMs) break;
            }

            onComplete?.Invoke();
            GameModule.UI.CloseUI<UILoading>();
        }
    }
}
