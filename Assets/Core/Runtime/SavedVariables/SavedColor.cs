using UnityEngine;

namespace NS.Core.SavedVariables {
    public sealed class SavedColor : SavedVariable<Color> {
        public SavedColor(string key, Color defaultValue) : base(key, defaultValue) { }

        public override void Init() {
            var value = new Color(
                PlayerPrefs.GetFloat(Key + "R", DefaultValue.r),
                PlayerPrefs.GetFloat(Key + "G", DefaultValue.g),
                PlayerPrefs.GetFloat(Key + "B", DefaultValue.b),
                PlayerPrefs.GetFloat(Key + "A", DefaultValue.a)
            );
            SetWithoutSave(value);
            if (!PlayerPrefs.HasKey(Key))
                SaveValue(value);
        }

        protected override void SaveValue(Color value) {
            PlayerPrefs.SetFloat(Key + "R", value.r);
            PlayerPrefs.SetFloat(Key + "G", value.g);
            PlayerPrefs.SetFloat(Key + "B", value.b);
            PlayerPrefs.SetFloat(Key + "A", value.a);
        }
    }
}