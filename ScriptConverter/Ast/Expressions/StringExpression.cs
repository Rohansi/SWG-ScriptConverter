using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Expressions
{
    class StringExpression : Expression
    {
        public string Value { get; private set; }
        public bool IsSingleQuote { get; private set; }

        public StringExpression(ScriptToken token, bool singleQuote)
            : base(token)
        {
            Value = token.Contents;
            IsSingleQuote = singleQuote;
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
