using System.Collections.Generic;
using ScriptConverter.Ast.Expressions;
using ScriptConverter.Ast.Statements;

namespace ScriptConverter.Parser.Parselets.Statements
{
    class SwitchParselet : IStatementParselet
    {
        public Statement Parse(ScriptParser parser, ScriptToken token, out bool trailingSemicolon)
        {
            trailingSemicolon = false;

            parser.Take(ScriptTokenType.LeftParen);

            var expression = parser.ParseExpression();

            parser.Take(ScriptTokenType.RightParen);
            parser.Take(ScriptTokenType.LeftBrace);

            var hasDefault = false;
            var branches = new List<SwitchStatement.Branch>();

            while (!parser.Match(ScriptTokenType.RightBrace))
            {
                var conditions = new List<Expression>();

                while (true)
                {
                    if (parser.MatchAndTake(ScriptTokenType.Case))
                    {
                        var condition = parser.ParseExpression();
                        conditions.Add(condition);

                        parser.Take(ScriptTokenType.Colon);
                        continue;
                    }

                    if (!parser.Match(ScriptTokenType.Default))
                        break;

                    var defaultToken = parser.Take(ScriptTokenType.Default);

                    if (hasDefault)
                        throw new CompilerException(defaultToken, "Multiple default labels");

                    conditions.Add(null); // special default condition
                    hasDefault = true;

                    parser.Take(ScriptTokenType.Colon);
                }

                if (conditions.Count > 0)
                {
                    var block = ParseBlock(parser);
                    var branch = new SwitchStatement.Branch(conditions, block);
                    branches.Add(branch);
                    continue;
                }

                var errorToken = parser.Peek();
                throw new CompilerException(errorToken, "Expected Case or Default but found {0}", errorToken);
            }

            parser.Take(ScriptTokenType.RightBrace);

            return new SwitchStatement(token, parser.Previous, expression, branches);
        }

        private static BlockStatement ParseBlock(ScriptParser parser)
        {
            var statements = new List<Statement>();
            var start = parser.Peek();

            while (!parser.Match(ScriptTokenType.Case) &&
                   !parser.Match(ScriptTokenType.Default) &&
                   !parser.Match(ScriptTokenType.RightBrace))
            {
                statements.Add(parser.ParseStatement());
            }

            return new BlockStatement(start, parser.Previous, statements, true);
        }
    }
}
