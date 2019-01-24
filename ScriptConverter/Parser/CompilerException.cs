using System;
using JetBrains.Annotations;

namespace ScriptConverter.Parser
{
    public class CompilerException : Exception
    {
        protected CompilerException(string message)
            : base(message)
        {

        }

        [StringFormatMethod("format")]
        internal CompilerException(ScriptToken token, string format, params object[] args)
            : base(string.Format("{0}({1}): {2}", token.FileName ?? "null", token.RangeString, string.Format(format, args)))
        {

        }
    }
}
