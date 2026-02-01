using UnityEngine;

namespace NS.Core.Utils {
    public abstract class PersistentSingletonNoAutoInit<T> : MonoBehaviour where T : MonoBehaviour {
        private static T? _instance;
        public static T? Instance => _instance;

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