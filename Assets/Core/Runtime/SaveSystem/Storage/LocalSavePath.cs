using System.IO;
using UnityEngine;

namespace NS.Core.SaveSystem {
    public enum SaveLocation {
        Persistent,
        Temporary,
        Cache
    }

    public static class LocalSavePath {
        public static string GetBasePath(SaveLocation location) {
            string root;

            switch (location) {
                default:
                case SaveLocation.Persistent:
                    root = Application.persistentDataPath;
                    break;

                case SaveLocation.Temporary:
                    root = Application.temporaryCachePath;
                    break;

                case SaveLocation.Cache:
                    root = Path.Combine(Application.persistentDataPath, "cache");
                    break;
            }

            root = root.Replace('\\', '/');
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            return root;
        }

        public static string GetPath(SaveLocation location, params string[]? subFolders) {
            var basePath = GetBasePath(location);
            if (subFolders == null || subFolders.Length == 0)
                return basePath;

            var combinedSubPath = Path.Combine(subFolders);
            if (string.IsNullOrEmpty(combinedSubPath))
                return basePath;

            var fullPath = Path.Combine(basePath, combinedSubPath);
            var dir = Path.GetDirectoryName(fullPath);
            var targetDir = string.IsNullOrEmpty(dir) ? basePath : dir;
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            return fullPath;
        }
    }
}