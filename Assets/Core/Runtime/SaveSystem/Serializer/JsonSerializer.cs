using System;
using UnityEngine;

namespace NS.Core.SaveSystem {
    public sealed class JsonSerializer : ISerializer {
        public string Serialize(object obj) { return JsonUtility.ToJson(obj); }

        public T? Deserialize<T>(string json) { return JsonUtility.FromJson<T>(json); }

        public object? Deserialize(string json, Type type) { return JsonUtility.FromJson(json, type); }
    }
}