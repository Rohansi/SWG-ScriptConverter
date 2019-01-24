using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Expressions
{
    class NumberExpression : Expression
    {
        public string Value { get; private set; }

        public NumberExpression(ScriptToken token)
            : base(token)
        {
            Value = token.Contents;
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
