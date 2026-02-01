#if USING_UNITASK && USING_ADDRESSABLES
using System;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace NS.Core.Utils {
    public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T> {
        public const string NameOfInstance = nameof(_instance);

        private static T? _instance;
        private static AsyncOperationHandle<T>? _handle;

        protected virtual string AddressKey => typeof(T).Name;

        public static T Instance {
            get {
#if UNITY_EDITOR
                if (_instance == null)
                    _instance = LoadFromAssetDatabaseOrCreate();
#endif
                return _instance == null ? throw new Exception($"{typeof(T).Name} not yet loaded. Call InitializeAsync() first!") : _instance;
            }
        }

        /// <summary>
        /// Loads the ScriptableObject instance asynchronously if not already loaded.
        /// </summary>
        public static async UniTask InitializeAsync() {
            if (_instance != null)
                return;

            var handle = Addressables.LoadAssetAsync<T>(typeof(T).Name);
            _handle = handle;
            await handle.Task.AsUniTask();
            if (handle.Status == AsyncOperationStatus.Succeeded) {
                _instance = handle.Result;
                return;
            }

            Debug.LogError($"Failed to load Addressable ScriptableObject '{typeof(T).Name}'");
        }

        public static void InitializeSync() {
            if (_instance != null)
                return;

            _instance = Addressables.LoadAssetAsync<T>(typeof(T).Name).WaitForCompletion();
            if (_instance == null)
                Debug.LogError($"Failed to load Addressable ScriptableObject '{typeof(T).Name}'");
        }

        public static void Release() {
            if (_handle.HasValue) {
                Addressables.Release(_handle.Value);
                _handle = null;
            }

            _instance = null;
        }

#if UNITY_EDITOR
        private static T LoadFromAssetDatabaseOrCreate() {
            var instance = LoadFromAssetDatabase();
            if (instance != null)
                return instance;

            var assetPath = $"Assets/{typeof(T).Name}.asset";
            instance = CreateInstance<T>();
            AssetDatabase.CreateAsset(instance, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Created new singleton asset for {typeof(T).Name} at {assetPath}");
            MakeAssetAddressable(assetPath, typeof(T).Name);
            return instance;
        }

        private static T? LoadFromAssetDatabase() {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            if (guids.Length > 0) {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<T>(path);
            }

            Debug.LogWarning($"Could not find {typeof(T).Name} in AssetDatabase.");
            return null;
        }

        private static void MakeAssetAddressable(string assetPath, string addressKey) {
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            if (settings == null) {
                Debug.LogError("AddressableAssetSettings not found or Addressables not initialized.");
                return;
            }

            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            var entry = settings.FindAssetEntry(guid);
            if (entry == null) {
                var group = settings.DefaultGroup;
                entry = settings.CreateOrMoveEntry(guid, group);
            }

            entry.address = addressKey;

            EditorUtility.SetDirty(settings);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
#endif