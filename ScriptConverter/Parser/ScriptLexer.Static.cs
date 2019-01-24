using System.Collections.Generic;

namespace ScriptConverter.Parser
{
    partial class ScriptLexer
    {
        private static OperatorDictionary<ScriptTokenType> _operators;
        private static Dictionary<string, ScriptTokenType> _keywords;

        static ScriptLexer()
        {
            EofToken = new ScriptToken(null, default(SourcePosition), default(SourcePosition), ScriptTokenType.Eof, null);

            _operators = new OperatorDictionary<ScriptTokenType>
            {
                { ";", ScriptTokenType.Semicolon },
                { ",", ScriptTokenType.Comma },
                { ".", ScriptTokenType.Dot },
                { "=", ScriptTokenType.Assign },
                { "?", ScriptTokenType.QuestionMark },
                { ":", ScriptTokenType.Colon },

                { "(", ScriptTokenType.LeftParen },
                { ")", ScriptTokenType.RightParen },

                { "{", ScriptTokenType.LeftBrace },
                { "}", ScriptTokenType.RightBrace },

                { "[", ScriptTokenType.LeftSquare },
                { "]", ScriptTokenType.RightSquare },
                
                { "+", ScriptTokenType.Add },
                { "+=", ScriptTokenType.AddAssign },
                { "-", ScriptTokenType.Subtract },
                { "-=", ScriptTokenType.SubtractAssign },
                { "*", ScriptTokenType.Multiply },
                { "*=", ScriptTokenType.MultiplyAssign },
                { "/", ScriptTokenType.Divide },
                { "/=", ScriptTokenType.DivideAssign },
                { "%", ScriptTokenType.Remainder },
                { "%=", ScriptTokenType.RemainderAssign },
                { "++", ScriptTokenType.Increment },
                { "--", ScriptTokenType.Decrement },
                
                { "==", ScriptTokenType.EqualTo },
                { "!=", ScriptTokenType.NotEqualTo },
                { ">", ScriptTokenType.GreaterThan },
                { ">=", ScriptTokenType.GreaterThanOrEqual },
                { "<", ScriptTokenType.LessThan },
                { "<=", ScriptTokenType.LessThanOrEqual },

                { "&&", ScriptTokenType.LogicalAnd },
                { "||", ScriptTokenType.LogicalOr },
                { "!", ScriptTokenType.LogicalNot },
                
                { "~", ScriptTokenType.BitwiseNot },
                { "&", ScriptTokenType.BitwiseAnd },
                { "&=", ScriptTokenType.BitwiseAndAssign },
                { "|", ScriptTokenType.BitwiseOr },
                { "|=", ScriptTokenType.BitwiseOrAssign },
                { "^", ScriptTokenType.BitwiseXor },
                { "^=", ScriptTokenType.BitwiseXorAssign },
                { "<<", ScriptTokenType.BitwiseShiftLeft },
                { "<<=", ScriptTokenType.BitwiseShiftLeftAssign },
                { ">>", ScriptTokenType.BitwiseShiftRight },
                { ">>=", ScriptTokenType.BitwiseShiftRightAssign },

                { "##", ScriptTokenType.Hash },
                
                { "#define", ScriptTokenType.PreprocessDefine },
                { "#ifdef", ScriptTokenType.PreprocessIfDef },
                { "#endif", ScriptTokenType.PreprocessEndIf },
            };

            _keywords = new Dictionary<string, ScriptTokenType>
            {
                { "null", ScriptTokenType.Null },
                { "true", ScriptTokenType.True },
                { "false", ScriptTokenType.False },

                { "return", ScriptTokenType.Return },
                { "if", ScriptTokenType.If },
                { "else", ScriptTokenType.Else },
                { "for", ScriptTokenType.For },
                { "while", ScriptTokenType.While },
                { "do", ScriptTokenType.Do },
                { "break", ScriptTokenType.Break },
                { "continue", ScriptTokenType.Continue },
                { "switch", ScriptTokenType.Switch },
                { "case", ScriptTokenType.Case },
                { "default", ScriptTokenType.Default },
                
                { "inherits", ScriptTokenType.Inherits },
                { "include", ScriptTokenType.Include },
                { "const", ScriptTokenType.Const },
                { "public", ScriptTokenType.Public },
                { "new", ScriptTokenType.New },
                { "instanceof", ScriptTokenType.InstanceOf },
                { "try", ScriptTokenType.Try },
                { "catch", ScriptTokenType.Catch },
                { "throw", ScriptTokenType.Throw },
            };
        }
    }
}
