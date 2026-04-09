using UnityEngine;

namespace GameLogic
{
    public class TestMouseManager : MonoBehaviour
    {
        private void Awake()
        {
            MouseManager.Instance.OnLeftClick.AddListener(HandleLeftClick);
            MouseManager.Instance.OnLeftDoubleClick.AddListener(HandleLeftDoubleClick);

            MouseManager.Instance.OnLeftDrag.AddListener(HandleLeftDrag);
            MouseManager.Instance.OnLeftDragEnd.AddListener(HandleLeftDragEnd);

            MouseManager.Instance.OnRightClick.AddListener(HandleRightClick);
            MouseManager.Instance.OnRightDoubleClick.AddListener(HandleRightDoubleClick);

            MouseManager.Instance.OnRightDrag.AddListener(HandleRightDrag);
            MouseManager.Instance.OnRightDragEnd.AddListener(HandleRightDragEnd);
            
            MouseManager.Instance.OnScroll.AddListener(HandleScroll);
        }

        // private void OnDestroy()
        // {
        //     MouseManager.Instance.OnLeftClick.RemoveListener(HandleLeftClick);
        //     MouseManager.Instance.OnLeftDoubleClick.RemoveListener(HandleLeftDoubleClick);
        //
        //     MouseManager.Instance.OnLeftDrag.RemoveListener(HandleLeftDrag);
        //     MouseManager.Instance.OnLeftDragEnd.RemoveListener(HandleLeftDragEnd);
        //
        //     MouseManager.Instance.OnRightClick.RemoveListener(HandleRightClick);
        //     MouseManager.Instance.OnRightDoubleClick.RemoveListener(HandleRightDoubleClick);
        //
        //     MouseManager.Instance.OnRightDrag.RemoveListener(HandleRightDrag);
        //     MouseManager.Instance.OnRightDragEnd.RemoveListener(HandleRightDragEnd);
        //     
        //     MouseManager.Instance.OnScroll.RemoveListener(HandleScroll);
        // }

        private void HandleLeftClick(MouseClickEventArgs args)
        {
            Debug.Log($"左键单击, 物体: {(args.HitObject ? args.HitObject.name : "无")}");
        }

        private void HandleLeftDoubleClick(MouseClickEventArgs args)
        {
            Debug.Log($"左键双击, 物体: {(args.HitObject ? args.HitObject.name : "无")}");
        }
        
        private void HandleLeftDrag(Vector2 delta)
        {
            // 拖拽时的增量位移（屏幕像素）
            Debug.Log($"左键按住拖动：{delta}");
        }
        
        private void HandleLeftDragEnd()
        {
            Debug.Log($"左键停止拖动");
        }
        
        private void HandleScroll(Vector2 delta)
        {
            Debug.Log($"滚轮滚动: {delta}");
        }

        private void HandleRightClick(MouseClickEventArgs args)
        {
            Debug.Log($"右键单击, 物体: {(args.HitObject ? args.HitObject.name : "无")}");
        }
        
        private void HandleRightDoubleClick(MouseClickEventArgs args)
        {
            Debug.Log($"右键双击, 物体: {(args.HitObject ? args.HitObject.name : "无")}");
        }
        
        private void HandleRightDrag(Vector2 delta)
        {
            Debug.Log($"右键按住拖动：{delta}");
        }
        
        private void HandleRightDragEnd()
        {
            Debug.Log($"右键停止拖动");
        } 
    }
}