using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Fusion
{
    /// <summary>
    /// 解决嵌套 ScrollRect 时拖动冲突的问题。
    /// 应挂载在内层 ScrollRect 上，实现拖动方向判断并动态启用对应的 ScrollRect。
    /// </summary>
    [AddComponentMenu("Tools/Components/Nested ScrollRect")]
    public class NestedScrollRect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Tooltip("外层 ScrollRect。可留空，自动在父级中查找。")]
        [SerializeField] private ScrollRect _parentScrollRect;

        [Tooltip("当前 ScrollRect 是否为纵向滚动。")]
        [SerializeField] private bool _isVertical = true;

        private ScrollRect _currentScrollRect;
        private bool _routingToParent;

        private void Awake()
        {
            _currentScrollRect = GetComponent<ScrollRect>();

            if (_parentScrollRect == null)
            {
                var parents = GetComponentsInParent<ScrollRect>();
                if (parents.Length > 1)
                    _parentScrollRect = parents[1]; // 跳过自己
                else
                    Debug.LogWarning("NestedScrollRect: 未找到父级 ScrollRect。");
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _routingToParent = ShouldRouteToParent(eventData);

            if (_routingToParent)
                _parentScrollRect?.OnBeginDrag(eventData);
            else
                _currentScrollRect?.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_routingToParent)
                _parentScrollRect?.OnDrag(eventData);
            else
                _currentScrollRect?.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_routingToParent)
                _parentScrollRect?.OnEndDrag(eventData);
            else
                _currentScrollRect?.OnEndDrag(eventData);

            _routingToParent = false;
        }

        /// <summary>
        /// 判断当前拖动是否应交由父级 ScrollRect 处理
        /// </summary>
        private bool ShouldRouteToParent(PointerEventData eventData)
        {
            Vector2 delta = eventData.delta;
            if (_isVertical)
                return Mathf.Abs(delta.x) > Mathf.Abs(delta.y); 
            
            return Mathf.Abs(delta.y) > Mathf.Abs(delta.x); 
        }
    }
}
