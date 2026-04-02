using TMPro;
using Cysharp.Threading.Tasks;
using Fantasy.Event;
using UnityEngine;
using UnityEngine.UI;
using TEngine;

namespace GameLogic
{
    /// <summary>
    /// 定义一个结构体的测试事件
    /// </summary>
    public struct TestEvent
    {
        public string Test;
    }

    /// <summary>
    /// 订阅该结构体类型的测试事件
    /// </summary>
    public class OnTestEvent : EventSystem<TestEvent>
    {
        protected override void Handler(TestEvent self)
        {
            Log.Info($"接收到了TestEvent事件：{self.Test}");
        }
    }
    
	[Window(UILayer.UI, location : "UILogin")]
	public partial class UILogin
	{
        protected override void OnCreate()
        {
            base.OnCreate();
            
            // 测试fantasy的定时器
            // 1.Net定时器
            FantasyManager.Instance.NetSchedulerTimer.OnceTimer(1500, () =>
            {
                Log.Info($"测试Fantasy的Net定时器，1.5秒后执行一次");
            });

            // 2.Unity定时器
            FantasyManager.Instance.UnitySchedulerTimer.RepeatedTimer(2000, () =>
            {
                Log.Info($"测试Fantasy的Unity定时器，每隔2秒后执行一次");
            });
            
            // 测试fantasy的事件系统
            FantasyManager.Instance.Event.Publish(new TestEvent()
            {
                Test = "Hello World!"
            });
        }

        #region 事件

		private async partial UniTaskVoid OnClickLoginBtn()
        {
            var account = m_tmpInputAccount.text;
            var password = m_tmpInputPassword.text;

            if (string.IsNullOrEmpty(account))
            {
                
            }
            
			await UniTask.Yield();
		}

		#endregion
	}
}
