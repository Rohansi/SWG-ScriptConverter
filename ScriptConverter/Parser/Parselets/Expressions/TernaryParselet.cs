using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Parser.Parselets.Expressions
{
    class TernaryParselet : IInfixParselet
    {
        public int Precedence { get { return (int)PrecedenceValue.Ternary; } }

        public Expression Parse(ScriptParser parser, Expression left, ScriptToken token)
        {
            var trueExpr = parser.ParseExpression();
            parser.Take(ScriptTokenType.Colon);
            var falseExpr = parser.ParseExpression();

            return new TernaryExpression(left, trueExpr, falseExpr);
        }
    }
}
