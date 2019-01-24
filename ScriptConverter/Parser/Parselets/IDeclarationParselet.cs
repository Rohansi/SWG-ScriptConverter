using ScriptConverter.Ast.Declarations;

namespace ScriptConverter.Parser.Parselets
{
    interface IDeclarationParselet
    {
        Declaration Parse(ScriptParser parser, ScriptToken token);
    }
}
