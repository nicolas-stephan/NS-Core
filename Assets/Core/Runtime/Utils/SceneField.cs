using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NS.Core.Utils {
    [Serializable]
    public struct SceneField {
        public const string NameOfSceneGuid = nameof(sceneGuid);
        public const string NameOfSceneName = nameof(sceneName);

        [SerializeField] private string sceneGuid;
        [SerializeField] private string sceneName;

        public string SceneGuid => sceneGuid;
        public string SceneName => sceneName;
        public bool IsValid => !string.IsNullOrEmpty(SceneName);

        public override string ToString() => SceneName;

        public bool IsActiveScene {
            get {
                if (!IsValid)
                    return false;

                var activeScene = SceneManager.GetActiveScene();
                if (!activeScene.isLoaded)
                    return false;

                return activeScene.name == SceneName;
            }
        }
    }

    public static class SceneFieldExtensions {
        public static void LoadScene(this SceneField sceneField, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
            => SceneManager.LoadScene(sceneField.SceneGuid, loadSceneMode);

        public static AsyncOperation? LoadSceneAsync(this SceneField sceneField, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
            => SceneManager.LoadSceneAsync(sceneField.SceneName, loadSceneMode);
    }
}