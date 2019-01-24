using System.Collections.Generic;
using System.Linq;
using ScriptConverter.Ast.Expressions;
using ScriptConverter.Ast.Statements;

namespace ScriptConverter.Parser.Parselets.Statements
{
    class ForParselet : IStatementParselet
    {
        public Statement Parse(ScriptParser parser, ScriptToken token, out bool trailingSemicolon)
        {
            trailingSemicolon = false;

            parser.Take(ScriptTokenType.LeftParen);

            List<Statement> initializers = null;
            if (!parser.Match(ScriptTokenType.Semicolon))
            {
                initializers = parser.ParseSeparatedBy(ScriptTokenType.Comma, (_, first) =>
                {
                    var stmt = parser.ParseStatement(false);

                    if (!(stmt is VariableStatement) && !(stmt is NakedStatement))
                        throw new CompilerException(token, "bad for loop initializer");

                    return stmt;
                }).ToList();
            }

            parser.Take(ScriptTokenType.Semicolon);

            Expression condition = null;
            if (!parser.Match(ScriptTokenType.Semicolon))
                condition = parser.ParseExpression();

            parser.Take(ScriptTokenType.Semicolon);

            List<Expression> increment = null;
            if (!parser.Match(ScriptTokenType.RightParen))
                increment = parser.ParseSeparatedBy(ScriptTokenType.Comma, (_, first) => parser.ParseExpression()).ToList();

            parser.Take(ScriptTokenType.RightParen);

            var block = parser.ParseBlock();

            return new ForStatement(token, parser.Previous, initializers, condition, increment, block);
        }
    }
}
