using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Parser.Parselets
{
    interface IInfixParselet
    {
        int Precedence { get; }

        Expression Parse(ScriptParser parser, Expression left, ScriptToken token);
    }
}
