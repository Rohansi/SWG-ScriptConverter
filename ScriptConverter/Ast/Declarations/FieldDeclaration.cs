using ScriptConverter.Ast.Expressions;
using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Declarations
{
    class FieldDeclaration : Declaration
    {
        public ScriptType Type { get; private set; }
        public string Name { get; private set; }
        public Expression Value { get; private set; }
        public bool IsConstant { get; private set; }
        public bool IsPublic { get; private set; }

        public FieldDeclaration(ScriptToken start, ScriptToken end, ScriptType type, string name, Expression value, bool isConst, bool isPublic)
            : base(start, end)
        {
            Type = type;
            Name = name;
            Value = value;
            IsConstant = isConst;
            IsPublic = isPublic;
        }

        public override TDecl Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent()
        {
            Value.SetParent(null);
        }
    }
}
