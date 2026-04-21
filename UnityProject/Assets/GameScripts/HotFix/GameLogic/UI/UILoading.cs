using UnityEngine;

namespace GameLogic
{
	[Window(UILayer.UI, location : "UILoading")]
	public partial class UILoading
	{
        private const float SmoothSpeed = 1.5f;

        private float _targetProgress;
        private float _currentProgress;

        public float CurrentProgress => _currentProgress;

        protected override void OnCreate()
        {
            base.OnCreate();
            _targetProgress = 0f;
            _currentProgress = 0f;
            RefreshUI(0f);
        }

        public void SetProgress(float progress)
        {
            _targetProgress = Mathf.Clamp01(progress);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (_currentProgress >= _targetProgress) return;
            _currentProgress = Mathf.MoveTowards(_currentProgress, _targetProgress, SmoothSpeed * Time.deltaTime);
            RefreshUI(_currentProgress);
        }

        private void RefreshUI(float value)
        {
            m_sliderPrecent.value = value;
            m_tmpPrecent.text = (value * 100f).ToString("F1") + "%";
        }

		#region 事件

		private partial void OnSliderPrecentChange(float value)
		{
		}

		#endregion
	}
}
