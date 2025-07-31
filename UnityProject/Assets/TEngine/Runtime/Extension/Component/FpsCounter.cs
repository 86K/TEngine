using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Fusion
{
    [AddComponentMenu("Tools/Components/Fps Counter")]
    public class FpsCounter : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Text _fpsText;
        [SerializeField] private TextMeshProUGUI _tmpText;

        [Header("Settings")]
        [SerializeField] private float _updateInterval = 0.5f;
        [SerializeField] private int _maxSamples = 100;

        private int _frameCount;
        private float _timeAccumulator;
        private float _timeLeft;
        private int _currentFps;
        private float _refreshRate;

        private readonly Queue<int> _fpsSamples = new();

        private void Awake()
        {
            DontDestroyOnLoad(this);
            _refreshRate = Screen.currentResolution.refreshRate;
            _timeLeft = _updateInterval;
        }

        private void Update()
        {
            _frameCount++;
            _timeAccumulator += Time.deltaTime;
            _timeLeft -= Time.unscaledDeltaTime;

            if (_timeLeft <= 0f)
            {
                _currentFps = _timeAccumulator > 0f ? (int)(_frameCount / _timeAccumulator) : 0;
                _fpsSamples.Enqueue(_currentFps);
                if (_fpsSamples.Count > _maxSamples)
                    _fpsSamples.Dequeue();

                _timeAccumulator = 0f;
                _frameCount = 0;
                _timeLeft = _updateInterval;

                string text = $"FPS: {_currentFps} / {_refreshRate}Hz";
                if (_fpsText) _fpsText.text = text;
                if (_tmpText) _tmpText.text = text;
            }
        }

        private void OnGUI()
        {
            if (_fpsText != null || _tmpText != null)
                return;

            GUIStyle style = new GUIStyle(GUI.skin.box)
            {
                fontSize = 15,
                alignment = TextAnchor.MiddleCenter
            };

            float width = 120;
            float height = 20;
            float x = 5;
            float y = Screen.height - height - 5;

            GUI.color = Color.green;
            GUI.Box(new Rect(x, y, width, height), $"FPS: {_currentFps} / {_refreshRate}Hz", style);

            DrawGraph();
        }

        private void DrawGraph()
        {
            if (_fpsSamples.Count < 2)
                return;

            float graphWidth = 120;
            float graphHeight = 40;
            float startX = 5;
            float startY = Screen.height - 70;

            Vector2 prev = Vector2.zero;
            int[] fpsArray = _fpsSamples.ToArray();
            for (int i = 0; i < fpsArray.Length; i++)
            {
                float x = startX + i * (graphWidth / _maxSamples);
                float y = startY + graphHeight - (fpsArray[i] / _refreshRate) * graphHeight;
                Vector2 curr = new(x, y);

                if (i > 0)
                    Drawing.DrawLine(prev, curr, Color.cyan, 1);

                prev = curr;
            }

            GUI.color = Color.white;
            GUI.Label(new Rect(startX, startY - 15, 100, 15), "FPS Curve");
        }
    }

    /// <summary>
    /// 简单线条绘制（使用GL实现，需放在OnPostRender或类似方法中才能正常运行）
    /// </summary>
    public static class Drawing
    {
        private static Texture2D _lineTex;

        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
        {
            if (_lineTex == null)
            {
                _lineTex = new Texture2D(1, 1);
                _lineTex.SetPixel(0, 0, Color.white);
                _lineTex.Apply();
            }

            Matrix4x4 matrix = GUI.matrix;
            Color savedColor = GUI.color;

            float angle = Vector3.Angle(pointB - pointA, Vector2.right);
            if (pointA.y > pointB.y) angle = -angle;

            float length = Vector3.Distance(pointA, pointB);
            GUI.color = color;
            GUIUtility.RotateAroundPivot(angle, pointA);
            GUI.DrawTexture(new Rect(pointA.x, pointA.y, length, width), _lineTex);
            GUI.matrix = matrix;
            GUI.color = savedColor;
        }
    }
}
