using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Parser.Parselets.Expressions
{
    class IdentifierParselet : IPrefixParselet
    {
        public Expression Parse(ScriptParser parser, ScriptToken token)
        {
            return new IdentifierExpression(token);
        }
    }
}
