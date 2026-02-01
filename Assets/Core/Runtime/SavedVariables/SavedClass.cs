using UnityEngine;

namespace NS.Core.SavedVariables {
    public sealed class SavedClass<T> : SavedVariable<T> where T : class {
        public SavedClass(string key, T defaultValue) : base(key, defaultValue) { }

        public override void Init() {
            var json = PlayerPrefs.GetString(Key);
            var value = json == string.Empty ? DefaultValue : JsonUtility.FromJson<T>(json);
            SetWithoutSave(value);
            if (!PlayerPrefs.HasKey(Key))
                SaveValue(value);
        }

        protected override void SaveValue(T value) => PlayerPrefs.SetString(Key, JsonUtility.ToJson(value));
    }
}