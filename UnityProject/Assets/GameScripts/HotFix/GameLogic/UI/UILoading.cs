using UnityEngine;

namespace GameLogic
{
	[Window(UILayer.UI, location : "UILoading")]
	public partial class UILoading
	{
        protected override void OnCreate()
        {
            base.OnCreate();
            
            SetProgress(0);
        }

        public void SetProgress(float progress)
        {
            // 将进度钳制在0到1之间
            m_sliderPrecent.value = Mathf.Clamp01(progress);
            m_tmpPrecent.text = (progress * 100f).ToString("F1") + "%";
        }
        
		#region 事件

		private partial void OnSliderPrecentChange(float value)
		{
		}

		#endregion
	}
}
