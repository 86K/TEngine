using System;
using System.IO;
using UnityEngine;

namespace TEngine
{
    /// <summary>
    /// 屏幕截图器。
    /// </summary>
    [AddComponentMenu("Tools/Components/Screen Capturer")]
    public class ScreenCapturer : MonoBehaviour
    {
        [Tooltip("按下此键进行截图")]
        [SerializeField] private KeyCode _captureKey = KeyCode.C;

        private string _folderPath;

        private void Awake()
        {
            _folderPath = Path.Combine(Application.persistentDataPath, "ScreenCaptures");
            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
            }
            else
            {
                Debug.LogWarning($"注意：屏幕截图的保存地址'{_folderPath}'已存在！请检查是否存在占用情况。");
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(_captureKey))
            {
                CaptureScreenshot();
            }
        }

        private void CaptureScreenshot()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"Screenshot_{timestamp}.jpg";
            string filePath = Path.Combine(_folderPath, fileName);

            try
            {
                ScreenCapture.CaptureScreenshot(filePath, ScreenCapture.StereoScreenCaptureMode.BothEyes);
                Debug.Log($"Screenshot saved to: {filePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Screenshot failed: {e.Message}");
            }
        }
    }
}