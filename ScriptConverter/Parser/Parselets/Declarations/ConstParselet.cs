using ScriptConverter.Ast.Declarations;

namespace ScriptConverter.Parser.Parselets.Declarations
{
    class ConstParselet : IDeclarationParselet
    {
        public Declaration Parse(ScriptParser parser, ScriptToken token)
        {
            parser.ParseNamedType(out var type, out var name, out var typeToken, out var nameToken);

            var assignmentToken = parser.Take(ScriptTokenType.Assign);

            var value = parser.ParseExpression();

            var semicolonToken = parser.Take(ScriptTokenType.Semicolon);

            return new FieldDeclaration(token, parser.Previous, type, name, value, true, true)
            {
                ModifierToken = token,
                TypeToken = typeToken,
                NameToken = nameToken,
                AssignmentToken = assignmentToken,
                SemicolonToken = semicolonToken,
            };
        }
    }
}
