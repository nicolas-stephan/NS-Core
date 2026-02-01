using System;
using UnityEngine;

namespace NS.Core.SavedVariables {
    public abstract class SavedVariable<T> {
        private readonly string _key;
        public readonly T DefaultValue;
        private string _keySuffix = string.Empty;

        private T _value = default!;
        private bool _isInitialized;

        protected string Key => _key + _keySuffix;

        public T Value {
            get {
                InternalInit();
                return _value;
            }
            set {
                InternalInit();
                _value = value;
                SaveValue(_value);
                OnChanged?.Invoke(_value);
            }
        }

        public delegate void OnChangedEvent(T value);

        public OnChangedEvent? OnChanged;

        protected SavedVariable(string key, T defaultValue) {
            _key = key;
            DefaultValue = defaultValue;
        }

        public void Delete() => PlayerPrefs.DeleteKey(Key);

        private void InternalInit() {
            if (_isInitialized)
                return;
#if UNITY_EDITOR
            string GetPlayerIDForPlayMode() {
                const string vpIdArg = "-vpId";
                var args = Environment.GetCommandLineArgs();
                foreach (var arg in args)
                    if (arg.StartsWith(vpIdArg))
                        return arg.Replace($"{vpIdArg}=", string.Empty);

                return string.Empty;
            }

            _keySuffix = $"-{GetPlayerIDForPlayMode()}";
#endif
            Init();
            _isInitialized = true;
        }

        public abstract void Init();
        protected abstract void SaveValue(T value);

        protected void SetWithoutSave(T value) { _value = value; }
    }
}