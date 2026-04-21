using TEngine;

namespace GameLogic
{
	[Window(UILayer.UI, location : "UITip")]
	public partial class UITip
	{
        private int _autoCloseTimerId;

        protected override void OnCreate()
        {
            base.OnCreate();
            RefreshTip();
        }

        protected override void OnRefresh()
        {
            base.OnRefresh();
            RefreshTip();
        }

        protected override void OnDestroy()
        {
            CancelAutoCloseTimer();
            base.OnDestroy();
        }

        private void RefreshTip()
        {
            var content = (string)UserDatas[0];
            m_tmpContent.text = content;
            GameEvent.Send(EventId.AutoRefresh);

            int delayCloseSeconds = 2;
            if (UserDatas.Length == 2)
            {
                delayCloseSeconds = (int)UserDatas[1];
            }
            CancelAutoCloseTimer();
            if (!string.IsNullOrEmpty(content) && delayCloseSeconds > 0)
            {
                _autoCloseTimerId = GameModule.Timer.AddTimer(_ => Close(), delayCloseSeconds);
            }
        }

        private void CancelAutoCloseTimer()
        {
            if (_autoCloseTimerId > 0)
            {
                GameModule.Timer.RemoveTimer(_autoCloseTimerId);
                _autoCloseTimerId = 0;
            }
        }
    }
}
