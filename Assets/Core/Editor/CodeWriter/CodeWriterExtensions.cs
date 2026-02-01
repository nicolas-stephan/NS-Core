using System;

namespace NS.Core.Editor.CodeWriter {
    public static class CodeWriterExtensions {
        public static IDisposable Namespace(this CodeWriter w, string name)
            => w.Block($"namespace {name}");

        public static IDisposable Class(this CodeWriter w, string modifiers, string name, string? inheritance = null) {
            var header = $"{modifiers} class {name}";
            if (!string.IsNullOrEmpty(inheritance))
                header += $" : {inheritance}";

            return w.Block(header);
        }

        public static IDisposable Method(this CodeWriter w, string signature)
            => w.Block(signature);

        public static IDisposable If(this CodeWriter w, string condition)
            => w.Block($"if ({condition})");

        public static IDisposable Else(this CodeWriter w)
            => w.Block("else");

        public static IDisposable Region(this CodeWriter w, string name) {
            w.WriteLine($"#region {name}");
            return new RegionHandle(w);
        }

        private class RegionHandle : IDisposable {
            private readonly CodeWriter _w;
            public RegionHandle(CodeWriter w) => _w = w;
            public void Dispose() => _w.WriteLine("#endregion");
        }
    }
}