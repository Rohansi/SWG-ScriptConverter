using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Parser.Parselets.Expressions
{
    class BinaryOperatorParselet : IInfixParselet
    {
        private readonly bool _isRight;

        public BinaryOperatorParselet(PrecedenceValue precedence, bool isRight)
        {
            _isRight = isRight;
            Precedence = (int)precedence;
        }

        public int Precedence { get; private set; }

        public Expression Parse(ScriptParser parser, Expression left, ScriptToken token)
        {
            var right = parser.ParseExpression(Precedence - (_isRight ? 1 : 0));
            return new BinaryOperatorExpression(token.Type, left, right);
        }
    }
}
