using System;
using System.Threading.Tasks;
using NS.Core.SaveSystem;
using NUnit.Framework;

namespace NS.Core.Tests.Editor.SaveSystem {
    public class SaveSystemTests {
        [Test]
        public async Task SaveAsync_WritesWrappedData() {
            var storage = new MockStorage();
            var serializer = new MockSerializer();
            var saveSystem = new ConcreteSaveSystem(storage, serializer);
            var data = new TestData { Name = "Hello", Value = 42 };

            await saveSystem.SaveAsync(data);

            Assert.AreEqual($"VP|{typeof(TestData).FullName}|Hello:42", storage.WrittenText);
        }

        [Test]
        public async Task LoadAsync_ReturnsDeserializedData() {
            var storage = new MockStorage();
            var serializer = new MockSerializer();
            var saveSystem = new ConcreteSaveSystem(storage, serializer);
            storage.WrittenText = $"VP|{typeof(TestData).FullName}|World:100";

            var result = await saveSystem.LoadAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual("World", result.Name);
            Assert.AreEqual(100, result.Value);
        }

        [Test]
        public async Task LoadAsync_FileNotExists_ReturnsNewInstance() {
            var storage = new MockStorage { ExistsFlag = false };
            var serializer = new MockSerializer();
            var saveSystem = new ConcreteSaveSystem(storage, serializer);

            var result = await saveSystem.LoadAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(string.Empty, result.Name);
            Assert.AreEqual(0, result.Value);
        }

        private class TestData {
            public string Name { get; set; } = string.Empty;
            public int Value { get; set; }
        }

        private class ConcreteSaveSystem : SaveSystem<TestData> {
            public ConcreteSaveSystem(ISaveStorage storage, ISerializer serializer, params IMigrationStepBase[] migrations)
                : base(storage, serializer, migrations) { }
        }

        private class MockStorage : ISaveStorage {
            public bool ExistsFlag = true;
            public string? WrittenText;

            public Task WriteTextAsync(string text) {
                WrittenText = text;
                return Task.CompletedTask;
            }

            public Task<string?> ReadTextAsync() { return Task.FromResult(WrittenText); }

            public bool Exists() { return ExistsFlag; }

            public Task DeleteAsync() {
                WrittenText = null;
                ExistsFlag = false;
                return Task.CompletedTask;
            }
        }

        private class MockSerializer : ISerializer {
            public string Serialize(object obj) {
                if (obj is VersionedPayload vp)
                    return $"VP|{vp.typeName}|{vp.payload}";
                if (obj is TestData td)
                    return $"{td.Name}:{td.Value}";
                return obj.ToString();
            }

            public T? Deserialize<T>(string json) {
                if (typeof(T) != typeof(VersionedPayload) || !json.StartsWith("VP|"))
                    return default;
                var parts = json.Split('|');
                return (T)(object)new VersionedPayload(parts[1], parts[2]);
            }

            public object? Deserialize(string json, Type type) {
                if (type != typeof(TestData))
                    return null;
                var parts = json.Split(':');
                return new TestData { Name = parts[0], Value = int.Parse(parts[1]) };
            }
        }
    }
}