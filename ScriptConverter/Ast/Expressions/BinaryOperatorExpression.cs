using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Expressions
{
    class BinaryOperatorExpression : Expression
    {
        public ScriptTokenType Operation { get; private set; }
        public Expression Left { get; private set; }
        public Expression Right { get; private set; }

        public BinaryOperatorExpression(ScriptTokenType op, Expression left, Expression right)
            : base(left.Start, right.End)
        {
            Operation = op;
            Left = left;
            Right = right;
        }

        public override TExpr Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Expression parent)
        {
            Parent = parent;

            Left.SetParent(this);
            Right.SetParent(this);
        }
    }
}
