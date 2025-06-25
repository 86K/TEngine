using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TEngine;

namespace GameLogic
{
    [Window(UILayer.UI)]
    class ModuleMenuUI : UIWindow
    {
        #region 脚本工具生成的代码
        private Button _btnExitSystem;
        protected override void ScriptGenerator()
        {
            _btnExitSystem = FindChildComponent<Button>("m_btnExitSystem");
            _btnExitSystem.onClick.AddListener(UniTask.UnityAction(OnClickExitSystemBtn));
        }
        #endregion

        #region 事件
        private async UniTaskVoid OnClickExitSystemBtn()
        {
            await UniTask.Yield();
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        #endregion

    }
}