using System.Collections.Generic;
using System.Linq;
using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Parser.Parselets.Expressions
{
    class NewParselet : IPrefixParselet
    {
        public Expression Parse(ScriptParser parser, ScriptToken token)
        {
            var type = string.Join(".", parser.ParseSeparatedBy(ScriptTokenType.Dot, (_, first) => parser.Take(ScriptTokenType.Identifier).Contents));

            if (type == "string")
                type = "String";

            var isArray = parser.Match(ScriptTokenType.LeftSquare);

            parser.Take(isArray ? ScriptTokenType.LeftSquare : ScriptTokenType.LeftParen);

            var parameters = parser.ParseSeparatedBy(ScriptTokenType.Comma, (_, first) =>
            {
                if (first && parser.Match(isArray ? ScriptTokenType.RightSquare : ScriptTokenType.RightParen))
                    return null;

                return parser.ParseExpression();
            }).ToList();

            parser.Take(isArray ? ScriptTokenType.RightSquare : ScriptTokenType.RightParen);

            List<Expression> values = null;
            if (isArray && parser.MatchAndTake(ScriptTokenType.LeftBrace))
            {
                values = parser.ParseSeparatedBy(ScriptTokenType.Comma, (_, first) =>
                {
                    if (parser.Match(ScriptTokenType.RightBrace))
                        return null;

                    return parser.ParseExpression();
                }).ToList();

                parser.MatchAndTake(ScriptTokenType.RightBrace);
            }

            int arrayDimensions = isArray ? 1 : 0;
            if (isArray)
            {
                while (parser.Match(ScriptTokenType.LeftSquare) && parser.Match(ScriptTokenType.RightSquare, 1))
                {
                    parser.Take(ScriptTokenType.LeftSquare);
                    parser.Take(ScriptTokenType.RightSquare);
                    arrayDimensions++;
                }
            }

            return new NewExpression(token, parser.Previous, type, arrayDimensions, parameters, values);
        }
    }
}
