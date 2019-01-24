using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Parser.Parselets.Expressions
{
    class GroupParselet : IPrefixParselet
    {
        public Expression Parse(ScriptParser parser, ScriptToken token)
        {
            if (IsCast(parser))
            {
                var type = parser.ParseType();
                parser.Take(ScriptTokenType.RightParen);
                var value = parser.ParseExpression((int)PrecedenceValue.Cast - 1);
                return new CastExpression(token, type, value);
            }

            var expression = parser.ParseExpression();
            parser.Take(ScriptTokenType.RightParen);
            return expression;
        }

        private static bool IsCast(ScriptParser parser)
        {
            var token = parser.Peek();
            if (token.Type != ScriptTokenType.Identifier)
                return false;

            if (token.Contents == "resizeable")
                return true;

            var i = 1;
            while (parser.Match(ScriptTokenType.Dot, i) && parser.Match(ScriptTokenType.Identifier, i + 1))
            {
                i += 2;
            }

            var next1Type = parser.Peek(i).Type;
            var next2Type = parser.Peek(i + 1).Type;

            return (next1Type == ScriptTokenType.RightParen && IsCastable(next2Type)) ||
                   (next1Type == ScriptTokenType.LeftSquare && next2Type == ScriptTokenType.RightSquare);
        }

        private static bool IsCastable(ScriptTokenType token)
        {
            return token == ScriptTokenType.Identifier ||
                   token == ScriptTokenType.Number ||
                   token == ScriptTokenType.Null ||
                   token == ScriptTokenType.LeftParen ||
                   token == ScriptTokenType.SingleString ||
                   token == ScriptTokenType.String;
        }
    }
}
