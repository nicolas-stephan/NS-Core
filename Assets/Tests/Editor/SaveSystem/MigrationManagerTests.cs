using System;
using NS.Core.SaveSystem;
using NUnit.Framework;

namespace NS.Core.Tests.Editor.SaveSystem {
    public class MigrationManagerTests {
        [Test]
        public void Migrate_FromV1ToV3_Success() {
            var serializer = new MockSerializer();
            var m1 = new MigrationV1ToV2();
            var m2 = new MigrationV2ToV3();
            var manager = new MigrationManager<DataV3, NoContext>(serializer, m1, m2);

            var payload = new VersionedPayload(typeof(DataV1).FullName, "{}");
            var result = manager.Migrate(payload, NoContext.Instance);

            Assert.IsInstanceOf<DataV3>(result);
            Assert.AreEqual("11", ((DataV3)result!).FinalValue);
        }

        [Test]
        public void Migrate_FromV2ToV3_Success() {
            var serializer = new MockSerializer();
            var m1 = new MigrationV1ToV2();
            var m2 = new MigrationV2ToV3();
            var manager = new MigrationManager<DataV3, NoContext>(serializer, m1, m2);

            var payload = new VersionedPayload(typeof(DataV2).FullName, "{}");
            var result = manager.Migrate(payload, NoContext.Instance);

            Assert.IsInstanceOf<DataV3>(result);
            Assert.AreEqual("50", ((DataV3)result!).FinalValue);
        }

        [Test]
        public void Migrate_UnknownType_ReturnsNull() {
            var serializer = new MockSerializer();
            var manager = new MigrationManager<DataV3, NoContext>(serializer);

            var payload = new VersionedPayload("UnknownType", "{}");
            var result = manager.Migrate(payload, NoContext.Instance);

            Assert.IsNull(result);
        }

        private class DataV1 {
            public int Value;
        }

        private class DataV2 {
            public int NewValue;
        }

        private class DataV3 {
            public string FinalValue = string.Empty;
        }

        private class MigrationV1ToV2 : Migration<DataV1, DataV2, NoContext> {
            public override DataV2 Migrate(DataV1 from, NoContext ctx) { return new DataV2 { NewValue = from.Value + 1 }; }
        }

        private class MigrationV2ToV3 : Migration<DataV2, DataV3, NoContext> {
            public override DataV3 Migrate(DataV2 from, NoContext ctx) { return new DataV3 { FinalValue = from.NewValue.ToString() }; }
        }

        private class MockSerializer : ISerializer {
            public string Serialize(object obj) { return ""; }

            public T? Deserialize<T>(string json) { return default; }

            public object? Deserialize(string json, Type type) {
                if (type == typeof(DataV1))
                    return new DataV1 { Value = 10 };
                if (type == typeof(DataV2))
                    return new DataV2 { NewValue = 50 };
                return null;
            }
        }
    }
}