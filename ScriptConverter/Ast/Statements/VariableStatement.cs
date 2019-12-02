using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ScriptConverter.Ast.Expressions;
using ScriptConverter.Parser;

namespace ScriptConverter.Ast.Statements
{
    class VariableStatement : Statement
    {
        public class Definition
        {
            public ScriptType Type { get; private set; }
            public string Name { get; private set; }
            public Expression Value { get; private set; }

            public ScriptToken TypeToken { get; set; }
            public ScriptToken NameToken { get; set; }
            public ScriptToken AssignmentToken { get; set; }

            public Definition(ScriptType type, string name, Expression value)
            {
                Type = type;
                Name = name;
                Value = value;
            }
        }

        public ScriptType BaseType { get; private set; }
        public bool Final { get; private set; }
        public ReadOnlyCollection<Definition> Definitions { get; private set; }

        public ScriptToken ModifierToken { get; set; }
        public ScriptToken SemicolonToken { get; set; }

        public VariableStatement(ScriptToken start, ScriptToken end, ScriptType baseType, bool final, List<Definition> definitions)
            : base(start, end)
        {
            if (definitions.Count < 1)
                throw new ArgumentException("need at least 1 definition", "definitions");

            BaseType = baseType;
            Final = final;
            Definitions = definitions.AsReadOnly();
        }

        public override TStmt Accept<TDoc, TDecl, TStmt, TExpr>(IAstVisitor<TDoc, TDecl, TStmt, TExpr> visitor)
        {
            return visitor.Visit(this);
        }

        public override void SetParent(Statement parent)
        {
            Parent = parent;

            foreach (var definition in Definitions)
            {
                if (definition.Value != null)
                    definition.Value.SetParent(null);
            }
        }
    }
}
