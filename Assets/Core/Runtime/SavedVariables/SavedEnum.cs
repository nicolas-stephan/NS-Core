using System;
using UnityEngine;

namespace NS.Core.SavedVariables {
    public sealed class SavedEnum<T> : SavedVariable<T> where T : struct, Enum {
        public SavedEnum(string key, T defaultValue) : base(key, defaultValue) { }

        public override void Init() {
            var intValue = PlayerPrefs.GetInt(Key, (int)(object)DefaultValue);
            if (!Enum.IsDefined(typeof(T), intValue))
                return;
            var value = (T)(object)intValue;
            SetWithoutSave(value);
            if (!PlayerPrefs.HasKey(Key))
                SaveValue(value);
        }

        protected override void SaveValue(T value) => PlayerPrefs.SetInt(Key, (int)(object)value);
    }
}