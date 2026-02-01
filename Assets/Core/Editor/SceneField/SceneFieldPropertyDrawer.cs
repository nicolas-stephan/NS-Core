using UnityEditor;
using UnityEngine;

namespace NS.Core.Editor.SceneField {
    [CustomPropertyDrawer(typeof(Utils.SceneField))]
    public class SceneFieldPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            var pGuid = property.FindPropertyRelative(Utils.SceneField.NameOfSceneGuid);
            var pBaked = property.FindPropertyRelative(Utils.SceneField.NameOfSceneName);

            SceneAsset currentAsset = null;
            if (!string.IsNullOrEmpty(pGuid.stringValue)) {
                var path = AssetDatabase.GUIDToAssetPath(pGuid.stringValue);
                if (!string.IsNullOrEmpty(path))
                    currentAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            }

            EditorGUI.BeginChangeCheck();
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var newObj = EditorGUI.ObjectField(position, currentAsset, typeof(SceneAsset), false);
            if (EditorGUI.EndChangeCheck()) {
                pGuid.stringValue = string.Empty;
                pBaked.stringValue = string.Empty;
                var sceneAsset = newObj as SceneAsset;
                if (sceneAsset != null) {
                    var path = AssetDatabase.GetAssetPath(sceneAsset);
                    pGuid.stringValue = AssetDatabase.AssetPathToGUID(path);
                    pBaked.stringValue = sceneAsset.name;
                }
            }

            EditorGUI.EndProperty();
        }
    }
}