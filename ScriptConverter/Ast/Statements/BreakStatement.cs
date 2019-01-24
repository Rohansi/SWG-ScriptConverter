using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Statements
{
    class BreakStatement : Statement
    {
        public BreakStatement(ScriptToken token)
            : base(token)
        {
            
        }

        public override TStmt Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Statement parent)
        {
            Parent = parent;
        }
    }
}
