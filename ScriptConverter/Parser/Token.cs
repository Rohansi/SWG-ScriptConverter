namespace ScriptConverter.Parser
{
    abstract class Token<T>
    {
        public readonly string FileName;
        public readonly SourcePosition Start;
        public readonly SourcePosition End;

        public readonly T Type;
        public readonly string Contents;

        protected Token(string fileName, SourcePosition start, SourcePosition end, T type, string contents)
        {
            FileName = fileName;
            Start = start;
            End = end;

            Type = type;
            Contents = contents;
        }

        protected Token(Token<T> token, T type, string contents)
            : this(token.FileName, token.Start, token.End, type, contents)
        {

        }

        protected Token(T type, string contents)
            : this(null, new SourcePosition(-1), new SourcePosition(-1), type, contents)
        {

        }

        public string RangeString
        {
            get { return Start.ToRangeString(End); }
        }
    }
    
    enum ScriptTokenType
    {
        Identifier,

        Number,
        SingleString,
        String,

        Null,
        True,
        False,

        Return,
        If,
        Else,
        For,
        While,
        Do,
        Break,
        Continue,
        Switch,
        Case,
        Default,

        Hash,
        Inherits,
        Include,
        Const,
        Public,
        New,
        InstanceOf,
        Try,
        Catch,
        Throw,

        Semicolon,
        Comma,
        Dot,
        Assign,
        QuestionMark,
        Colon,

        LeftParen,
        RightParen,

        LeftBrace,
        RightBrace,

        LeftSquare,
        RightSquare,

        Add,
        AddAssign,
        Subtract,
        SubtractAssign,
        Multiply,
        MultiplyAssign,
        Divide,
        DivideAssign,
        Remainder,
        RemainderAssign,
        Increment,
        Decrement,

        EqualTo,
        NotEqualTo,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,

        LogicalAnd,
        LogicalOr,
        LogicalNot,

        BitwiseNot,
        BitwiseAnd,
        BitwiseAndAssign,
        BitwiseOr,
        BitwiseOrAssign,
        BitwiseXor,
        BitwiseXorAssign,
        BitwiseShiftLeft,
        BitwiseShiftLeftAssign,
        BitwiseShiftRight,
        BitwiseShiftRightAssign,

        PreprocessDefine,
        PreprocessIfDef,
        PreprocessEndIf,

        Eof
    }

    class ScriptToken : Token<ScriptTokenType>
    {
        public ScriptToken(string fileName, SourcePosition start, SourcePosition end, ScriptTokenType type, string contents)
            : base(fileName, start, end, type, contents)
        {

        }

        public ScriptToken(Token<ScriptTokenType> token, ScriptTokenType type, string contents)
            : base(token, type, contents)
        {

        }

        public ScriptToken(ScriptTokenType type, string contents)
            : base(type, contents)
        {

        }

        public override string ToString()
        {
            switch (Type)
            {
                case ScriptTokenType.Identifier:
                case ScriptTokenType.Number:
                case ScriptTokenType.String:
                    var contentsStr = Contents;
                    if (contentsStr.Length > 16)
                        contentsStr = contentsStr.Substring(0, 13) + "...";

                    return string.Format("{0}('{1}')", Type, contentsStr);

                default:
                    return Type.ToString();
            }
        }
    }
}
