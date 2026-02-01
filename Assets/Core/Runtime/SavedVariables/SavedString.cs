using UnityEngine;

namespace NS.Core.SavedVariables {
    public sealed class SavedString : SavedVariable<string> {
        public SavedString(string key, string defaultValue = "") : base(key, defaultValue) { }

        public override void Init() {
            var value = PlayerPrefs.GetString(Key, DefaultValue);
            SetWithoutSave(value);
            if (!PlayerPrefs.HasKey(Key))
                SaveValue(value);
        }

        protected override void SaveValue(string value) => PlayerPrefs.SetString(Key, value);
    }
}