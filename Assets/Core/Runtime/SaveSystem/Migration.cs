using System;

namespace NS.Core.SaveSystem {
    public abstract class Migration<TFrom, TTo, TContext> : IMigrationStep<TContext> {
        public Type FromType => typeof(TFrom);
        public Type ToType => typeof(TTo);

        public object? MigrateObject(object? input, TContext ctx) { return input is TFrom from ? Migrate(from, ctx) : null; }

        public abstract TTo Migrate(TFrom from, TContext ctx);
    }

    public abstract class Migration<TFrom, TTo> : IMigrationStep {
        public Type FromType => typeof(TFrom);
        public Type ToType => typeof(TTo);

        public object? MigrateObject(object? input) { return input is TFrom from ? Migrate(from) : null; }

        public abstract TTo Migrate(TFrom from);
    }
}