using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Expressions
{
    class IndexerExpression : Expression
    {
        public Expression Left { get; private set; }
        public Expression Index { get; private set; }

        public IndexerExpression(ScriptToken end, Expression left, Expression index)
            : base(left.Start, end)
        {
            Left = left;
            Index = index;
        }

        public override TExpr Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Expression parent)
        {
            Parent = parent;

            Left.SetParent(this);
            Index.SetParent(this);
        }
    }
}
