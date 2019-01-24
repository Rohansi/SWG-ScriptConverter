using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Parser.Parselets.Expressions
{
    class IndexerParselet : IInfixParselet
    {
        public int Precedence { get { return (int)PrecedenceValue.Suffix; } }

        public Expression Parse(ScriptParser parser, Expression left, ScriptToken token)
        {
            var index = parser.ParseExpression();
            
            parser.Take(ScriptTokenType.RightSquare);

            return new IndexerExpression(parser.Previous, left, index);
        }
    }
}
