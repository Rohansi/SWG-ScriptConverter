using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Parser.Parselets.Expressions
{
    class PrefixOperatorParselet : IPrefixParselet
    {
        private readonly int _precedence;

        public PrefixOperatorParselet(PrecedenceValue precedence)
        {
            _precedence = (int)precedence;
        }

        public Expression Parse(ScriptParser parser, ScriptToken token)
        {
            var right = parser.ParseExpression(_precedence);
            return new PrefixOperatorExpression(token, right);
        }
    }
}
