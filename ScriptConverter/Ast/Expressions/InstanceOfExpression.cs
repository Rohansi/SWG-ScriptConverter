using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Expressions
{
    class InstanceOfExpression : Expression
    {
        public Expression Left { get; private set; }
        public ScriptType Type { get; private set; }

        public InstanceOfExpression(ScriptToken end, Expression left, ScriptType type)
            : base(left.Start, end)
        {
            Left = left;
            Type = type;
        }

        public override TExpr Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Expression parent)
        {
            Parent = parent;

            Left.SetParent(this);
        }
    }
}
