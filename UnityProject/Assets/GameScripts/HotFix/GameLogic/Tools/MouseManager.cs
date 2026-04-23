using UnityEngine.Serialization;

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
        [HideInInspector] public UnityEvent<MouseClickEventArgs> onLeftClick;
        [HideInInspector] public UnityEvent<MouseClickEventArgs> onRightClick;
        [HideInInspector] public UnityEvent<MouseClickEventArgs> onLeftDoubleClick;
        [HideInInspector] public UnityEvent<MouseClickEventArgs> onRightDoubleClick;
        [HideInInspector] public UnityEvent<Vector2> onScroll;
        [HideInInspector] public UnityEvent<Vector2> onLeftDrag;
        [HideInInspector] public UnityEvent<Vector2> onRightDrag;
        [HideInInspector] public UnityEvent onLeftDragEnd;
        [HideInInspector] public UnityEvent onRightDragEnd;

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

        private readonly ButtonState _leftState = new ButtonState();
        private readonly ButtonState _rightState = new ButtonState();

        private PointerEventData _pointerEventData;
        private List<RaycastResult> _uiRaycastResults;
        private Camera _mainCamera;

        public bool IsEnabled { get; private set; } = true;

        public override void Initialize()
        {
            base.Initialize();

            RefreshMainCamera();

            _uiRaycastResults = new List<RaycastResult>();
            _pointerEventData = new PointerEventData(EventSystem.current);

            onLeftClick ??= new UnityEvent<MouseClickEventArgs>();
            onRightClick ??= new UnityEvent<MouseClickEventArgs>();
            onLeftDoubleClick ??= new UnityEvent<MouseClickEventArgs>();
            onRightDoubleClick ??= new UnityEvent<MouseClickEventArgs>();
            onScroll ??= new UnityEvent<Vector2>();
            onLeftDrag ??= new UnityEvent<Vector2>();
            onRightDrag ??= new UnityEvent<Vector2>();
            onLeftDragEnd ??= new UnityEvent();
            onRightDragEnd ??= new UnityEvent();
        }

        private void Update()
        {
            if (!IsEnabled) return;

            if (_mainCamera == null || !_mainCamera.isActiveAndEnabled)
            {
                RefreshMainCamera();
            }

            Vector2 scrollDelta = Input.mouseScrollDelta;
            if (scrollDelta != Vector2.zero)
                onScroll.Invoke(scrollDelta);

            ProcessButton(0, _leftState);
            ProcessButton(1, _rightState);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        
            onLeftClick = null;
            onLeftDoubleClick = null;
            onRightClick = null;
            onRightDoubleClick = null;
            onLeftDrag = null;
            onLeftDragEnd = null;
            onRightDrag = null;
            onRightDragEnd = null;
            onScroll = null;
        }

        /// <summary>
        /// 启用监听鼠标事件。
        /// </summary>
        public void Enable()
        {
            IsEnabled = true;
        }

        /// <summary>
        /// 关闭监听鼠标事件。
        /// </summary>
        public void Disable()
        {
            IsEnabled = false;
            CancelButtonState(_leftState);
            CancelButtonState(_rightState);
        }

        private void CancelButtonState(ButtonState state)
        {
            if (state.PendingClickCoroutine != null)
            {
                StopCoroutine(state.PendingClickCoroutine);
                state.PendingClickCoroutine = null;
            }
            if (state.IsDragging)
            {
                if (state == _leftState)
                    onLeftDragEnd?.Invoke();
                else
                    onRightDragEnd?.Invoke();
            }
            state.IsPressed = false;
            state.IsDragging = false;
            state.PendingClickArgs = null;
        }

        private void RefreshMainCamera()
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                Debug.LogWarning("MouseManager: 当前场景未找到主摄像机");
            }
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
                        onLeftDoubleClick.Invoke(args);
                    else
                        onRightDoubleClick.Invoke(args);

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
                            onLeftDrag.Invoke(delta);
                        else
                            onRightDrag.Invoke(delta);
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
                            state.PendingClickCoroutine = StartCoroutine(DelayedClick(state, args));
                        }
                    }
                    else
                    {
                        // 拖拽结束
                        if (button == 0)
                            onLeftDragEnd.Invoke();
                        else
                            onRightDragEnd.Invoke();
                    }
                }

                state.IsPressed = false;
                state.IsDragging = false;
            }
        }

        private IEnumerator DelayedClick(ButtonState state, MouseClickEventArgs args)
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
                    onLeftDoubleClick.Invoke(args);
                else
                    onRightDoubleClick.Invoke(args);
            }
            else
            {
                if (args.Button == 0)
                    onLeftClick.Invoke(args);
                else
                    onRightClick.Invoke(args);
            }
        }

        private (MouseClickEventArgs.HitType type, GameObject hitObject, RaycastHit hitInfo, Vector3 worldPosition)
            PerformRaycast(Vector2 screenPosition)
        {
            // UI 检测
            if (EventSystem.current != null)
            {
                _pointerEventData.position = screenPosition;
                _uiRaycastResults.Clear();
                EventSystem.current.RaycastAll(_pointerEventData, _uiRaycastResults);

                if (_uiRaycastResults.Count > 0)
                {
                    GameObject uiObj = _uiRaycastResults[0].gameObject;
                    Vector3 worldPos = uiObj.transform.position;
                    return (MouseClickEventArgs.HitType.UI, uiObj, default, worldPos);
                }
            }

            // 3D 物理检测
            if (_mainCamera == null)
            {
                return (MouseClickEventArgs.HitType.None, null, default, Vector3.zero);
            }

            Ray ray = _mainCamera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, maxRayDistance, raycastLayerMask))
            {
                return (MouseClickEventArgs.HitType.Collider, hitInfo.collider.gameObject, hitInfo, hitInfo.point);
            }

            return (MouseClickEventArgs.HitType.None, null, default, Vector3.zero);
        }
    }
}
