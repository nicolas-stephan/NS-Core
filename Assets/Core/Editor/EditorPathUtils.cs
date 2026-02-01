using System.IO;
using UnityEditor;
using UnityEngine;

namespace NS.Core.Editor {
    public static class EditorPathUtils {
        public static string GetFolderPath(ScriptableObject target) => GetDirectory(MonoScript.FromScriptableObject(target));

        public static string GetFolderPath(MonoBehaviour target) => GetDirectory(MonoScript.FromMonoBehaviour(target));

        private static string GetDirectory(MonoScript script) {
            if (script == null)
                throw new FileNotFoundException("Could not find MonoScript.");

            var scriptPath = AssetDatabase.GetAssetPath(script);
            if (string.IsNullOrEmpty(scriptPath))
                throw new FileNotFoundException("Could not find asset path.");

            var scriptDirectory = Path.GetDirectoryName(scriptPath);
            if (scriptDirectory == null)
                throw new DirectoryNotFoundException($"Could not find directory for script path '{scriptPath}'");

            return scriptDirectory;
        }
    }
}