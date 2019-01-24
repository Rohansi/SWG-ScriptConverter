using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Parser.Parselets.Expressions
{
    class NumberParselet : IPrefixParselet
    {
        public Expression Parse(ScriptParser parser, ScriptToken token)
        {
            return new NumberExpression(token);
        }
    }
}
