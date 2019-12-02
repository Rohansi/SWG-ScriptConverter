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
                var catchToken = parser.Take(ScriptTokenType.Catch);
                parser.Take(ScriptTokenType.LeftParen);

                parser.ParseNamedType(out var type, out var name, out var typeToken, out var nameToken);

                parser.Take(ScriptTokenType.RightParen);

                var block = parser.ParseBlock(false);
                catches.Add(new TryStatement.Branch(type, name, block)
                {
                    TypeToken = typeToken,
                    NameToken = nameToken,
                    CatchToken = catchToken,
                });
            } while (parser.Match(ScriptTokenType.Catch));

            return new TryStatement(token, parser.Previous, mainBlock, catches);
        }
    }
}
