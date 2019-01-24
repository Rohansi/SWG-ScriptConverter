using System.Collections.Generic;
using System.Collections.ObjectModel;
using ScriptConverter.Ast.Expressions;
using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Statements
{
    class IfStatement : Statement
    {
        public class Branch
        {
            public Expression Condition { get; private set; }
            public BlockStatement Block { get; private set; }

            public Branch(Expression condition, BlockStatement block)
            {
                Condition = condition;
                Block = block;
            }
        }

        public ReadOnlyCollection<Branch> Branches { get; private set; }
        public Branch DefaultBranch { get; private set; }

        public IfStatement(ScriptToken start, ScriptToken end, List<Branch> branches, Branch defaultBranch)
            : base(start, end)
        {
            Branches = branches.AsReadOnly();
            DefaultBranch = defaultBranch;
        }

        public override TStmt Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Statement parent)
        {
            Parent = parent;

            foreach (var branch in Branches)
            {
                branch.Condition.SetParent(null);
                branch.Block.SetParent(this);
            }

            if (DefaultBranch != null)
            {
                DefaultBranch.Block.SetParent(this);
            }
        }
    }
}
