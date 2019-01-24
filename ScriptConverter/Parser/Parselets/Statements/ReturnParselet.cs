using ScriptConverter.Ast.Statements;

namespace ScriptConverter.Parser.Parselets.Statements
{
    class ReturnParselet : IStatementParselet
    {
        public Statement Parse(ScriptParser parser, ScriptToken token, out bool trailingSemicolon)
        {
            trailingSemicolon = true;

            if (parser.Match(ScriptTokenType.Semicolon))
                return new ReturnStatement(token, parser.Previous, null);

            var value = parser.ParseExpression();
            return new ReturnStatement(token, parser.Previous, value);
        }
    }
}
