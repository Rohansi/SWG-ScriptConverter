using System.Linq;
using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Parser.Parselets.Expressions
{
    class ArrayInitializerParselet : IPrefixParselet
    {
        public Expression Parse(ScriptParser parser, ScriptToken token)
        {
            var values = parser.ParseSeparatedBy(ScriptTokenType.Comma, (_, first) =>
            {
                if (parser.Match(ScriptTokenType.RightBrace))
                    return null;

                return parser.ParseExpression();
            }).ToList();

            parser.Take(ScriptTokenType.RightBrace);
            return new ArrayInitializerExpression(token, parser.Previous, values);
        }
    }
}
