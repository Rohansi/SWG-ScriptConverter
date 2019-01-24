using ScriptConverter.Ast.Expressions;
using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Statements
{
    class DoStatement : Statement
    {
        public Expression Condition { get; private set; }
        public BlockStatement Block { get; private set; }

        public DoStatement(ScriptToken start, ScriptToken end, Expression condition, BlockStatement block)
            : base(start, end)
        {
            Condition = condition;
            Block = block;
        }

        public override TStmt Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Statement parent)
        {
            Parent = parent;

            Condition.SetParent(null);
            Block.SetParent(this);
        }
    }
}
