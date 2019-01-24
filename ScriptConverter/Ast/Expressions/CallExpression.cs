using System.Collections.Generic;
using System.Collections.ObjectModel;
using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Expressions
{
    class CallExpression : Expression
    {
        public Expression Left { get; private set; }
        public ReadOnlyCollection<Expression> Parameters { get; private set; }
         
        public CallExpression(ScriptToken end, Expression left, List<Expression> parameters) 
            : base(left.Start, end)
        {
            Left = left;
            Parameters = parameters.AsReadOnly();
        }

        public override TExpr Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Expression parent)
        {
            Parent = parent;

            Left.SetParent(this);

            foreach (var expression in Parameters)
            {
                expression.SetParent(this);
            }
        }
    }
}
