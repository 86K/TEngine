using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace GameLogic
{
    /// <summary>
    /// 加载任务包装器。用于描述单个或一组异步加载任务，并支持进度追踪。
    /// </summary>
    public sealed class LoadingTask
    {
        private readonly string _taskName;
        private readonly Func<UniTask> _taskFunc;
        private readonly List<LoadingTask> _subTasks;

        // 私有构造函数，强制通过静态方法创建
        private LoadingTask(string taskName, Func<UniTask> taskFunc, List<LoadingTask> subTasks)
        {
            _taskName = taskName;
            _taskFunc = taskFunc;
            _subTasks = subTasks;
        }

        /// <summary>
        /// 创建一个普通的异步任务。
        /// </summary>
        /// <param name="taskName">任务名称（用于调试）。</param>
        /// <param name="taskFunc">返回一个 UniTask 的异步方法。</param>
        /// <returns>LoadingTask 实例。</returns>
        public static LoadingTask Create(string taskName, Func<UniTask> taskFunc)
        {
            return new LoadingTask(taskName, taskFunc, null);
        }

        /// <summary>
        /// 创建一个包含子任务集合的父任务。
        /// </summary>
        /// <param name="taskName">任务名称（用于调试）。</param>
        /// <param name="subTasks">子任务列表。</param>
        /// <returns>LoadingTask 实例。</returns>
        public static LoadingTask CreateGroup(string taskName, List<LoadingTask> subTasks)
        {
            return new LoadingTask(taskName, null, subTasks);
        }

        /// <summary>
        /// 递归地将任务树“展平”为一个包含所有底层任务的列表。
        /// </summary>
        /// <returns>所有底层任务的列表。</returns>
        public List<LoadingTask> Flatten()
        {
            var flatList = new List<LoadingTask>();
            FlattenInternal(this, flatList);
            return flatList;
        }

        private void FlattenInternal(LoadingTask task, List<LoadingTask> flatList)
        {
            if (task._subTasks != null)
            {
                // 如果这是一个任务组，则递归地展平它的所有子任务
                foreach (var subTask in task._subTasks)
                {
                    FlattenInternal(subTask, flatList);
                }
            }
            else
            {
                // 如果这是一个普通任务，则直接将其添加到列表中
                flatList.Add(task);
            }
        }

        /// <summary>
        /// 执行当前任务。
        /// </summary>
        /// <returns>UniTask。</returns>
        /// <exception cref="InvalidOperationException">如果尝试直接执行一个任务组。</exception>
        public async UniTask ExecuteAsync()
        {
            if (_taskFunc == null)
            {
                // 任务组不应该被直接执行
                throw new InvalidOperationException($"Cannot execute a task group '{_taskName}' directly.");
            }
            await _taskFunc();
        }

        // 用于调试
        public override string ToString() => _taskName;
    }
}