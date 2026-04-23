using UnityEngine;

namespace GameLogic
{
    public class TestMouseManager : MonoBehaviour
    {
        private void Awake()
        {
            MouseManager.Instance.onLeftClick.AddListener(HandleLeftClick);
            MouseManager.Instance.onLeftDoubleClick.AddListener(HandleLeftDoubleClick);

            MouseManager.Instance.onLeftDrag.AddListener(HandleLeftDrag);
            MouseManager.Instance.onLeftDragEnd.AddListener(HandleLeftDragEnd);

            MouseManager.Instance.onRightClick.AddListener(HandleRightClick);
            MouseManager.Instance.onRightDoubleClick.AddListener(HandleRightDoubleClick);

            MouseManager.Instance.onRightDrag.AddListener(HandleRightDrag);
            MouseManager.Instance.onRightDragEnd.AddListener(HandleRightDragEnd);
            
            MouseManager.Instance.onScroll.AddListener(HandleScroll);
        }

        // private void OnDestroy()
        // {
        //     MouseManager.Instance.onLeftClick.RemoveListener(HandleLeftClick);
        //     MouseManager.Instance.onLeftDoubleClick.RemoveListener(HandleLeftDoubleClick);
        //
        //     MouseManager.Instance.onLeftDrag.RemoveListener(HandleLeftDrag);
        //     MouseManager.Instance.onLeftDragEnd.RemoveListener(HandleLeftDragEnd);
        //
        //     MouseManager.Instance.onRightClick.RemoveListener(HandleRightClick);
        //     MouseManager.Instance.onRightDoubleClick.RemoveListener(HandleRightDoubleClick);
        //
        //     MouseManager.Instance.onRightDrag.RemoveListener(HandleRightDrag);
        //     MouseManager.Instance.onRightDragEnd.RemoveListener(HandleRightDragEnd);
        //     
        //     MouseManager.Instance.onScroll.RemoveListener(HandleScroll);
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
