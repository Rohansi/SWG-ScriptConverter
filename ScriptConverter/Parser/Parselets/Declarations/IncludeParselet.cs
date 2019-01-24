using System;
using System.Linq;
using ScriptConverter.Ast.Declarations;

namespace ScriptConverter.Parser.Parselets.Declarations
{
    class IncludeParselet : IDeclarationParselet
    {
        public Declaration Parse(ScriptParser parser, ScriptToken token)
        {
            var done = false;
            var tokens = parser.ParseSeparatedBy(ScriptTokenType.Dot, (_, first) =>
            {
                if (done)
                    return null;

                if (parser.Match(ScriptTokenType.Multiply))
                {
                    done = true;
                    return parser.Take(ScriptTokenType.Multiply);
                }

                return parser.Take(ScriptTokenType.Identifier);
            });

            var name = string.Join(".", tokens.Select(t => t.Contents));

            if (!parser.MatchAndTake(ScriptTokenType.Semicolon))
                Console.WriteLine("JFjngkjasnholjhnaskl");

            return new IncludeDeclaration(token, parser.Previous, name);
        }
    }
}
