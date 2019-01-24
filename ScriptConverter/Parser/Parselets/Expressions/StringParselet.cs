using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Parser.Parselets.Expressions
{
    class StringParselet : IPrefixParselet
    {
        private readonly bool _singleQuote;

        public StringParselet(bool singleQuote)
        {
            _singleQuote = singleQuote;
        }

        public Expression Parse(ScriptParser parser, ScriptToken token)
        {
            return new StringExpression(token, _singleQuote);
        }
    }
}
