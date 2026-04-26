using System.IO;
using System.Threading.Tasks;
using NS.Core.SaveSystem;
using NUnit.Framework;

namespace NS.Core.Tests.Editor.SaveSystem {
    public class LocalFileSaveStorageTests {
        private string _testFilePath = string.Empty;

        [SetUp]
        public void SetUp() {
            _testFilePath = Path.Combine(Path.GetTempPath(), "NS_Core_Test_Save.txt");
            if (File.Exists(_testFilePath))
                File.Delete(_testFilePath);
        }

        [TearDown]
        public void TearDown() {
            if (File.Exists(_testFilePath))
                File.Delete(_testFilePath);
        }

        [Test]
        public async Task WriteAndRead_Success() {
            var storage = new LocalFileSaveStorage(_testFilePath);
            const string content = "Hello World";

            await storage.WriteTextAsync(content);
            var result = await storage.ReadTextAsync();

            Assert.AreEqual(content, result);
        }

        [Test]
        public void Exists_ReturnsCorrectValue() {
            var storage = new LocalFileSaveStorage(_testFilePath);
            Assert.IsFalse(storage.Exists());

            File.WriteAllText(_testFilePath, "test");
            Assert.IsTrue(storage.Exists());
        }

        [Test]
        public async Task Delete_RemovesFile() {
            var storage = new LocalFileSaveStorage(_testFilePath);
            await File.WriteAllTextAsync(_testFilePath, "test");
            Assert.IsTrue(File.Exists(_testFilePath));

            await storage.DeleteAsync();
            Assert.IsFalse(File.Exists(_testFilePath));
        }
    }
}