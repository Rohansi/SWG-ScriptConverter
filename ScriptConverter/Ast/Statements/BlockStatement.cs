using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Statements
{
    class BlockStatement : Statement
    {
        public ReadOnlyCollection<Statement> Statements { get; private set; }
        public bool IsSwitch { get; private set; }

        public BlockStatement(ScriptToken start, ScriptToken end, List<Statement> statements, bool isSwitch = false)
            : base(start, end)
        {
            if (statements == null)
                throw new ArgumentNullException("statements");

            Statements = statements.AsReadOnly();
            IsSwitch = isSwitch;
        }

        public override TStmt Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Statement parent)
        {
            Parent = parent;

            foreach (var statement in Statements)
            {
                statement.SetParent(this);
            }
        }
    }
}
