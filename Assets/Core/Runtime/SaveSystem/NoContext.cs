namespace NS.Core.SaveSystem {
    public sealed class NoContext {
        public static readonly NoContext Instance = new();
        private NoContext() { }
    }
}