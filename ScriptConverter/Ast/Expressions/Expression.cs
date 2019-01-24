using System;
using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Expressions
{
    abstract class Expression
    {
        public readonly ScriptToken Start;
        public readonly ScriptToken End;
        public Expression Parent { get; protected set; }

        protected Expression(ScriptToken start, ScriptToken end = null)
        {
            if (start == null)
                throw new ArgumentNullException("start");

            Start = start;
            End = end ?? start;
        }

        public abstract TExpr Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor);

        public abstract void SetParent(Expression parent);
    }
}
