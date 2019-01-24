using ScriptConverter.Ast.Statements;

namespace ScriptConverter.Parser.Parselets.Statements
{
    class DoParselet : IStatementParselet
    {
        public Statement Parse(ScriptParser parser, ScriptToken token, out bool trailingSemicolon)
        {
            trailingSemicolon = true;

            var block = parser.ParseBlock();

            parser.Take(ScriptTokenType.While);
            parser.Take(ScriptTokenType.LeftParen);

            var condition = parser.ParseExpression();

            parser.Take(ScriptTokenType.RightParen);

            return new DoStatement(token, parser.Previous, condition, block);
        }
    }
}
