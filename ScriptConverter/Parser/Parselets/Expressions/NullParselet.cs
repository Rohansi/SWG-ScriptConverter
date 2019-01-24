using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Parser.Parselets.Expressions
{
    class NullParselet : IPrefixParselet
    {
        public Expression Parse(ScriptParser parser, ScriptToken token)
        {
            return new NullExpression(token);
        }
    }
}
