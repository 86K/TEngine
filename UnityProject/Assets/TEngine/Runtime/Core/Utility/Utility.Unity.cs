using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Internal;

namespace TEngine
{
    public static partial class Utility
    {
        /// <summary>
        /// Unity相关的实用函数。
        /// </summary>
        public static partial class Unity
        {
            private static IUpdateDriver _updateDriver;

            #region 控制协程Coroutine

            public static GameCoroutine StartCoroutine(string name, IEnumerator routine, MonoBehaviour bindBehaviour)
            {
                if (bindBehaviour == null)
                {
                    Log.Error("StartCoroutine {0} failed, bindBehaviour is null", name);
                    return null;
                }

                var behaviour = bindBehaviour;
                return StartCoroutine(behaviour, name, routine);
            }

            public static GameCoroutine StartCoroutine(string name, IEnumerator routine, GameObject bindGo)
            {
                if (bindGo == null)
                {
                    Log.Error("StartCoroutine {0} failed, BindGo is null", name);
                    return null;
                }

                var behaviour = GetDefaultBehaviour(bindGo);
                return StartCoroutine(behaviour, name, routine);
            }

            public static GameCoroutine StartGlobalCoroutine(string name, IEnumerator routine)
            {
                var coroutine = StartCoroutine(routine);
                var gameCoroutine = new GameCoroutine();
                gameCoroutine.Coroutine = coroutine;
                gameCoroutine.Name = name;
                gameCoroutine.BindBehaviour = null;
                return gameCoroutine;
            }

            public static void StopCoroutine(GameCoroutine coroutine)
            {
                if (coroutine.Coroutine != null)
                {
                    var behaviour = coroutine.BindBehaviour;
                    if (behaviour != null)
                    {
                        behaviour.StopCoroutine(coroutine.Coroutine);
                    }

                    coroutine.Coroutine = null;
                    coroutine.BindBehaviour = null;
                }
            }

            private static GameCoroutine StartCoroutine(MonoBehaviour behaviour, string name, IEnumerator routine)
            {
                var coroutine = behaviour.StartCoroutine(routine);
                var gameCoroutine = new GameCoroutine();
                gameCoroutine.Coroutine = coroutine;
                gameCoroutine.Name = name;
                gameCoroutine.BindBehaviour = behaviour;
                return gameCoroutine;
            }

            private static GameCoroutineAgent GetDefaultBehaviour(GameObject bindGameObject)
            {
                if (bindGameObject != null)
                {
                    if (bindGameObject.TryGetComponent(out GameCoroutineAgent coroutineBehaviour))
                    {
                        return coroutineBehaviour;
                    }

                    return bindGameObject.AddComponent<GameCoroutineAgent>();
                }

                return null;
            }


            public static Coroutine StartCoroutine(string methodName)
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    return null;
                }

                _MakeEntity();
                return _updateDriver.StartCoroutine(methodName);
            }

            public static Coroutine StartCoroutine(IEnumerator routine)
            {
                if (routine == null)
                {
                    return null;
                }

                _MakeEntity();
                return _updateDriver.StartCoroutine(routine);
            }

