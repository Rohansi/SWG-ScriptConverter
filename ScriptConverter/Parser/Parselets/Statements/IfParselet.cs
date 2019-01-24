using System.Collections.Generic;
using ScriptConverter.Ast.Expressions;
using ScriptConverter.Ast.Statements;

namespace ScriptConverter.Parser.Parselets.Statements
{
    class IfParselet : IStatementParselet
    {
        public Statement Parse(ScriptParser parser, ScriptToken token, out bool trailingSemicolon)
        {
            trailingSemicolon = false;

            var first = true;
            var branches = new List<IfStatement.Branch>();
            IfStatement.Branch defaultBranch = null;

            do
            {
                var isDefaultBranch = !first && !parser.MatchAndTake(ScriptTokenType.If);
                first = false;

                Expression condition = null;
                if (!isDefaultBranch)
                {
                    parser.Take(ScriptTokenType.LeftParen);

                    condition = parser.ParseExpression();

                    parser.Take(ScriptTokenType.RightParen);
                }

                var block = parser.ParseBlock();
                var branch = new IfStatement.Branch(condition, block);

                if (isDefaultBranch)
                    defaultBranch = branch;
                else
                    branches.Add(branch);

                if (isDefaultBranch)
                    break;
            } while (parser.MatchAndTake(ScriptTokenType.Else));

            return new IfStatement(token, parser.Previous, branches, defaultBranch);
        }
    }
}
