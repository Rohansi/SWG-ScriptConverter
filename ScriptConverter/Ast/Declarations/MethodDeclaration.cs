using System.Collections.Generic;
using System.Collections.ObjectModel;
using ScriptConverter.Ast.Statements;
using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Declarations
{
    class MethodDeclaration : Declaration
    {
        public class Parameter
        {
            public ScriptType Type { get; private set; }
            public string Name { get; private set; }

            public Parameter(ScriptType type, string name)
            {
                Type = type;
                Name = name;
            }
        }

        public ScriptType ReturnType { get; private set; }
        public string Name { get; private set; }
        public ReadOnlyCollection<Parameter> Parameters { get; private set; }
        public BlockStatement Block { get; private set; }

        public MethodDeclaration(ScriptToken start, ScriptToken end, ScriptType returnType, string name, List<Parameter> parameters, BlockStatement block)
            : base(start, end)
        {
            ReturnType = returnType;
            Name = name;
            Parameters = parameters.AsReadOnly();
            Block = block;
        }

        public override TDecl Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent()
        {
            Block.SetParent(null);
        }
    }
}
