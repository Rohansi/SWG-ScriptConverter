using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ScriptConverter.Ast.Declarations;

namespace ScriptConverter.Ast
{
    class Document
    {
        public ReadOnlyCollection<Declaration> Declarations { get; private set; }

        public Document(IEnumerable<Declaration> declarations)
        {
            Declarations = declarations
                .ToList()
                .AsReadOnly();
        }
    }
}
