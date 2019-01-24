using System.Linq;
using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Parser.Parselets.Expressions
{
    class CallParselet : IInfixParselet
    {
        public int Precedence { get { return (int)PrecedenceValue.Suffix; } }

        public Expression Parse(ScriptParser parser, Expression left, ScriptToken token)
        {
            var parameters = parser.ParseSeparatedBy(ScriptTokenType.Comma, (_, first) =>
            {
                if (first && parser.Match(ScriptTokenType.RightParen))
                    return null;

                return parser.ParseExpression();
            }).ToList();

            parser.Take(ScriptTokenType.RightParen);

            return new CallExpression(parser.Previous, left, parameters);
        }
    }
}
