using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Expressions
{
    class CastExpression : Expression
    {
        public ScriptType Type { get; private set; }
        public Expression Value { get; private set; }

        public CastExpression(ScriptToken start, ScriptType type, Expression value)
            : base(start, value.End)
        {
            Type = type;
            Value = value;
        }

        public override TExpr Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Expression parent)
        {
            Parent = parent;

            Value.SetParent(this);
        }
    }
}
