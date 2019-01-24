using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Expressions
{
    class FieldExpression : Expression
    {
        public Expression Left { get; private set; }
        public string Name { get; private set; }

        public FieldExpression(Expression left, ScriptToken name)
            : base(left.Start, name)
        {
            Left = left;
            Name = name.Contents;
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
