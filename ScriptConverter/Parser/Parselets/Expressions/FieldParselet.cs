using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Parser.Parselets.Expressions
{
    class FieldParselet : IInfixParselet
    {
        public int Precedence { get { return (int)PrecedenceValue.Suffix; } }

        public Expression Parse(ScriptParser parser, Expression left, ScriptToken token)
        {
            var name = parser.Take(ScriptTokenType.Identifier);
            return new FieldExpression(left, name);
        }
    }
}
