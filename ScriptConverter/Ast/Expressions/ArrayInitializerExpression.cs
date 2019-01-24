using System.Collections.Generic;
using System.Collections.ObjectModel;
using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Expressions
{
    class ArrayInitializerExpression : Expression
    {
        public ReadOnlyCollection<Expression> Values { get; private set; }

        public ArrayInitializerExpression(ScriptToken start, ScriptToken end, List<Expression> values)
            : base(start, end)
        {
            Values = values.AsReadOnly();
        }

        public override TExpr Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Expression parent)
        {
            Parent = parent;

            foreach (var expression in Values)
            {
                expression.SetParent(this);
            }
        }
    }
}
