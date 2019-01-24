using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Parser.Parselets.Expressions
{
    class InstanceOfParselet : IInfixParselet
    {
        public int Precedence { get { return (int)PrecedenceValue.Relational; } }

        public Expression Parse(ScriptParser parser, Expression left, ScriptToken token)
        {
            var type = parser.ParseType();
            return new InstanceOfExpression(parser.Previous, left, type);
        }
    }
}
