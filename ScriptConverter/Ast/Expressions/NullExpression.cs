using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Expressions
{
    class NullExpression : Expression
    {
        public NullExpression(ScriptToken token)
            : base(token)
        {
            
        }

        public override TExpr Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Expression parent)
        {
            Parent = parent;
        }
    }
}
