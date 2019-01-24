using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Statements
{
    class TryStatement : Statement
    {
        public class Branch
        {
            public ScriptType Type { get; private set; }
            public string Name { get; private set; }
            public BlockStatement Block { get; private set; }

            public Branch(ScriptType type, string name, BlockStatement block)
            {
                Type = type;
                Name = name;
                Block = block;
            }
        }

        public BlockStatement Block { get; private set; }
        public ReadOnlyCollection<Branch> Branches { get; private set; }

        public TryStatement(ScriptToken start, ScriptToken end, BlockStatement block, List<Branch> branches) : base(start, end)
        {
            if (branches.Count < 1)
                throw new ArgumentException("need at least 1 branch", "branches");

            Block = block;
            Branches = branches.AsReadOnly();
        }

        public override TStmt Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Statement parent)
        {
            Parent = parent;

            Block.SetParent(this);

            foreach (var branch in Branches)
            {
                branch.Block.SetParent(this);
            }
        }
    }
}
