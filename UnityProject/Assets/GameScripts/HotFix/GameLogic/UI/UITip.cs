using TEngine;

namespace GameLogic
{
	[Window(UILayer.UI, location : "UITip")]
	public partial class UITip
	{
        protected override void OnCreate()
        {
            base.OnCreate();

            var content = (string)UserDatas[0];
            m_tmpContent.text = content;
            GameEvent.Send(EventId.AutoRefresh);

            int delayCloseSeconds = 2;
            if (UserDatas.Length == 2)
            {
                delayCloseSeconds = (int)UserDatas[1];
            }
            int delayCloseMilliseconds = string.IsNullOrEmpty(content) ? 0 : delayCloseSeconds * 1000;
            FantasyManager.Instance.UnitySchedulerTimer.OnceTimer(delayCloseMilliseconds, Close);
        }
    }
}
