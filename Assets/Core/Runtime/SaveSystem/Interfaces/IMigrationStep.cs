using System;

namespace NS.Core.SaveSystem {
    public interface IMigrationStepBase {
        Type FromType { get; }
        Type ToType { get; }
    }

    public interface IMigrationStep : IMigrationStepBase {
        object? MigrateObject(object? input);
    }

    public interface IMigrationStep<in TContext> : IMigrationStepBase {
        object? MigrateObject(object? input, TContext ctx);
    }
}