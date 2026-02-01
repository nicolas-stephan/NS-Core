using UnityEngine;

namespace NS.Core.SavedVariables {
    public sealed class SavedBool : SavedVariable<bool> {
        public SavedBool(string key, bool defaultValue = false) : base(key, defaultValue) { }

        public override void Init() {
            var value = PlayerPrefs.GetInt(Key, DefaultValue ? 1 : 0) == 1;
            SetWithoutSave(value);
            if (!PlayerPrefs.HasKey(Key))
                SaveValue(value);
        }

        protected override void SaveValue(bool value) => PlayerPrefs.SetInt(Key, value ? 1 : 0);
    }
}