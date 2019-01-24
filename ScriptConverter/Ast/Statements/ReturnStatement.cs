using ScriptConverter.Ast.Expressions;
using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Statements
{
    class ReturnStatement : Statement
    {
        public Expression Value { get; private set; }

        public ReturnStatement(ScriptToken start, ScriptToken end, Expression value)
            : base(start, end)
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

            if (Value != null)
                Value.SetParent(null);
        }
    }
}
