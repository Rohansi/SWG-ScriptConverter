using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Parser.Parselets.Expressions
{
    class PostfixOperatorParselet : IInfixParselet
    {
        public PostfixOperatorParselet(PrecedenceValue precedence)
        {
            Precedence = (int)precedence;
        }

        public int Precedence { get; private set; }

        public Expression Parse(ScriptParser parser, Expression left, ScriptToken token)
        {
            return new PostfixOperatorExpression(token, left);
        }
    }
}
