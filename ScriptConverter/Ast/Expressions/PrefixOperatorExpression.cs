using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Expressions
{
    class PrefixOperatorExpression : Expression
    {
        public ScriptTokenType Operation { get; private set; }
        public Expression Right { get; private set; }

        public PrefixOperatorExpression(ScriptToken start, Expression right)
            : base(start, right.End)
        {
            Operation = start.Type;
            Right = right;
        }

        public override TExpr Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Expression parent)
        {
            Parent = parent;

            Right.SetParent(this);
        }
    }
}
