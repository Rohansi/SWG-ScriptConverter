using System.Collections.Generic;
using System.Collections.ObjectModel;
using ScriptConverter.Ast.Expressions;
using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Statements
{
    class ForStatement : Statement
    {
        public ReadOnlyCollection<Statement> Initializers { get; private set; }
        public Expression Condition { get; private set; }
        public ReadOnlyCollection<Expression> Increment { get; private set; }
        public BlockStatement Block { get; private set; }

        public ForStatement(ScriptToken start, ScriptToken end, List<Statement> initializers, Expression condition, List<Expression> increment, BlockStatement block)
            : base(start, end)
        {
            Initializers = initializers != null ? initializers.AsReadOnly() : null;
            Condition = condition;
            Increment = increment != null ? increment.AsReadOnly() : null;
            Block = block;
        }

        public override TStmt Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Statement parent)
        {
            Parent = parent;

            if (Initializers != null)
            {
                foreach (var statement in Initializers)
                {
                    statement.SetParent(this);
                }
            }

            if (Condition != null)
                Condition.SetParent(null);

            if (Increment != null)
            {
                foreach (var expression in Increment)
                {
                    expression.SetParent(null);
                }
            }

            Block.SetParent(this);
        }
    }
}
