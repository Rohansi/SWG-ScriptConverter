using System;
using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Statements
{
    abstract class Statement
    {
        public readonly ScriptToken Start;
        public readonly ScriptToken End;
        public Statement Parent { get; protected set; }

        protected Statement(ScriptToken start, ScriptToken end = null)
        {
            if (start == null)
                throw new ArgumentNullException("start");

            Start = start;
            End = end ?? start;
        }

        public abstract TStmt Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor);

        public abstract void SetParent(Statement parent);
    }
}
