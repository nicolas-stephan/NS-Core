using System;
using System.Threading.Tasks;
using UnityEngine;

namespace NS.Core.SaveSystem {
    public abstract class SaveSystem<TTarget, TMigrationContext> where TTarget : class, new() {
        private readonly string _fullName;
        private readonly MigrationManager<TTarget, TMigrationContext> _manager;
        private readonly ISerializer _serializer;
        private readonly ISaveStorage _storage;

        public SaveSystem(ISaveStorage storage, ISerializer serializer, params IMigrationStepBase[] migrationSteps) {
            _storage = storage;
            _fullName = typeof(TTarget).FullName ?? throw new InvalidOperationException($"Unknown type {typeof(TTarget).FullName}");
            _serializer = serializer;
            _manager = new MigrationManager<TTarget, TMigrationContext>(serializer, migrationSteps);
        }

        public async Task SaveAsync(TTarget data) {
            var wrapper = new VersionedPayload(_fullName, _serializer.Serialize(data));
            var json = _serializer.Serialize(wrapper);
            await _storage.WriteTextAsync(json);
        }

        public async Task<TTarget> LoadAsync(TMigrationContext context) {
            if (!_storage.Exists())
                return new TTarget();

            try {
                var text = await _storage.ReadTextAsync();
                if (string.IsNullOrEmpty(text))
                    return new TTarget();
                var wrapper = _serializer.Deserialize<VersionedPayload>(text);
                if (wrapper == null)
                    return new TTarget();
                var result = _manager.Migrate(wrapper, context);
                return result == null ? new TTarget() : (TTarget)result;
            } catch (Exception e) {
                Debug.Log(e);
                //TODO maybe think about a "debug" process that try to get the data directly when deserialization process fail
                return new TTarget();
            }
        }
    }

    public abstract class SaveSystem<TTarget> where TTarget : class, new() {
        private readonly SaveSystem<TTarget, NoContext> _inner;

        public SaveSystem(ISaveStorage storage, ISerializer serializer, params IMigrationStepBase[] migrationSteps) {
            _inner = new NoContextSaveSystem(storage, serializer, migrationSteps);
        }

        public Task SaveAsync(TTarget data) { return _inner.SaveAsync(data); }

        public Task<TTarget> LoadAsync() { return _inner.LoadAsync(NoContext.Instance); }

        private class NoContextSaveSystem : SaveSystem<TTarget, NoContext> {
            public NoContextSaveSystem(ISaveStorage storage, ISerializer serializer, params IMigrationStepBase[] migrationSteps) : base(storage, serializer, migrationSteps) { }
        }
    }
}