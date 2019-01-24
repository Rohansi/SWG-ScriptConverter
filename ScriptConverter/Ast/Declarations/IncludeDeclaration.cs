﻿using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Declarations
{
    class IncludeDeclaration : Declaration
    {
        public string Name { get; private set; }

        public IncludeDeclaration(ScriptToken start, ScriptToken end, string name)
            : base(start, end)
        {
            Name = name;
        }

        public override TDecl Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent()
        {

        }
    }
}
