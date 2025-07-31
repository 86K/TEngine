using TMPro;

namespace GameLogic
{
    [Window(UILayer.UI)]
    class UIMain : UIWindow
    {
        #region 脚本工具生成的代码
        private TextMeshProUGUI _tmpNote;
        private TextMeshProUGUI _tmpToDo;
        private TextMeshProUGUI _tmpMemorandum;
        private TextMeshProUGUI _tmpCalendar;
        private TextMeshProUGUI _tmpMe;
        protected override void ScriptGenerator()
        {
            _tmpNote = FindChildComponent<TextMeshProUGUI>("Menu/m_tmpNote");
            _tmpToDo = FindChildComponent<TextMeshProUGUI>("Menu/m_tmpToDo");
            _tmpMemorandum = FindChildComponent<TextMeshProUGUI>("Menu/m_tmpMemorandum");
            _tmpCalendar = FindChildComponent<TextMeshProUGUI>("Menu/m_tmpCalendar");
            _tmpMe = FindChildComponent<TextMeshProUGUI>("Menu/m_tmpMe");
        }
        #endregion

        #region 事件
        #endregion
    }
}