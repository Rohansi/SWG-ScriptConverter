using System.Collections.Generic;
using ScriptConverter.Ast;
using ScriptConverter.Ast.Statements;

namespace ScriptConverter.Parser.Parselets.Statements
{
    class ConstVariableParselet : IStatementParselet
    {
        public Statement Parse(ScriptParser parser, ScriptToken token, out bool trailingSemicolon)
        {
            trailingSemicolon = true;

            ScriptType type;
            string name;
            parser.ParseNamedType(out type, out name);

            parser.Take(ScriptTokenType.Assign);

            var value = parser.ParseExpression();

            var definitions = new List<VariableStatement.Definition>()
            {
                new VariableStatement.Definition(type, name, value)
            };

            return new VariableStatement(token, parser.Previous, type, true, definitions);
        }
    }
}
