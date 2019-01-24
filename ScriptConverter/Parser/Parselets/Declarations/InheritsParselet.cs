using System;
using System.Linq;
using ScriptConverter.Ast.Declarations;

namespace ScriptConverter.Parser.Parselets.Declarations
{
    class InheritsParselet : IDeclarationParselet
    {
        public Declaration Parse(ScriptParser parser, ScriptToken token)
        {
            var tokens = parser.ParseSeparatedBy(ScriptTokenType.Dot, (_, first) => parser.Take(ScriptTokenType.Identifier));

            var name = string.Join(".", tokens.Select(t => t.Contents));

            if (!parser.MatchAndTake(ScriptTokenType.Semicolon))
                Console.WriteLine("fkshgnbkashnkhnasb");

            return new InheritsDeclaration(token, parser.Previous, name);
        }
    }
}
