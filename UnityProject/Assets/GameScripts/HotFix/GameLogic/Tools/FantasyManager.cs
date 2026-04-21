using Fantasy;
using Fantasy.Async;
using Fantasy.Event;
using Fantasy.Network;
using Fantasy.Timer;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// Fantasy的全局管理器。
    /// </summary>
    public class FantasyManager : SingletonBehaviour<FantasyManager>
    {
        private FantasyRuntime _fantasyRuntime;

        public Scene Scene => Fantasy.Runtime.Scene;
        public Session Session => Fantasy.Runtime.Session;
        
        /// <summary>
        /// 网络调度定时器。
        /// </summary>
        public TimerSchedulerNet NetSchedulerTimer => Fantasy.Runtime.Scene.TimerComponent.Net;
        
        /// <summary>
        /// Unity时间轮的调度定时器。
        /// </summary>
        public TimerSchedulerNetUnity UnitySchedulerTimer => Fantasy.Runtime.Scene.TimerComponent.Unity;
        
        /// <summary>
        /// 事件组件。
        /// </summary>
        public EventComponent Event => Fantasy.Runtime.Scene.EventComponent;
        
        /// <summary>
        /// 
        /// </summary>
        public CoroutineLockComponent CoroutineLock => Fantasy.Runtime.Scene.CoroutineLockComponent;
        
        public override void Initialize()
        {
            base.Initialize();

            if (_fantasyRuntime != null)
            {
                return;
            }
             
            // 通过添加FantasyRuntime组件的方式，初始化Fantasy框架，
            // 而不是await Fantasy.Platform.Unity.Entry.Initialize();再去创建scene和session。
            // 启用FantasyRuntime作为Instance，绑定到本管理器下。
            _fantasyRuntime = GetComponentInChildren<FantasyRuntime>();
            if (_fantasyRuntime == null)
            {
                _fantasyRuntime = new GameObject("FantasyRuntime").AddComponent<FantasyRuntime>();
                _fantasyRuntime.transform.SetParent(transform);
            }
            _fantasyRuntime.isRuntimeInstance = true;
            
            // 绑定服务端ip、port和协议类型
            _fantasyRuntime.remoteIP = GameManager.Config.remoteIp;
            _fantasyRuntime.remotePort = GameManager.Config.remotePort;
            _fantasyRuntime.protocol = FantasyRuntime.NetworkProtocolType.KCP;
            
            // 注：通过FantasyRuntime去初始化框架和连接服务器需要一定的时间。
            // 尽量在第一个界面显示出来之后去使用。
        } 
    }
}
