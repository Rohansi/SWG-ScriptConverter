using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Expressions
{
    class PostfixOperatorExpression : Expression
    {
        public ScriptTokenType Operation { get; private set; }
        public Expression Left { get; private set; }

        public PostfixOperatorExpression(ScriptToken end, Expression left)
            : base(left.Start, end)
        {
            Operation = end.Type;
            Left = left;
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
