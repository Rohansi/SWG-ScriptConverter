using System.Collections.Generic;
using System.Collections.ObjectModel;
using ScriptConverter.Ast.Expressions;
using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Statements
{
    class SwitchStatement : Statement
    {
        public class Branch
        {
            public ReadOnlyCollection<Expression> Conditions { get; private set; }
            public BlockStatement Block { get; private set; }

            public Branch(List<Expression> conditions, BlockStatement block)
            {
                Conditions = conditions.AsReadOnly();
                Block = block;
            }
        }

        public Expression Expression { get; private set; }
        public ReadOnlyCollection<Branch> Branches { get; private set; } 

        public SwitchStatement(ScriptToken start, ScriptToken end, Expression expression, List<Branch> branches)
            : base(start, end)
        {
            Expression = expression;
            Branches = branches.AsReadOnly();
        }

        public override TStmt Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Statement parent)
        {
            Parent = parent;

            Expression.SetParent(null);

            foreach (var branch in Branches)
            {
                foreach (var expression in branch.Conditions)
                {
                    if (expression != null)
                        expression.SetParent(null);
                }

                branch.Block.SetParent(this);
            }
        }
    }
}
