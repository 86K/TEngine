using Fantasy;
using Fantasy.Event;
using Fantasy.Timer;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// Fantasy的全局管理器。
    /// </summary>
    public class FantasyManager : SingletonBehaviour<FantasyManager>
    {
        /// <summary>
        /// 事件管理器。
        /// </summary>
        public EventComponent Event => Fantasy.Runtime.Scene.EventComponent;
        
        /// <summary>
        /// 网络调度定时器。
        /// </summary>
        public TimerSchedulerNet NetSchedulerTimer => Fantasy.Runtime.Scene.TimerComponent.Net;
        
        /// <summary>
        /// Unity时间轮的调度定时器。
        /// </summary>
        public TimerSchedulerNetUnity UnitySchedulerTimer => Fantasy.Runtime.Scene.TimerComponent.Unity;
        
        public override void Initialize()
        {
            base.Initialize();
            
            // 通过添加FantasyRuntime组件的方式，初始化Fantasy框架，
            // 而不是await Fantasy.Platform.Unity.Entry.Initialize();再去创建scene和session。
            // 启用FantasyRuntime作为Instance，绑定到本管理器下。
            var fantasyRuntime = new GameObject("FantasyRuntime").AddComponent<FantasyRuntime>();
            fantasyRuntime.transform.SetParent(transform);
            fantasyRuntime.isRuntimeInstance = true;
            
            // 绑定服务端ip、port和协议类型
            fantasyRuntime.remoteIP = "127.0.0.1";
            fantasyRuntime.remotePort = 11001;
            fantasyRuntime.protocol = FantasyRuntime.NetworkProtocolType.KCP;
        }
    }
}