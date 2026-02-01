using UnityEngine;

namespace NS.Core.SavedVariables {
    public sealed class SavedFloat : SavedVariable<float> {
        public SavedFloat(string key, float defaultValue = 0) : base(key, defaultValue) { }

        public override void Init() {
            var value = PlayerPrefs.GetFloat(Key, DefaultValue);
            SetWithoutSave(value);
            if (!PlayerPrefs.HasKey(Key))
                SaveValue(value);
        }

        protected override void SaveValue(float value) => PlayerPrefs.SetFloat(Key, value);
    }
}