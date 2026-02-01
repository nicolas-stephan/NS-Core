using System.Reflection;
using UnityEditor;

namespace NS.Core.Editor {
    public static class SerializedPropertyExtensions {
        public static object? GetValue(this SerializedProperty property) {
            string[] path = property.propertyPath.Split('.');
            object? obj = property.serializedObject.targetObject;

            foreach (var member in path) {
                if (member.Contains("[")) {
                    var arrayName = member[..member.IndexOf('[')];
                    var index = int.Parse(member.Substring(member.IndexOf('[') + 1, member.IndexOf(']') - member.IndexOf('[') - 1));
                    obj = GetFieldOrPropertyValue(obj, arrayName);
                    if (obj is System.Collections.IList list)
                        obj = list[index];
                } else {
                    obj = GetFieldOrPropertyValue(obj, member);
                }
            }

            return obj;
        }

        private static object? GetFieldOrPropertyValue(object? source, string name) {
            if (source == null)
                return null;
            var type = source.GetType();
            var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
                return field.GetValue(source);

            var prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return prop != null ? prop.GetValue(source) : null;
        }
    }
}