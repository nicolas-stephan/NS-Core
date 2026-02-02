using System;
using System.Collections.Generic;
using System.Reflection;

namespace NS.Core.Utils {
    public static class AssemblyUtils {
        /// <summary>
        /// Returns all assemblies that are likely part of your actual project code,
        /// excluding Unity, System, Mono, and other framework assemblies.
        /// </summary>
        public static IEnumerable<Assembly> GetUserAssemblies() {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies) {
                if (assembly.IsDynamic)
                    continue;

                var name = assembly.FullName;
                if (IsFrameworkAssembly(name) || IsUnityAssembly(name))
                    continue;

#if UNITY_EDITOR
                if (IsEditorAssembly(name))
                    continue;
#endif

                yield return assembly;
            }
        }

        private static bool IsEditorAssembly(string name) {
            return name.IndexOf("Editor", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   name.IndexOf("Test", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsFrameworkAssembly(string name) {
            return name.StartsWith("System", StringComparison.Ordinal) ||
                   name.StartsWith("mscorlib", StringComparison.Ordinal) ||
                   name.StartsWith("netstandard", StringComparison.Ordinal) ||
                   name.StartsWith("Mono", StringComparison.Ordinal) ||
                   name.StartsWith("I18N", StringComparison.Ordinal);
        }

        private static bool IsUnityAssembly(string name) {
            return name.StartsWith("Unity", StringComparison.Ordinal) ||
                   name.StartsWith("Bee", StringComparison.Ordinal) ||
                   name.StartsWith("nunit", StringComparison.Ordinal);
        }
    }
}