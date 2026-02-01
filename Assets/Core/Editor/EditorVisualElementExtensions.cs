using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NS.Core.Editor {
    public static class EditorVisualElementExtensions {
        public static T? LoadAssetFromName<T>(string type, string name) where T : Object {
            var guids = AssetDatabase.FindAssets($"t:{type} {name}");
            if (guids.Length <= 0) {
                Debug.LogError($"Could not find {type} {name}");
                return null;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        public static void LoadStylesheetFromName(this VisualElement element, string name)
            => element.styleSheets.Add(LoadAssetFromName<StyleSheet>("StyleSheet", name));

        public static void BuildFromName(out VisualElement target, string name) {
            var asset = LoadAssetFromName<VisualTreeAsset>("VisualTreeAsset", name);
            target = asset != null ? asset.CloneTree() : new VisualElement();
        }
    }
}