using System;
using System.Text;

namespace NS.Core.Editor.CodeWriter {
    public class CodeWriter {
        private const string IndentString = "    ";
        
        private readonly StringBuilder _sb = new();
        private int _indentLevel;

        public CodeWriter WriteLine(string line = "") {
            if (line.Length > 0)
                _sb.Append(new string(' ', _indentLevel * IndentString.Length));

            _sb.AppendLine(line);
            return this;
        }

        public CodeWriter Write(string text) {
            _sb.Append(new string(' ', _indentLevel * IndentString.Length));
            _sb.Append(text);
            return this;
        }

        public IDisposable Block(string header) {
            WriteLine(header);
            WriteLine("{");
            _indentLevel++;
            return new BlockHandle(this);
        }

        public override string ToString() => _sb.ToString();

        private class BlockHandle : IDisposable {
            private readonly CodeWriter _writer;
            public BlockHandle(CodeWriter writer) => _writer = writer;

            public void Dispose() {
                _writer._indentLevel--;
                _writer.WriteLine("}");
            }
        }
    }
}