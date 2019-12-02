using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptConverter.Parser
{
    partial class ScriptLexer : Lexer<ScriptToken, ScriptTokenType>
    {
        public ScriptLexer(IEnumerable<char> source, string fileName = null)
            : base(source, fileName)
        {
            Rules = new List<LexerRule>
            {
                TryLexNumber,
                TryLexOperator,
                TryLexString,
                TryLexWord,
            };
        }

        protected override bool SkipComment(StringBuilder sb, out bool isMultiline)
        {
            if (TakeIfNext("//"))
            {
                isMultiline = true; // this comment eats the rest of this line + the newline character
                sb.Append("//");

                while (true)
                {
                    var ch = TakeChar();
                    if (ch == '\0')
                        return true;

                    sb.Append(ch);

                    if (ch == '\r' && PeekChar() == '\n')
                    {
                        sb.Append(TakeChar()); // \n
                        return true;
                    }

                    if (ch == '\n' || ch == '\r')
                        return true;
                }
            }

            if (TakeIfNext("/*"))
            {
                isMultiline = false;
                sb.Append("/*");

                while (true)
                {
                    if (TakeIfNext("*/"))
                    {
                        sb.Append("*/");
                        return true;
                    }

                    var ch = TakeChar();
                    
                    if (ch == '\n' || ch == '\r')
                        isMultiline = true;

                    if (ch == '\0')
                        return true;
                }
            }

            isMultiline = false;
            return false;
        }

        private bool TryLexOperator(char ch, out ScriptToken token)
        {
            var opList = _operators.Lookup(ch);
            if (opList != null)
            {
                var op = opList.FirstOrDefault(o => TakeIfNext(o.Item1));

                if (op != null)
                {
                    token = Token(op.Item2, op.Item1);
                    return true;
                }
            }

            token = null;
            return false;
        }

        private bool TryLexString(char ch, out ScriptToken token)
        {
            if (ch == '"' || ch == '\'')
            {
                TakeChar();

                var stringTerminator = ch;
                var stringContentsBuilder = new StringBuilder();

                while (true)
                {
                    if (AtEof)
                        throw ErrorStart(CompilerError.UnterminatedString);

                    ch = TakeChar();

                    if (ch == stringTerminator)
                        break;

                    if (ch != '\\')
                    {
                        switch (ch)
                        {
                            case '\n':
                                stringContentsBuilder.Append("\\n");
                                continue;
                            case '\r':
                                stringContentsBuilder.Append("\\r");
                                continue;
                        }

                        if (ch < ' ' || ch > '~')
                        {
                            stringContentsBuilder.AppendFormat("\\u{0:X4}", (int)ch);
                            continue;
                        }

                        stringContentsBuilder.Append(ch);
                        continue;
                    }

                    // escape sequence
                    ch = TakeChar();

                    if (AtEof)
                        throw Error(CompilerError.UnexpectedEofString);

                    switch (ch)
                    {
                        case '\\':
                            stringContentsBuilder.Append("\\\\");
                            break;

                        case '"':
                            stringContentsBuilder.Append("\\\"");
                            break;

                        case '\'':
                            stringContentsBuilder.Append("\\'");
                            break;

                        case 'r':
                            stringContentsBuilder.Append("\\r");
                            break;

                        case 'n':
                            stringContentsBuilder.Append("\\n");
                            break;

                        case 't':
                            stringContentsBuilder.Append("\\t");
                            break;

                        case '0':
                            stringContentsBuilder.Append("\\0");
                            break;

                        // TODO: more escape sequences

                        default:
                            throw Error(CompilerError.InvalidEscapeSequence, ch);
                    }
                }

                var stringContents = stringContentsBuilder.ToString();

                if (stringTerminator == '\'')
                {
                    token = Token(ScriptTokenType.SingleString, stringContents);
                    return true;
                }

                token = Token(ScriptTokenType.String, stringContents);
                return true;
            }

            token = null;
            return false;
        }

        private bool TryLexWord(char ch, out ScriptToken token)
        {
            if (char.IsLetter(ch) || ch == '_')
            {
                var wordContents = TakeWhile(c => char.IsLetterOrDigit(c) || c == '_');

                ScriptTokenType keywordType;
                var isKeyword = _keywords.TryGetValue(wordContents, out keywordType);

                // HACK: allow using enum as an identifier
                if (!isKeyword && wordContents == "enum")
                    wordContents = "enum_";

                token = Token(isKeyword ? keywordType : ScriptTokenType.Identifier, wordContents);
                return true;
            }

            token = null;
            return false;
        }

        private bool TryLexNumber(char ch, out ScriptToken token)
        {
            if (char.IsDigit(ch) || (ch == '.' && char.IsDigit(PeekChar(1))))
            {
                var format = NumberFormat.Decimal;
                var hasDecimal = false;
                var prefix = "";

                if (ch == '0')
                {
                    var nextChar = PeekChar(1);

                    if (nextChar == 'x' || nextChar == 'X')
                        format = NumberFormat.Hexadecimal;

                    if (nextChar == 'b' || nextChar == 'B')
                        format = NumberFormat.Binary;

                    if (format != NumberFormat.Decimal)
                    {
                        prefix = "" + TakeChar() + TakeChar();
                    }
                }

                if (ch == '.')
                {
                    hasDecimal = true;
                    prefix = "" + TakeChar();
                }

                Func<char, bool> isHexLetter = c => (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
                Func<char, bool> isDigit = c => char.IsDigit(c) || (format == NumberFormat.Hexadecimal && isHexLetter(c));

                var numberContents = TakeWhile(c =>
                {
                    if (format == NumberFormat.Decimal && c == '.' && isDigit(PeekChar(1)) && !hasDecimal)
                    {
                        hasDecimal = true;
                        return true;
                    }

                    if (format == NumberFormat.Decimal && (c == 'f' || c == 'd' || c == 'L') && !isDigit(PeekChar(1)))
                        return true;

                    return isDigit(c);
                });

                token = Token(ScriptTokenType.Number, prefix + numberContents);
                return true;
            }

            token = null;
            return false;
        }

        enum NumberFormat
        {
            Decimal, Hexadecimal, Binary
        }

        private ScriptToken Token(ScriptTokenType type, string contents)
        {
            return new ScriptToken(FileName, new SourcePosition(StartLine, StartColumn), new SourcePosition(Line, Column - 1), type, contents);
        }
    }
}
