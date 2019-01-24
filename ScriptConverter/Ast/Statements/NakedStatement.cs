using ScriptConverter.Ast.Expressions;

namespace ScriptConverter.Ast.Statements
{
    class NakedStatement : Statement
    {
        public Expression Expression { get; private set; }

        public NakedStatement(Expression expression)
            : base(expression.Start, expression.End)
        {
            Expression = expression;
        }

        public override TStmt Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Statement parent)
        {
            Parent = parent;

            Expression.SetParent(null);
        }
    }
}
