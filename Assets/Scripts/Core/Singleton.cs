using UnityEngine;

namespace DesignPattern
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static bool DontDestroy { get; set; }
        
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);
                    
                    if(_instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).Name + " Singleton";
                        if (DontDestroy)
                        {
                            DontDestroyOnLoad(singletonObject);
                        }
                    }
                }
                return _instance;
            }
        }

        protected virtual void KeepAlive(bool enable)
        {
            DontDestroy = enable;
        }
        
        protected virtual void Awake()
        {
            if (_instance && _instance.GetInstanceID() != this.GetInstanceID())
            {
                Destroy(this);
            }
            if (!_instance)
            {
                _instance = (T)(MonoBehaviour)this;
            }
            if (DontDestroy)
            {
                DontDestroyOnLoad(this);
            }
        }
    }
}