            public static Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    return null;
                }

                _MakeEntity();
                return _updateDriver.StartCoroutine(methodName, value);
            }

            public static void StopCoroutine(string methodName)
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    return;
                }

                _MakeEntity();
                _updateDriver.StopCoroutine(methodName);
            }

            public static void StopCoroutine(IEnumerator routine)
            {
                if (routine == null)
                {
                    return;
                }

                _MakeEntity();
                _updateDriver.StopCoroutine(routine);
            }

            public static void StopCoroutine(Coroutine routine)
            {
                if (routine == null)
                {
                    return;
                }

                _MakeEntity();
                _updateDriver.StopCoroutine(routine);
                routine = null;
            }

            public static void StopAllCoroutines()
            {
                _MakeEntity();
                _updateDriver.StopAllCoroutines();
            }

            #endregion

            #region 注入UnityUpdate/FixedUpdate/LateUpdate

            /// <summary>
            /// 为给外部提供的 添加帧更新事件。
            /// </summary>
            /// <param name="fun"></param>
            public static void AddUpdateListener(Action fun)
            {
                _MakeEntity();
                AddUpdateListenerImp(fun).Forget();
            }

            private static async UniTaskVoid AddUpdateListenerImp(Action fun)
            {
                await UniTask.Yield( /*PlayerLoopTiming.LastPreUpdate*/);
                _updateDriver.AddUpdateListener(fun);
            }

            /// <summary>
            /// 为给外部提供的 添加物理帧更新事件。
            /// </summary>
            /// <param name="fun"></param>
            public static void AddFixedUpdateListener(Action fun)
            {
                _MakeEntity();
                AddFixedUpdateListenerImp(fun).Forget();
            }

            private static async UniTaskVoid AddFixedUpdateListenerImp(Action fun)
            {
                await UniTask.Yield(PlayerLoopTiming.LastEarlyUpdate);
                _updateDriver.AddFixedUpdateListener(fun);
            }

            /// <summary>
            /// 为给外部提供的 添加Late帧更新事件。
            /// </summary>
            /// <param name="fun"></param>
            public static void AddLateUpdateListener(Action fun)
            {
                _MakeEntity();
                AddLateUpdateListenerImp(fun).Forget();
            }

            private static async UniTaskVoid AddLateUpdateListenerImp(Action fun)
            {
                await UniTask.Yield( /*PlayerLoopTiming.LastPreLateUpdate*/);
                _updateDriver.AddLateUpdateListener(fun);
            }

            /// <summary>
            /// 移除帧更新事件。
            /// </summary>
            /// <param name="fun"></param>
            public static void RemoveUpdateListener(Action fun)
            {
                _MakeEntity();
                _updateDriver.RemoveUpdateListener(fun);
            }

            /// <summary>
            /// 移除物理帧更新事件。
            /// </summary>
            /// <param name="fun"></param>
            public static void RemoveFixedUpdateListener(Action fun)
            {
                _MakeEntity();
                _updateDriver.RemoveFixedUpdateListener(fun);
            }

            /// <summary>
            /// 移除Late帧更新事件。
            /// </summary>
            /// <param name="fun"></param>
            public static void RemoveLateUpdateListener(Action fun)
            {
                _MakeEntity();
                _updateDriver.RemoveLateUpdateListener(fun);
            }

            #endregion

            #region Unity Events 注入

            /// <summary>
            /// 为给外部提供的Destroy注册事件。
            /// </summary>
            /// <param name="fun"></param>
            public static void AddDestroyListener(Action fun)
            {
                _MakeEntity();
                _updateDriver.AddDestroyListener(fun);
            }

            /// <summary>
            /// 为给外部提供的Destroy反注册事件。
            /// </summary>
            /// <param name="fun"></param>
            public static void RemoveDestroyListener(Action fun)
            {
                _MakeEntity();
                _updateDriver.RemoveDestroyListener(fun);
            }

            /// <summary>
            /// 为给外部提供的OnDrawGizmos注册事件。
            /// </summary>
            /// <param name="fun"></param>
            public static void AddOnDrawGizmosListener(Action fun)
            {
                _MakeEntity();
                _updateDriver.AddOnDrawGizmosListener(fun);
            }

            /// <summary>
            /// 为给外部提供的OnDrawGizmos反注册事件。
            /// </summary>
            /// <param name="fun"></param>
            public static void RemoveOnDrawGizmosListener(Action fun)
            {
                _MakeEntity();
                _updateDriver.RemoveOnDrawGizmosListener(fun);
            }

            /// <summary>
            /// 为给外部提供的OnApplicationPause注册事件。
            /// </summary>
            /// <param name="fun"></param>
            public static void AddOnApplicationPauseListener(Action<bool> fun)
            {
                _MakeEntity();
                _updateDriver.AddOnApplicationPauseListener(fun);
            }

            /// <summary>
            /// 为给外部提供的OnApplicationPause反注册事件。
            /// </summary>
            /// <param name="fun"></param>
            public static void RemoveOnApplicationPauseListener(Action<bool> fun)
            {
                _MakeEntity();
                _updateDriver.RemoveOnApplicationPauseListener(fun);
            }

            #endregion

            private static void _MakeEntity()
            {
                if (_updateDriver != null)
                {
                    return;
                }

                _updateDriver = ModuleSystem.GetModule<IUpdateDriver>();
            }

            #region FindObjectOfType
            public static T FindObjectOfType<T>() where T : UnityEngine.Object
            {
#if UNITY_6000_0_OR_NEWER
                return UnityEngine.Object.FindFirstObjectByType<T>();
#else
                return UnityEngine.Object.FindObjectOfType<T>();

#endif
            }

            #endregion
            
            #region -- Camera

        /// <summary>
        /// 通过鼠标滚轮里滑和外滑的方式来调整摄像机的视角达到放大或缩小场景的目的
        /// </summary>
        /// <param name="camera">摄像机</param>
        public static void ScaleScene(Camera camera)
        {
            //里滑 -> 放大
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if (camera.fieldOfView <= 100)
                {
                    camera.fieldOfView += 2;
                }
                else if (camera.orthographicSize <= 20)
                {
                    camera.orthographicSize += 0.5f;
                }
            }

            //外滑 -> 缩小
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                if (camera.fieldOfView > 25)
                {
                    camera.fieldOfView -= 2;
                }
                else if (camera.orthographicSize >= 1)
                {
                    camera.orthographicSize -= 0.5f;
                }
            }
        }

        #endregion
        
        #region -- GameObject

        /// <summary>
        /// 獲取或添加組件
        /// </summary>
        /// <param name="go"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>T（組件）</returns>
        public static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            return component ? component : go.AddComponent<T>();
        }
        
        /// <summary>
        /// 通过鼠标滚轮里滑和外滑的方式来调整物体的localScale达到放大和缩小物体的目的
        /// </summary>
        /// <param name="go">要缩放的物体</param>
        /// <param name="speed">速率</param>
        /// <param name="minScale">最小缩放比例限制</param>
        /// <param name="maxScale">最大缩放比例限制</param>
        public static void ScaleGameObject(GameObject go, float speed, float minScale = 0.3f, float maxScale = 5f)
        {
            if (speed <= 0)
            {
                speed = 1;
            }

            float s = Input.GetAxis("Mouse ScrollWheel");
            float scale = 0.0f;

            if (s > 0)
            {
                scale = s * speed;
            }

            if (s < 0)
            {
                scale = 1f / (s * speed);
            }

            if (go.transform.localScale.x > maxScale)
            {
                go.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
            }

            if (go.transform.localScale.x <= maxScale && go.transform.localScale.x >= minScale)
            {
                go.transform.localScale += new Vector3(scale, scale, scale);
            }

            if (go.transform.localScale.x < minScale)
            {
                go.transform.localScale = new Vector3(minScale, minScale, minScale);
            }
        }

        #endregion

        #region -- Text

        /// <summary>
        /// 打字机效果输出文字内容。
        /// </summary>
        /// <param name="text">文本组件</param>
        /// <param name="content">文字内容</param>
        /// <param name="intervalTime">间隔时间</param>
        /// <returns></returns>
        public static IEnumerator TypeWriter(UnityEngine.UI.Text text, string content, float intervalTime)
        {
            text.text = "";
            int index = 0;
            while (index < content.Length)
            {
                yield return new WaitForSeconds(intervalTime);
                text.text += content[index];
                index++;
            }
        }
        
        /// <summary>
        /// 打字机效果输出文字内容。
        /// </summary>
        /// <param name="text">文本组件</param>
        /// <param name="content">文字内容</param>
        /// <param name="intervalTime">间隔时间</param>
        /// <returns></returns>
        public static IEnumerator TypeWriter(TMPro.TextMeshProUGUI text, string content, float intervalTime)
        {
            text.text = "";
            int index = 0;
            while (index < content.Length)
            {
                yield return new WaitForSeconds(intervalTime);
                text.text += content[index];
                index++;
            }
        }
        
        #endregion

        #region -- Transform -> rotate

        /// <summary>
        /// 物体绕自身旋转
        /// </summary>
        /// <param name="transform">自身物体</param>
        /// <param name="speed">旋转速度</param>
        public static void RotateAroundSelf(Transform transform, float speed)
        {
            if (transform)
            {
                transform.Rotate(Vector3.up, -speed * Input.GetAxis("Mouse X"), Space.World);
                transform.Rotate(Vector3.right, speed * Input.GetAxis("Mouse Y"), Space.World);
            }
        }
        
        /// <summary>
        /// 物体绕目标物体旋转
        /// </summary>
        /// <param name="self">自身物体</param>
        /// <param name="target">目标物体</param>
        /// <param name="speed">旋转速度</param>
        public static void RotateAroundTarget(Transform self, Transform target, float speed)
        {
            if (self && target)
            {
                var position = target.position;
                self.RotateAround(position, self.up, speed * Input.GetAxis("Mouse X"));
                self.RotateAround(position, self.right, -speed * Input.GetAxis("Mouse Y"));
            }
        }

        #endregion

        #region -- Transform -> material

        /// <summary>
        /// 得到物体上所有的材质球。
        /// </summary>
        /// <param name="transform"></param>
        public static Dictionary<string, Material[]> GetAllMaterials(Transform transform)
        {
            if (transform)
            {
                Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
                Dictionary<string, Material[]> materials = new Dictionary<string, Material[]>();
                foreach (Renderer renderer in renderers)
                {
                    string key = renderer.name;
                    Material[] mats = renderer.materials;
                    materials.TryAdd(key, mats);
                }

                return materials;
            }

            return null;
        }

        /// <summary>
        /// 把物体上的所有材质球改为同一个。
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="material"></param>
        public static void ModifyAllMaterials(Transform transform, Material material)
        {
            if (!transform || !material)
            {
                Debug.Log("The GameObject or Material is NULL");
                return;
            }

            Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] materials = new Material[renderers[i].materials.Length];
                for (int j = 0; j < materials.Length; j++)
                {
                    materials[j] = material;
                }

                renderers[i].materials = materials;
            }
        }

        /// <summary>
        /// 把物体上的所有材质球修改为设置的材质球
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="materials"></param>
        public static void ModifyAllMaterials(Transform transform, Material[][] materials)
        {
            if (transform == null || materials.Length == 0)
            {
                Debug.Log("The GameObject or Material is NULL");
                return;
            }

            Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
            if (materials.Length != renderers.Length)
            {
                Debug.Log("The count of material is NOT true");
                return;
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].materials.Length == materials[i].Length)
                {
                    renderers[i].materials = materials[i];
                }
                else
                {
                    Debug.Log($"物体：{renderers[i].gameObject.name}替换材质球失败");
                }
            }
        }

        #endregion

        #region -- Animator

        /// <summary>
        /// 动画是否播放完毕。
        /// </summary>
        /// <param name="animator">动画Animator。</param>
        /// <param name="motionName">动画名称。</param>
        /// <param name="time">动画归一化的时间，默认是1，但是有些动画不能达到1，可以用0.95左右的值去判断。</param>
        /// <returns></returns>
        public static bool AnimatorIsPlayOvered(Animator animator, string motionName, float time = 1.0f)
        {
            AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (animatorStateInfo.IsName(motionName))
            {
                if (animatorStateInfo.normalizedTime >= time)
                {
                    animator.speed = 0;
                    return true;
                }
            }
            else
            {
                Debug.LogError($"动画：{animator} 不包含：{motionName}");
            }

            return false;
        }

        #endregion
        }

        public class GameCoroutine
        {
            public string Name;
            public Coroutine Coroutine;
            public MonoBehaviour BindBehaviour;
        }

        class GameCoroutineAgent : MonoBehaviour
        {
        }
    }
}