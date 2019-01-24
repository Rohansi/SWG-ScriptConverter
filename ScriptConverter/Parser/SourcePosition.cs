using System.Globalization;
using JetBrains.Annotations;

namespace ScriptConverter.Parser
{
    public struct SourcePosition
    {
        public readonly int Line;
        public readonly int Column;

        public SourcePosition(int line, int column = -1)
        {
            Line = line;
            Column = column;
        }

        [Pure]
        public override string ToString()
        {
            if (Column <= 0)
                return Line.ToString("G", CultureInfo.InvariantCulture);

            return string.Format(CultureInfo.InvariantCulture, "{0:G}:{1:G}", Line, Column);
        }

        [Pure]
        public string ToRangeString(SourcePosition end)
        {
            if (Line == end.Line && Column == end.Column)
                return ToString();

            return string.Format(CultureInfo.InvariantCulture, "{0}-{1}", this, end);
        }
    }
}
