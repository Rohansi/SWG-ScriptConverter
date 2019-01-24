using ScriptConverter.Ast;
using ScriptConverter.Ast.Declarations;

namespace ScriptConverter.Parser.Parselets.Declarations
{
    class ConstParselet : IDeclarationParselet
    {
        public Declaration Parse(ScriptParser parser, ScriptToken token)
        {
            ScriptType type;
            string name;
            parser.ParseNamedType(out type, out name);

            parser.Take(ScriptTokenType.Assign);

            var value = parser.ParseExpression();

            parser.Take(ScriptTokenType.Semicolon);

            return new FieldDeclaration(token, parser.Previous, type, name, value, true, true);
        }
    }
}
