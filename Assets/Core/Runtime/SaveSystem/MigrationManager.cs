using System;
using System.Collections.Generic;
using System.Linq;

namespace NS.Core.SaveSystem {
    public class MigrationManager<TTarget, TContext> {
        private readonly Dictionary<Type, IMigrationStepBase> _migrations = new();
        private readonly ISerializer _serializer;
        private readonly Dictionary<string, Type> _types = new();

        public MigrationManager(ISerializer serializer, params IMigrationStepBase[] migrationSteps) {
            _serializer = serializer;
            RegisterMigrations(migrationSteps);
            var fullName = typeof(TTarget).FullName;
            if (string.IsNullOrEmpty(fullName))
                throw new Exception($"{typeof(TTarget).FullName} does not have a full name");
            _types.TryAdd(fullName, typeof(TTarget));
        }

        private void RegisterMigrations(IMigrationStepBase[] migrationSteps) {
            var allMigrations = migrationSteps.ToList();
            var targetType = typeof(TTarget);
            while (true) {
                var migration = allMigrations.FirstOrDefault(migration => migration.ToType == targetType);
                if (migration == null)
                    break;

                _migrations.Add(migration.FromType, migration);
                allMigrations.Remove(migration);
                targetType = migration.FromType;

                _types.TryAdd(migration.FromType.FullName, migration.FromType);
                _types.TryAdd(migration.ToType.FullName, migration.ToType);
            }
        }

        public object? Migrate(VersionedPayload versionedPayload, TContext ctx) {
            if (!_types.TryGetValue(versionedPayload.typeName, out var startType))
                return null;

            var fromObj = _serializer.Deserialize(versionedPayload.payload, startType);
            if (fromObj == null)
                return null;


            var currentType = fromObj.GetType();
            var current = fromObj;

            while (current != null) {
                if (!_migrations.TryGetValue(currentType, out var migration))
                    break;

                if (migration is IMigrationStep<TContext> withCtx)
                    current = withCtx.MigrateObject(current, ctx);
                else if (migration is IMigrationStep withoutCtx)
                    current = withoutCtx.MigrateObject(current);
                else
                    // Unknown migration kind; stop
                    break;

                currentType = migration.ToType;
            }

            return current;
        }
    }
}