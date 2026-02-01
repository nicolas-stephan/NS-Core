using UnityEngine;

namespace NS.Core.Utils {
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        public static T? Instance { get; private set; }

        protected virtual void Awake() {
            if (Instance != null) {
                Debug.LogWarning("Multiple instances of " + GetType() + " detected. Destroying...");
                Destroy(Instance);
            }

            Instance = (T)(object)this;
        }
    }
}