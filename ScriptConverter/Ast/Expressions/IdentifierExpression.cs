using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Expressions
{
    class IdentifierExpression : Expression
    {
        public string Name { get; private set; }

        public IdentifierExpression(ScriptToken token)
            : base(token)
        {
            Name = token.Contents;
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
