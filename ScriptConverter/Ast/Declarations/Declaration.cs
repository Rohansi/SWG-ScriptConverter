using System;
using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Declarations
{
    abstract class Declaration
    {
        public readonly ScriptToken Start;
        public readonly ScriptToken End;

        protected Declaration(ScriptToken start, ScriptToken end = null)
        {
            if (start == null)
                throw new ArgumentNullException("start");

            Start = start;
            End = end ?? start;
        }

        public abstract TDecl Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor);

        public abstract void SetParent();
    }
}
