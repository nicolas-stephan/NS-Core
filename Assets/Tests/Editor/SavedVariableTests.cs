using System;
using NS.Core;
using NS.Core.SavedVariables;
using NUnit.Framework;

namespace Tests.Editor {
    public class SavedVariableTests {
        private static string NewKey(string name) => $"Common.Tests.{name}.{Guid.NewGuid()}";

        [SetUp]
        public void Setup() {
            // Nothing to do; each test uses a unique key suffix via GUID.
        }

        [Test]
        public void SavedInt_Defaults_ThenPersists_AndDeleteResets() {
            var key = NewKey(nameof(SavedInt_Defaults_ThenPersists_AndDeleteResets));

            var a = new SavedInt(key, 7);
            Assert.AreEqual(7, a.Value, "First access should initialize to default when no stored value exists");

            int changedTo = -1;
            a.OnChanged += v => changedTo = v;
            a.Value = 10;
            Assert.AreEqual(10, a.Value);
            Assert.AreEqual(10, changedTo, "OnChanged should be invoked with new value");

            // New instance should read the persisted value
            var b = new SavedInt(key, 3);
            Assert.AreEqual(10, b.Value, "Second instance should read persisted value, not its own default");

            // Delete should clear storage and next instance gets default
            a.Delete();
            var c = new SavedInt(key, 2);
            Assert.AreEqual(2, c.Value, "After Delete, value should reset to default on new instance");
        }

        [Test]
        public void SavedBool_PersistsAcrossInstances() {
            var key = NewKey(nameof(SavedBool_PersistsAcrossInstances));
            var a = new SavedBool(key, false);
            Assert.IsFalse(a.Value);
            a.Value = true;

            var b = new SavedBool(key, false);
            Assert.IsTrue(b.Value);
        }

        private class MyConfig {
            public int Count;
            public string Name = string.Empty;
        }

        [Test]
        public void SavedClass_Roundtrip_JSON() {
            var key = NewKey(nameof(SavedClass_Roundtrip_JSON));
            var def = new MyConfig { Count = 1, Name = "def" };
            var a = new SavedClass<MyConfig>(key, def);

            // Initially, default should be present
            Assert.AreEqual(1, a.Value.Count);
            Assert.AreEqual("def", a.Value.Name);

            a.Value = new MyConfig { Count = 99, Name = "hello" };

            var b = new SavedClass<MyConfig>(key, def);
            Assert.AreEqual(99, b.Value.Count);
            Assert.AreEqual("hello", b.Value.Name);
        }
    }
}