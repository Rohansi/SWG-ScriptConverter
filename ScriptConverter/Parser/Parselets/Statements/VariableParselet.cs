using System.Linq;
using ScriptConverter.Ast;
using ScriptConverter.Ast.Expressions;
using ScriptConverter.Ast.Statements;

namespace ScriptConverter.Parser.Parselets.Statements
{
    class VariableParselet : IStatementParselet
    {
        public Statement Parse(ScriptParser parser, ScriptToken token, out bool trailingSemicolon)
        {
            trailingSemicolon = true;

            var baseType = parser.ParseType(token);

            var definitions = parser.ParseSeparatedBy(ScriptTokenType.Comma, (_, first) =>
            {
                var name = parser.Take(ScriptTokenType.Identifier).Contents;

                var arrayDims = 0;
                while (parser.MatchAndTake(ScriptTokenType.LeftSquare))
                {
                    parser.Take(ScriptTokenType.RightSquare);
                    arrayDims++;
                }

                var type = baseType;

                if (arrayDims > 0)
                    type = new ScriptType(baseType.Name, baseType.ArrayDimensions + arrayDims, baseType.IsResizable);

                Expression value = null;
                if (parser.MatchAndTake(ScriptTokenType.Assign))
                    value = parser.ParseExpression();

                return new VariableStatement.Definition(type, name, value);
            }).ToList();

            return new VariableStatement(token, parser.Previous, baseType, false, definitions);
        }
    }
}
