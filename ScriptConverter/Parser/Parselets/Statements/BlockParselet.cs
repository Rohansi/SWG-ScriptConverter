using System.Collections.Generic;
using ScriptConverter.Ast.Statements;

namespace ScriptConverter.Parser.Parselets.Statements
{
    class BlockParselet : IStatementParselet
    {
        public Statement Parse(ScriptParser parser, ScriptToken token, out bool trailingSemicolon)
        {
            trailingSemicolon = false;

            var statements = new List<Statement>();

            while (!parser.Match(ScriptTokenType.RightBrace))
            {
                statements.Add(parser.ParseStatement());
            }

            parser.Take(ScriptTokenType.RightBrace);

            return new BlockStatement(token, parser.Previous, statements);
        }
    }
}
