namespace ScriptConverter.Ast.Expressions
{
    class TernaryExpression : Expression
    {
        public Expression Condition { get; private set; }
        public Expression TrueExpression { get; private set; }
        public Expression FalseExpression { get; private set; }

        public TernaryExpression(Expression condition, Expression trueExpression, Expression falseExpression)
            : base(condition.Start, falseExpression.End)
        {
            Condition = condition;
            TrueExpression = trueExpression;
            FalseExpression = falseExpression;
        }

        public override TExpr Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Expression parent)
        {
            Parent = parent;
        
            Condition.SetParent(this);
            TrueExpression.SetParent(this);
            FalseExpression.SetParent(this);
        }
    }
}
