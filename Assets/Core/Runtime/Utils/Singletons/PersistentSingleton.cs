using UnityEngine;

namespace NS.Core.Utils {
    public abstract class PersistentSingleton<T> : MonoBehaviour where T : MonoBehaviour {
        private static T? _instance;

        public static T Instance {
            get {
                if (_instance)
                    return _instance;

                var obj = new GameObject(typeof(T).Name);
                _instance = obj.AddComponent<T>();

                return _instance;
            }
        }

        protected virtual void Awake() {
            if (_instance && _instance != this) {
                Debug.LogWarning("Multiple instances of " + GetType() + " detected. Destroying...");
                Destroy(gameObject);
                return;
            }

            _instance = (T)(object)this;
            DontDestroyOnLoad(this);
        }
    }
}