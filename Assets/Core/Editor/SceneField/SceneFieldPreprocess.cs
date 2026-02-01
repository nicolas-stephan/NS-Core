using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace NS.Core.Editor.SceneField {
    /// <summary>
    /// Preprocesses build by validating and baking SceneField references in assets and scenes.
    /// Ensures runtime scene names are up-to-date and that referenced scenes are included in Build Settings.
    /// </summary>
    public class SceneFieldPreprocess : IPreprocessBuildWithReport {
        private readonly struct Details : IEquatable<Details> {
            public readonly Object Target;
            public readonly string SceneGuid;
            public readonly string ContainerPath;

            public Details(Object target, string sceneGuid, string containerPath) {
                Target = target;
                SceneGuid = sceneGuid;
                ContainerPath = containerPath;
            }

            public bool Equals(Details other) => SceneGuid == other.SceneGuid && ContainerPath == other.ContainerPath;
            public override bool Equals(object obj) => obj is Details other && Equals(other);
            public override int GetHashCode() => HashCode.Combine(SceneGuid, ContainerPath);
        }

        private const string AssetSearchFilter = "t:ScriptableObject t:Prefab";

        #region Build Preprocess Entry

        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report) {
            var enabledScenePaths = GetEnabledScenePaths();
            var usedSceneGuids = new HashSet<Details>();
            var issues = new List<string>();

            ScanAssetsForSceneFields(usedSceneGuids);
            ProcessScenesInBuild(enabledScenePaths, usedSceneGuids);
            CheckIfUsedSceneAreEnabled(enabledScenePaths, usedSceneGuids, issues);
            if (issues.Count > 0)
                throw new BuildFailedException("Build blocked: Some SceneField references are invalid or point to scenes not in Build Settings:\n" +
                                               string.Join("\n", issues.Distinct()));

            AssetDatabase.SaveAssets();
        }

        private void CheckIfUsedSceneAreEnabled(string[] enabledScenePaths, HashSet<Details> usedSceneGuids, List<string> issues) {
            foreach (var details in usedSceneGuids) {
                var path = AssetDatabase.GUIDToAssetPath(details.SceneGuid);
                if (enabledScenePaths.Contains(path))
                    continue;
                var objectName = details.Target.name;
                var objectType = details.Target.GetType().Name;
                var selectionHint = $"{details.ContainerPath} | {objectType} \"{objectName}\"";
                var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                issues.Add($"- {selectionHint} -> references scene '{sceneAsset.name}' not found in Build Settings.");
            }
        }

        #endregion

        #region Asset Processing

        private static void ScanAssetsForSceneFields(HashSet<Details> usedSceneGuids) {
            var assetGuids = AssetDatabase.FindAssets(AssetSearchFilter);
            foreach (var guid in assetGuids) {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(assetPath))
                    continue;

                foreach (var obj in AssetDatabase.LoadAllAssetsAtPath(assetPath))
                    if (obj is ScriptableObject so) {
                        BakeAndValidateSceneFields(so, out var sceneGuid);
                        if (!string.IsNullOrEmpty(sceneGuid))
                            usedSceneGuids.Add(new Details(so, sceneGuid, assetPath));
                    }

                if (!assetPath.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
                    continue;

                var prefabGo = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (prefabGo == null)
                    continue;

                foreach (var comp in prefabGo.GetComponentsInChildren<Component>(true))
                    if (comp != null) {
                        BakeAndValidateSceneFields(comp, out var sceneGuid);
                        if (!string.IsNullOrEmpty(sceneGuid))
                            usedSceneGuids.Add(new Details(comp, sceneGuid, assetPath));
                    }
            }
        }

        #endregion

        #region Scene Processing

        private static void ProcessScenesInBuild(string[] enabledScenePaths, HashSet<Details> usedSceneGuids) {
            var setup = EditorSceneManager.GetSceneManagerSetup();
            var originalActiveScene = SceneManager.GetActiveScene();

            try {
                EnsureActiveSceneValid(ref originalActiveScene);

                foreach (var scenePath in enabledScenePaths)
                    ProcessSingleScene(scenePath, usedSceneGuids, originalActiveScene);
            } finally {
                RestoreSceneSetupOrCreateEmpty(setup);
            }
        }

        private static void EnsureActiveSceneValid(ref Scene originalActiveScene) {
            if (originalActiveScene.IsValid() && originalActiveScene.isLoaded)
                return;

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            originalActiveScene = SceneManager.GetActiveScene();
        }

        private static void ProcessSingleScene(string scenePath, HashSet<Details> usedSceneGuids, Scene originalActiveScene) {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            var previousActiveScene = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(scene);

            var sceneModified = false;
            try {
                foreach (var root in scene.GetRootGameObjects()) {
                    foreach (var comp in root.GetComponentsInChildren<Component>(true)) {
                        if (comp == null)
                            continue;
                        if (BakeAndValidateSceneFields(comp, out var sceneGuid))
                            sceneModified = true;

                        if (!string.IsNullOrEmpty(sceneGuid))
                            usedSceneGuids.Add(new Details(comp, sceneGuid, scenePath));
                    }
                }

                if (!sceneModified)
                    return;

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            } finally {
                RestoreActiveScene(previousActiveScene, originalActiveScene);
                if (scene.name != originalActiveScene.name)
                    EditorSceneManager.CloseScene(scene, true);
            }
        }

        private static void RestoreActiveScene(Scene previousActiveScene, Scene originalActiveScene) {
            if (previousActiveScene.IsValid() && previousActiveScene.isLoaded) {
                SceneManager.SetActiveScene(previousActiveScene);
                return;
            }

            if (originalActiveScene.IsValid() && originalActiveScene.isLoaded)
                SceneManager.SetActiveScene(originalActiveScene);
        }

        private static void RestoreSceneSetupOrCreateEmpty(SceneSetup[] setup) {
            if (setup is { Length: > 0 }) {
                EditorSceneManager.RestoreSceneManagerSetup(setup);
                return;
            }

            if (SceneManager.sceneCount == 0)
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        #endregion

        #region SerializedProperty Helpers and Baking

        private static bool BakeAndValidateSceneFields(Object target, out string usedSceneGuid) {
            var modified = false;
            usedSceneGuid = string.Empty;
            try {
                using var so = new SerializedObject(target);
                var iterator = so.GetIterator();
                if (!iterator.NextVisible(true))
                    return false;

                do {
                    if (iterator.propertyType != SerializedPropertyType.Generic)
                        continue;

                    FindSceneFieldSerializedProperties(iterator, out var guidProperty, out var sceneNameProperty);
                    if (guidProperty == null || sceneNameProperty == null)
                        continue;

                    var scene = GetSceneAsset(guidProperty);
                    if (scene == null)
                        continue;

                    usedSceneGuid = guidProperty.stringValue;
                    if (EnsureSceneNameUpToDate(sceneNameProperty, scene))
                        modified = true;
                } while (iterator.NextVisible(false));

                if (modified)
                    so.ApplyModifiedPropertiesWithoutUndo();
            } catch {
                // ignore non-serializable objects
            }

            return modified;
        }

        private static void FindSceneFieldSerializedProperties(SerializedProperty container, out SerializedProperty? guidProperty, out SerializedProperty? sceneNameProperty) {
            var copy = container.Copy();
            var depth = copy.depth;

            guidProperty = null;
            sceneNameProperty = null;

            if (!copy.NextVisible(true))
                return;
            do {
                if (copy.depth <= depth)
                    break;

                if (guidProperty == null && copy.propertyPath.EndsWith($".{Utils.SceneField.NameOfSceneGuid}", StringComparison.Ordinal))
                    guidProperty = copy.Copy();
                else if (sceneNameProperty == null && copy.propertyPath.EndsWith($".{Utils.SceneField.NameOfSceneName}", StringComparison.Ordinal))
                    sceneNameProperty = copy.Copy();
            } while (copy.NextVisible(false));
        }

        #endregion

        #region Utilities

        private static string[] GetEnabledScenePaths() =>
            EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();

        private static SceneAsset? GetSceneAsset(SerializedProperty guidProperty) {
            if (string.IsNullOrEmpty(guidProperty.stringValue))
                return null;

            var fullPath = AssetDatabase.GUIDToAssetPath(guidProperty.stringValue);
            return AssetDatabase.LoadAssetAtPath<SceneAsset>(fullPath);
        }

        private static bool EnsureSceneNameUpToDate(SerializedProperty pRuntimeSceneName, SceneAsset sceneAsset) {
            if (pRuntimeSceneName.stringValue == sceneAsset.name)
                return false;
            pRuntimeSceneName.stringValue = sceneAsset.name;
            return true;
        }

        #endregion
    }
}