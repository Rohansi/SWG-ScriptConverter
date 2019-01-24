using System.Collections.Generic;
using ScriptConverter.Ast;
using ScriptConverter.Ast.Statements;

namespace ScriptConverter.Parser.Parselets.Statements
{
    class TryParselet : IStatementParselet
    {
        public Statement Parse(ScriptParser parser, ScriptToken token, out bool trailingSemicolon)
        {
            trailingSemicolon = false;

            var mainBlock = parser.ParseBlock(false);
            var catches = new List<TryStatement.Branch>(2);

            do
            {
                parser.Take(ScriptTokenType.Catch);
                parser.Take(ScriptTokenType.LeftParen);

                ScriptType type;
                string name;
                parser.ParseNamedType(out type, out name);

                parser.Take(ScriptTokenType.RightParen);

                var block = parser.ParseBlock(false);
                catches.Add(new TryStatement.Branch(type, name, block));
            } while (parser.Match(ScriptTokenType.Catch));

            return new TryStatement(token, parser.Previous, mainBlock, catches);
        }
    }
}
