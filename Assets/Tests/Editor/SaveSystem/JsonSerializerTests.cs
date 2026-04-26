using System;
using NS.Core.SaveSystem;
using NUnit.Framework;

namespace NS.Core.Tests.Editor.SaveSystem {
    public class JsonSerializerTests {
        [Test]
        public void Serialize_ReturnsJson() {
            var serializer = new JsonSerializer();
            var data = new TestData { name = "Test", value = 123 };
            var json = serializer.Serialize(data);

            Assert.That(json, Does.Contain("Test"));
            Assert.That(json, Does.Contain("123"));
        }

        [Test]
        public void Deserialize_Generic_ReturnsCorrectData() {
            var serializer = new JsonSerializer();
            const string json = "{\"name\":\"Alice\",\"value\":99}";
            var result = serializer.Deserialize<TestData>(json);

            Assert.IsNotNull(result);
            Assert.AreEqual("Alice", result?.name);
            Assert.AreEqual(99, result?.value);
        }

        [Test]
        public void Deserialize_ByType_ReturnsCorrectData() {
            var serializer = new JsonSerializer();
            const string json = "{\"name\":\"Bob\",\"value\":50}";
            var result = serializer.Deserialize(json, typeof(TestData)) as TestData;

            Assert.IsNotNull(result);
            Assert.AreEqual("Bob", result?.name);
            Assert.AreEqual(50, result?.value);
        }

        [Serializable]
        private class TestData {
            public string name = "";
            public int value;
        }
    }
}