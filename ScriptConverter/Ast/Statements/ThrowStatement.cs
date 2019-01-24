using ScriptConverter.Ast.Expressions;
using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Statements
{
    class ThrowStatement : Statement
    {
        public Expression Value { get; private set; }
        
        public ThrowStatement(ScriptToken start, Expression value)
            : base(start, value.End)
        {
            Value = value;
        }

        public override TStmt Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Statement parent)
        {
            Parent = parent;

            Value.SetParent(null);
        }
    }
}
