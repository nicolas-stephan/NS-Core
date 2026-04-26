using System;

namespace NS.Core.SaveSystem {
    public interface ISerializer {
        string Serialize(object obj);
        T? Deserialize<T>(string json);
        object? Deserialize(string json, Type type);
    }
}