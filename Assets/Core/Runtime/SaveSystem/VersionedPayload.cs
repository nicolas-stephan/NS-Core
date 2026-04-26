using System;

namespace NS.Core.SaveSystem {
    [Serializable]
    public class VersionedPayload {
        public string typeName;
        public string payload;

        public VersionedPayload(string? inTypeName, string inPayload) {
            typeName = inTypeName ?? string.Empty;
            payload = inPayload;
        }
    }
}