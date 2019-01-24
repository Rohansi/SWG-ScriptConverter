using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Expressions
{
    class BoolExpression : Expression
    {
        public bool Value { get; private set; }

        public BoolExpression(ScriptToken token, bool value)
            : base(token)
        {
            Value = value;
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
