using NS.Core.SaveSystem;
using NUnit.Framework;

namespace NS.Core.Tests.Editor.SaveSystem {
    public class VersionedPayloadTests {
        [Test]
        public void Constructor_SetsProperties() {
            const string typeName = "MyNamespace.MyClass";
            const string payload = "{\"data\": 123}";
            var vp = new VersionedPayload(typeName, payload);

            Assert.AreEqual(typeName, vp.typeName);
            Assert.AreEqual(payload, vp.payload);
        }
    }
}