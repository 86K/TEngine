using Cysharp.Threading.Tasks;
using Fantasy;
using UnityEngine;
using Log = TEngine.Log;

namespace GameLogic
{
	[Window(UILayer.UI, location : "UILogin")]
	public partial class UILogin
	{
        protected override void OnCreate()
        {
            base.OnCreate();
            
            // NOTE：在第一个界面打开之后再去使用各种全局管理器。
            // 这个时候FantasyRuntime已经初始化框架了，可以使用它的Instance了
            Log.Info($"{Fantasy.Runtime.Session.RemoteEndPoint} ");
            
            // 测试MouseManager
            GameManager.Instance.gameObject.AddComponent<TestMouseManager>();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameModule.UI.ShowUIAsync<UITip>("测试UITip功能，所以要凑一段很长的文字内容，去测试它的自适应文字内容，动态适应底图的高度，哈哈哈", 2);
            }
        }

        #region 事件

		private async partial UniTaskVoid OnClickLoginBtn()
        {
            var account = m_tmpInputAccount.text;
            var password = m_tmpInputPassword.text;

            if (string.IsNullOrEmpty(account))
            {
                GameModule.UI.ShowUIAsync<UITip>("账号不能为空");
                return;
            }
            
            if (string.IsNullOrEmpty(password))
            {
                GameModule.UI.ShowUIAsync<UITip>("密码不能为空");
                return;
            }
            
            var loginResponse = await FantasyManager.Instance.Session.C2G_LoginRequest(account, password);
            Log.Info($"登陆结果，code：{loginResponse.code} result：{loginResponse.result}  id：{loginResponse.userId}");
			await UniTask.Yield();
		}

		#endregion
	}
}
