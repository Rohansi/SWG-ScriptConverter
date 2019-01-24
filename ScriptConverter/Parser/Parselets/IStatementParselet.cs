using ScriptConverter.Ast.Statements;

namespace ScriptConverter.Parser.Parselets
{
    interface IStatementParselet
    {
        Statement Parse(ScriptParser parser, ScriptToken token, out bool trailingSemicolon);
    }
}
