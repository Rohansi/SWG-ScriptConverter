using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Parser.Parselets
{
    interface IPrefixParselet
    {
        Expression Parse(ScriptParser parser, ScriptToken token);
    }
}
