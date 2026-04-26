using System.Threading.Tasks;

namespace NS.Core.SaveSystem {
    /// <summary>
    ///     Abstraction for persisting save data. Implement this to target different backends
    ///     such as local files, web services, Steam Cloud, consoles, etc.
    /// </summary>
    public interface ISaveStorage {
        /// <summary>
        ///     Writes the provided text payload to the storage.
        /// </summary>
        Task WriteTextAsync(string content);

        /// <summary>
        ///     Reads the text payload from the storage. Returns null if nothing exists.
        /// </summary>
        Task<string?> ReadTextAsync();

        /// <summary>
        ///     Returns true if the payload exists in the storage.
        /// </summary>
        bool Exists();

        /// <summary>
        ///     Deletes the payload from the storage if present.
        /// </summary>
        Task DeleteAsync();
    }
}