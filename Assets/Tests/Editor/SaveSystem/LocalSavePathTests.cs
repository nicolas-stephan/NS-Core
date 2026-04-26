using System;
using System.IO;
using NS.Core.SaveSystem;
using NUnit.Framework;

namespace NS.Core.Tests.Editor.SaveSystem {
    public class LocalSavePathTests {
        [Test]
        public void GetBasePath_ReturnsValidPath() {
            var path = LocalSavePath.GetBasePath(SaveLocation.Persistent);
            Assert.IsFalse(string.IsNullOrEmpty(path));
            Assert.IsTrue(Directory.Exists(path));
        }

        [Test]
        public void GetPath_Persistent_ReturnsCorrectPath() {
            var basePath = LocalSavePath.GetBasePath(SaveLocation.Persistent);
            var path = LocalSavePath.GetPath(SaveLocation.Persistent, "Tests", "test.save");

            Assert.That(path, Does.StartWith(basePath));
            Assert.That(path, Does.Contain("Tests"));
            Assert.That(path, Does.EndWith("test.save"));
        }

        [Test]
        public void GetPath_CreatesDirectory() {
            var subFolder = "TempTestDir_" + Guid.NewGuid();
            var path = LocalSavePath.GetPath(SaveLocation.Persistent, subFolder, "test.save");
            var dir = Path.GetDirectoryName(path);

            Assert.IsTrue(Directory.Exists(dir));

            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }
    }
}