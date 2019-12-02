using System.Collections.Generic;
using ScriptConverter.Ast.Statements;

namespace ScriptConverter.Parser.Parselets.Statements
{
    class ConstVariableParselet : IStatementParselet
    {
        public Statement Parse(ScriptParser parser, ScriptToken token, out bool trailingSemicolon)
        {
            trailingSemicolon = true;

            parser.ParseNamedType(out var type, out var name, out var typeToken, out var nameToken);

            var assignmentToken = parser.Take(ScriptTokenType.Assign);

            var value = parser.ParseExpression();

            var definitions = new List<VariableStatement.Definition>()
            {
                new VariableStatement.Definition(type, name, value)
                {
                    TypeToken = typeToken,
                    NameToken = nameToken,
                    AssignmentToken = assignmentToken,
                },
            };

            return new VariableStatement(token, parser.Previous, type, true, definitions)
            {
                ModifierToken = token,
            };
        }
    }
}
