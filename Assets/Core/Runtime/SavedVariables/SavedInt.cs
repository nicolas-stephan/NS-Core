using UnityEngine;

namespace NS.Core.SavedVariables {
    public sealed class SavedInt : SavedVariable<int> {
        public SavedInt(string key, int defaultValue = 0) : base(key, defaultValue) { }

        public override void Init() {
            var value = PlayerPrefs.GetInt(Key, DefaultValue);
            SetWithoutSave(value);
            if (!PlayerPrefs.HasKey(Key))
                SaveValue(value);
        }

        protected override void SaveValue(int value) => PlayerPrefs.SetInt(Key, value);
    }
}