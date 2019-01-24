using ScriptConverter.Ast.Statements;

namespace ScriptConverter.Parser.Parselets.Statements
{
    class WhileParselet : IStatementParselet
    {
        public Statement Parse(ScriptParser parser, ScriptToken token, out bool trailingSemicolon)
        {
            trailingSemicolon = false;

            parser.Take(ScriptTokenType.LeftParen);
            var condition = parser.ParseExpression();
            parser.Take(ScriptTokenType.RightParen);

            var block = parser.ParseBlock();

            return new WhileStatement(token, parser.Previous, condition, block);
        }
    }
}
