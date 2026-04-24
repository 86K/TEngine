using TEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    /// <summary>
    /// 底图自适应文字内容。
    /// </summary>
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("Extensions/UI/Image Adapts Text Size")]
    public class ImageAdaptsTextSize : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;

        private RectTransform _imageRect;
        private RectTransform _textRect;
        private float _imageWidth;

        [SerializeField] private float _space = 5;

        private void Awake()
        {
            _imageRect = _image.GetComponent<RectTransform>();
            _imageWidth= _imageRect.sizeDelta.x;
            _textRect = _text.GetComponent<RectTransform>();
            
            GameEvent.AddEventListener(EventId.AutoRefresh, Refresh);
        }

        private void OnDestroy()
        {
            GameEvent.RemoveEventListener(EventId.AutoRefresh, Refresh);
        }

        private void Refresh()
        {
            _text.ForceMeshUpdate();
            float contentHeight = _text.GetPreferredValues(_text.text, _textRect.rect.width, Mathf.Infinity).y;
            _imageRect.sizeDelta = new Vector2(_imageWidth, contentHeight + _space);
        }
    }
}