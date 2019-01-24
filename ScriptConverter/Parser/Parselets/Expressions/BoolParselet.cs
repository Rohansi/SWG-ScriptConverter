using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Parser.Parselets.Expressions
{
    class BoolParselet : IPrefixParselet
    {
        private readonly bool _value;

        public BoolParselet(bool value)
        {
            _value = value;
        }

        public Expression Parse(ScriptParser parser, ScriptToken token)
        {
            return new BoolExpression(token, _value);
        }
    }
}
