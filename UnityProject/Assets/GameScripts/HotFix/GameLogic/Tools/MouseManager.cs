namespace GameLogic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Events;

    public class MouseClickEventArgs : EventArgs
    {
        public enum HitType
        {
            None,
            UI,
            Collider
        }

        public HitType Type;
        public GameObject HitObject;
        public RaycastHit HitInfo;
        public Vector3 WorldPosition;
        public Vector2 ScreenPosition;
        public int Button;

        public MouseClickEventArgs(HitType type, GameObject obj, RaycastHit hitInfo, Vector3 worldPos, Vector2 screenPos, int button)
        {
            Type = type;
            HitObject = obj;
            HitInfo = hitInfo;
            WorldPosition = worldPos;
            ScreenPosition = screenPos;
            Button = button;
        }
    }

    public class MouseManager : SingletonBehaviour<MouseManager>
    {
        [Header("双击设置")]
        [SerializeField] private float doubleClickTime = 0.3f;
        [SerializeField] private float doubleClickMaxDistance = 30f;

        [Header("射线检测")]
        [SerializeField] private LayerMask raycastLayerMask = ~0;
        [SerializeField] private float maxRayDistance = 1000f;

        // 事件
        public UnityEvent<MouseClickEventArgs> OnLeftClick;
        public UnityEvent<MouseClickEventArgs> OnRightClick;
        public UnityEvent<MouseClickEventArgs> OnLeftDoubleClick;
        public UnityEvent<MouseClickEventArgs> OnRightDoubleClick;
        public UnityEvent<Vector2> OnScroll;
        public UnityEvent<Vector2> OnLeftDrag;
        public UnityEvent<Vector2> OnRightDrag;
        public UnityEvent OnLeftDragEnd;
        public UnityEvent OnRightDragEnd;

        private class ButtonState
        {
            public bool IsPressed;
            public bool IsDragging;
            public float PressTime;
            public Vector2 PressPosition;
            public Vector2 LastMousePosition;

            // 双击检测相关
            public float LastClickTime;
            public Vector2 LastClickPosition;

            // 延迟单击协程
            public Coroutine PendingClickCoroutine;
            public MouseClickEventArgs PendingClickArgs;
        }

        private ButtonState leftState = new ButtonState();
        private ButtonState rightState = new ButtonState();

        private PointerEventData pointerEventData;
        private List<RaycastResult> uiRaycastResults;
        private Camera mainCamera;

        public override void Initialize()
        {
            base.Initialize();

            mainCamera = Camera.main;
            if (mainCamera == null)
                Debug.LogError("MouseManager: 未找到主摄像机");

            uiRaycastResults = new List<RaycastResult>();
            pointerEventData = new PointerEventData(EventSystem.current);

            OnLeftClick ??= new UnityEvent<MouseClickEventArgs>();
            OnRightClick ??= new UnityEvent<MouseClickEventArgs>();
            OnLeftDoubleClick ??= new UnityEvent<MouseClickEventArgs>();
            OnRightDoubleClick ??= new UnityEvent<MouseClickEventArgs>();
            OnScroll ??= new UnityEvent<Vector2>();
            OnLeftDrag ??= new UnityEvent<Vector2>();
            OnRightDrag ??= new UnityEvent<Vector2>();
            OnLeftDragEnd ??= new UnityEvent();
            OnRightDragEnd ??= new UnityEvent();
        }

        private void Update()
        {
            Vector2 scrollDelta = Input.mouseScrollDelta;
            if (scrollDelta != Vector2.zero)
                OnScroll.Invoke(scrollDelta);

            ProcessButton(0, leftState);
            ProcessButton(1, rightState);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            OnLeftClick = null;
            OnLeftDoubleClick = null;
            OnRightClick = null;
            OnRightDoubleClick = null;
            OnLeftDrag = null;
            OnLeftDragEnd = null;
            OnRightDrag = null;
            OnRightDragEnd = null;
            OnScroll = null;
        }

        private void ProcessButton(int button, ButtonState state)
        {
            Vector2 currentMousePos = Input.mousePosition;

            // 按下
            if (Input.GetMouseButtonDown(button))
            {
                // 取消任何待执行的单击协程
                if (state.PendingClickCoroutine != null)
                {
                    StopCoroutine(state.PendingClickCoroutine);
                    state.PendingClickCoroutine = null;
                }

                state.IsPressed = true;
                state.PressTime = Time.time;
                state.PressPosition = currentMousePos;
                state.LastMousePosition = currentMousePos;
                state.IsDragging = false;

                // 检查是否为双击（基于上一次点击的时间/位置）
                float timeSinceLastClick = Time.time - state.LastClickTime;
                float distanceFromLastClick = Vector2.Distance(currentMousePos, state.LastClickPosition);

                bool isDoubleClick = (timeSinceLastClick < doubleClickTime) &&
                                     (distanceFromLastClick < doubleClickMaxDistance);

                if (isDoubleClick)
                {
                    // 触发双击，并清除上一次的单击记录
                    var hitResult = PerformRaycast(currentMousePos);
                    var args = new MouseClickEventArgs(
                        hitResult.type,
                        hitResult.hitObject,
                        hitResult.hitInfo,
                        hitResult.worldPosition,
                        currentMousePos,
                        button
                    );

                    if (button == 0)
                        OnLeftDoubleClick.Invoke(args);
                    else
                        OnRightDoubleClick.Invoke(args);

                    // 重置双击状态，防止后续单击干扰
                    state.LastClickTime = 0f;
                    state.LastClickPosition = Vector2.zero;
                    state.PendingClickArgs = null;
                }
                else
                {
                    // 不是双击，则立即执行上一次悬置的单击（如果存在）
                    if (state.PendingClickArgs != null)
                    {
                        InvokeClickEvent(state.PendingClickArgs, isDoubleClick: false);
                        state.PendingClickArgs = null;
                    }

                    // 记录本次按下的信息，但暂不触发单击
                    state.LastClickTime = Time.time;
                    state.LastClickPosition = currentMousePos;
                }
            }

            // 按住期间处理拖拽
            if (Input.GetMouseButton(button))
            {
                if (state.IsPressed)
                {
                    Vector2 delta = currentMousePos - state.LastMousePosition;
                    if (!state.IsDragging && Vector2.Distance(state.PressPosition, currentMousePos) > 5f)
                    {
                        state.IsDragging = true;
                        // 开始拖拽后，取消任何待执行的单击
                        if (state.PendingClickCoroutine != null)
                        {
                            StopCoroutine(state.PendingClickCoroutine);
                            state.PendingClickCoroutine = null;
                            state.PendingClickArgs = null;
                        }
                    }

                    if (state.IsDragging && delta != Vector2.zero)
                    {
                        if (button == 0)
                            OnLeftDrag.Invoke(delta);
                        else
                            OnRightDrag.Invoke(delta);
                    }

                    state.LastMousePosition = currentMousePos;
                }
            }

            // 抬起
            if (Input.GetMouseButtonUp(button))
            {
                if (state.IsPressed)
                {
                    if (!state.IsDragging)
                    {
                        // 抬起时启动延迟单击协程（除非这次按下已经被当作双击处理了）
                        // 注意：如果是双击情况，上面在按下时已经触发了双击，并清除了待处理参数，这里不应再启动单击
                        if (state.PendingClickArgs == null && state.LastClickTime > 0)
                        {
                            var hitResult = PerformRaycast(currentMousePos);
                            var args = new MouseClickEventArgs(
                                hitResult.type,
                                hitResult.hitObject,
                                hitResult.hitInfo,
                                hitResult.worldPosition,
                                currentMousePos,
                                button
                            );

                            state.PendingClickArgs = args;
                            state.PendingClickCoroutine = StartCoroutine(DelayedClick(button, state, args));
                        }
                    }
                    else
                    {
                        // 拖拽结束
                        if (button == 0)
                            OnLeftDragEnd.Invoke();
                        else
                            OnRightDragEnd.Invoke();
                    }
                }

                state.IsPressed = false;
                state.IsDragging = false;
            }
        }

        private IEnumerator DelayedClick(int button, ButtonState state, MouseClickEventArgs args)
        {
            yield return new WaitForSeconds(doubleClickTime);

            // 超时后仍未被打断，触发单击
            if (state.PendingClickArgs == args)
            {
                InvokeClickEvent(args, isDoubleClick: false);
                state.PendingClickArgs = null;
            }
            state.PendingClickCoroutine = null;
        }

        private void InvokeClickEvent(MouseClickEventArgs args, bool isDoubleClick)
        {
            if (isDoubleClick)
            {
                if (args.Button == 0)
                    OnLeftDoubleClick.Invoke(args);
                else
                    OnRightDoubleClick.Invoke(args);
            }
            else
            {
                if (args.Button == 0)
                    OnLeftClick.Invoke(args);
                else
                    OnRightClick.Invoke(args);
            }
        }

        private (MouseClickEventArgs.HitType type, GameObject hitObject, RaycastHit hitInfo, Vector3 worldPosition)
            PerformRaycast(Vector2 screenPosition)
        {
            // UI 检测
            if (EventSystem.current != null)
            {
                pointerEventData.position = screenPosition;
                uiRaycastResults.Clear();
                EventSystem.current.RaycastAll(pointerEventData, uiRaycastResults);

                if (uiRaycastResults.Count > 0)
                {
                    GameObject uiObj = uiRaycastResults[0].gameObject;
                    Vector3 worldPos = uiObj.transform.position;
                    return (MouseClickEventArgs.HitType.UI, uiObj, default, worldPos);
                }
            }

            // 3D 物理检测
            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, maxRayDistance, raycastLayerMask))
            {
                return (MouseClickEventArgs.HitType.Collider, hitInfo.collider.gameObject, hitInfo, hitInfo.point);
            }

            return (MouseClickEventArgs.HitType.None, null, default, Vector3.zero);
        }
    }
}