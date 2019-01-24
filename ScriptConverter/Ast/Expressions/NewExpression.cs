using System.Collections.Generic;
using System.Collections.ObjectModel;
using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Expressions
{
    class NewExpression : Expression
    {
        public string Type { get; private set; }
        public bool IsArray { get { return ArrayDimensions > 0; } }
        public int ArrayDimensions { get; private set; }
        public ReadOnlyCollection<Expression> Parameters { get; private set; }
        public ReadOnlyCollection<Expression> Initializer { get; private set; }
         
        public NewExpression(ScriptToken start, ScriptToken end, string type, int arrayDimensions, List<Expression> parameters, List<Expression> initializer = null)
            : base(start, end)
        {
            Type = type;
            ArrayDimensions = arrayDimensions;
            Parameters = parameters.AsReadOnly();
            Initializer = initializer != null ? initializer.AsReadOnly() : null;
        }

        public override TExpr Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Expression parent)
        {
            Parent = parent;

            foreach (var expression in Parameters)
            {
                expression.SetParent(this);
            }

            if (Initializer != null)
            {
                foreach (var expression in Initializer)
                {
                    expression.SetParent(this);
                }
            }
        }
    }
}
