using System;
using JetBrains.Annotations;

namespace ScriptConverter.Parser
{
    class LexerException : Exception
    {
        [StringFormatMethod("format")]
        internal LexerException(string fileName, SourcePosition pos, string format, params object[] args)
            : base(string.Format("{0}({1}): {2}", fileName ?? "null", pos, string.Format(format, args)))
        {

        }
    }
}
