using System.Linq;
using ScriptConverter.Ast;
using ScriptConverter.Ast.Declarations;

namespace ScriptConverter.Parser.Parselets.Declarations
{
    class MethodParselet : IDeclarationParselet
    {
        public Declaration Parse(ScriptParser parser, ScriptToken token)
        {
            var returnType = parser.ParseType(token);
            var name = parser.Take(ScriptTokenType.Identifier).Contents;

            if (parser.MatchAndTake(ScriptTokenType.Assign))
            {
                var value = parser.ParseExpression();
                parser.Take(ScriptTokenType.Semicolon);

                return new FieldDeclaration(token, parser.Previous, returnType, name, value, false, false);
            }

            parser.Take(ScriptTokenType.LeftParen);

            var parameters = parser.ParseSeparatedBy(ScriptTokenType.Comma, (_, first) =>
            {
                if (parser.Match(ScriptTokenType.RightParen))
                    return null;

                ScriptType paramType;
                string paramName;
                parser.ParseNamedType(out paramType, out paramName);

                return new MethodDeclaration.Parameter(paramType, paramName);
            }).ToList();

            parser.Take(ScriptTokenType.RightParen);

            var block = parser.ParseBlock(false);

            return new MethodDeclaration(token, parser.Previous, returnType, name, parameters, block);
        }
    }
}
