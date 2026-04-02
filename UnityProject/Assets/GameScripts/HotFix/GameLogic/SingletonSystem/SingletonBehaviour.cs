using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 全局MonoBehavior必须继承于此
    /// </summary>
    /// <typeparam name="T">子类类型</typeparam>
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        private static T _instance;

        private void Awake()
        {
            if (CheckInstance())
            {
                OnLoad();
            }
        }

        private bool CheckInstance()
        {
            if (this == Instance)
            {
                return true;
            }

            Object.Destroy(gameObject);
            return false;
        }

        /// <summary>
        /// 单例挂载的MonoBehaviour加载出来之后（Awake）调用此方法。
        /// </summary>
        protected virtual void OnLoad()
        {
            
        }

        /// <summary>
        /// 提供外部调用的初始化方法。
        /// 不需要把单例挂载，调用方法时会自动创建一个实例。
        /// </summary>
        public virtual void Initialize()
        {
        }

        protected virtual void OnDestroy()
        {
            if (this == _instance)
            {
                Release();
            }
        }

        /// <summary>
        /// 判断对象是否有效
        /// </summary>
        public static bool IsValid => _instance != null;

        public static T Active()
        {
            return Instance;
        }

        public static void Release()
        {
            if (_instance != null)
            {
                SingletonSystem.Release(_instance.gameObject, _instance);
                _instance = null;
            }
        }

        /// <summary>
        /// 实例
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    System.Type thisType = typeof(T);
                    string instName = thisType.Name;
                    GameObject go = SingletonSystem.GetGameObject(instName);
                    if (go == null)
                    {
                        go = GameObject.Find($"/{instName}");
                        if (go == null)
                        {
                            go = new GameObject(instName);
                            go.transform.position = Vector3.zero;
                        }
                    }

                    if (go != null)
                    {
                        _instance = go.GetComponent<T>();
                        if (_instance == null)
                        {
                            _instance = go.AddComponent<T>();
                        }
                    }

                    if (_instance == null)
                    {
                        Log.Fatal($"Can't create SingletonBehaviour<{typeof(T)}>");
                    }
                    
                    SingletonSystem.Retain(go, _instance);
                }

                return _instance;
            }
        }
    }
}