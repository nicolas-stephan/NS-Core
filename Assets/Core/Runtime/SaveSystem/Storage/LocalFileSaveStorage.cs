using System.IO;
using System.Threading.Tasks;

namespace NS.Core.SaveSystem {
    public sealed class LocalFileSaveStorage : ISaveStorage {
        private readonly string _fullPath;

        public LocalFileSaveStorage(SaveLocation location, params string[]? subFolders) : this(LocalSavePath.GetPath(location, subFolders)) { }

        public LocalFileSaveStorage(string path) {
            _fullPath = path;
            var dir = Path.GetDirectoryName(_fullPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public async Task WriteTextAsync(string content) { await File.WriteAllTextAsync(_fullPath, content); }

        public async Task<string?> ReadTextAsync() {
            if (!File.Exists(_fullPath))
                return null;
            return await File.ReadAllTextAsync(_fullPath);
        }

        public bool Exists() { return File.Exists(_fullPath); }

        public Task DeleteAsync() {
            if (File.Exists(_fullPath))
                File.Delete(_fullPath);
            return Task.CompletedTask;
        }
    }
}