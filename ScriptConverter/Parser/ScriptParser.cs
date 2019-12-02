using System;
using System.Collections.Generic;
using ScriptConverter.Ast;
using ScriptConverter.Ast.Declarations;
using ScriptConverter.Ast.Expressions;
using ScriptConverter.Ast.Statements;
using ScriptConverter.Parser.Parselets;

namespace ScriptConverter.Parser
{
    partial class ScriptParser : Parser
    {
        public ScriptParser(IEnumerable<ScriptToken> tokens)
            : base(tokens)
        {

        }

        public void ParseNamedType(out ScriptType type, out string name, out ScriptToken typeToken, out ScriptToken nameToken)
        {
            typeToken = Take();
            ParseNamedType(typeToken, out type, out name, out nameToken);
        }

        public void ParseNamedType(ScriptToken token, out ScriptType type, out string name, out ScriptToken nameToken)
        {
            type = ParseType(token);

            nameToken = Take(ScriptTokenType.Identifier);
            name = nameToken.Contents;

            var arrayDims = 0;
            while (MatchAndTake(ScriptTokenType.LeftSquare))
            {
                Take(ScriptTokenType.RightSquare);
                arrayDims++;
            }

            if (arrayDims == 0)
                return;

            type = new ScriptType(type.Name, type.ArrayDimensions + arrayDims, type.IsResizable);
        }

        public ScriptType ParseType()
        {
            return ParseType(Take());
        }

        public ScriptType ParseType(ScriptToken token)
        {
            if (token.Type != ScriptTokenType.Identifier)
                throw new CompilerException(token, CompilerError.ExpectedButFound, ScriptTokenType.Identifier, token);

            var isResizable = false;
            var arrayDims = 0;

            if (token.Contents == "resizeable") // yes
            {
                isResizable = true;
                token = Take(ScriptTokenType.Identifier);
            }

            var typeName = token.Contents;

            if (Match(ScriptTokenType.Dot))
            {
                typeName += "." + string.Join(".", ParseSeparatedBy(ScriptTokenType.Dot, (_, first) =>
                {
                    if (first)
                        Take(ScriptTokenType.Dot);

                    return Take(ScriptTokenType.Identifier).Contents;
                }));
            }

            if (isResizable)
            {
                Take(ScriptTokenType.LeftSquare);
                Take(ScriptTokenType.RightSquare);
            }
            else
            {
                while (MatchAndTake(ScriptTokenType.LeftSquare))
                {
                    Take(ScriptTokenType.RightSquare);
                    arrayDims++;
                }
            }

            return new ScriptType(typeName, arrayDims, isResizable);
        }

        /// <summary>
        /// Parse an expression into an expression tree. You can think of expressions as sub-statements.
        /// </summary>
        public Expression ParseExpression(int precendence = 0)
        {
            var token = Take();

            IPrefixParselet prefixParselet;
            _prefixParselets.TryGetValue(token.Type, out prefixParselet);

            if (prefixParselet == null)
                throw new CompilerException(token, CompilerError.ExpectedButFound, "Expression", token);

            var left = prefixParselet.Parse(this, token);

            while (GetPrecedence() > precendence)
            {
                token = Take();

                IInfixParselet infixParselet;
                _infixParselets.TryGetValue(token.Type, out infixParselet);

                if (infixParselet == null)
                    throw new Exception("probably can't happen");

                left = infixParselet.Parse(this, left, token);
            }

            return left;
        }

        /// <summary>
        /// Parse a statement into an expression tree.
        /// </summary>
        public Statement ParseStatement(bool takeTrailingSemicolon = true)
        {
            var token = Peek();

            IStatementParselet statementParselet;
            _statementParselets.TryGetValue(token.Type, out statementParselet);

            // HACK: workaround for variables
            if (token.Type == ScriptTokenType.Identifier && !IsVariableStatement())
                statementParselet = null;

            if (statementParselet == null)
            {
                var expr = ParseExpression();

                if (takeTrailingSemicolon)
                    Take(ScriptTokenType.Semicolon);

                return new NakedStatement(expr);
            }

            token = Take();

            bool hasTrailingSemicolon;
            var result = statementParselet.Parse(this, token, out hasTrailingSemicolon);

            if (takeTrailingSemicolon && hasTrailingSemicolon)
                Take(ScriptTokenType.Semicolon);

            return result;
        }

        /// <summary>
        /// Parse a block of code into an expression tree. Blocks can either be a single statement or 
        /// multiple surrounded by braces.
        /// </summary>
        public BlockStatement ParseBlock(bool allowSingle = true)
        {
            ScriptToken start;
            ScriptToken end;
            var statements = new List<Statement>();

            if (allowSingle && !Match(ScriptTokenType.LeftBrace))
            {
                start = Peek();

                statements.Add(ParseStatement());

                end = Previous;

                return new BlockStatement(start, end, statements);
            }

            start = Take(ScriptTokenType.LeftBrace);

            while (!Match(ScriptTokenType.RightBrace))
            {
                statements.Add(ParseStatement());
            }

            end = Take(ScriptTokenType.RightBrace);

            return new BlockStatement(start, end, statements);
        }

        /// <summary>
        /// Parses declarations until there are no more tokens available.
        /// </summary>
        public Document ParseAll()
        {
            var declarations = new List<Declaration>();

            while (!Match(ScriptTokenType.Eof))
            {
                var token = Take();

                if (token.Type == ScriptTokenType.Semicolon)
                    continue;

                IDeclarationParselet declarationParselet;
                if (!_declarationParselets.TryGetValue(token.Type, out declarationParselet))
                    throw new CompilerException(token, "expected declaration"); // TODO

                declarations.Add(declarationParselet.Parse(this, token));
            }

            return new Document(declarations);
        }

        private bool IsVariableStatement()
        {
            if (!Match(ScriptTokenType.Identifier))
                return false;

            var i = 1;
            while (Match(ScriptTokenType.Dot, i) && Match(ScriptTokenType.Identifier, i + 1))
            {
                i += 2;
            }

            return Match(ScriptTokenType.Identifier, i) ||
                (Match(ScriptTokenType.LeftSquare, i) && Match(ScriptTokenType.RightSquare, i + 1));
        }

        private int GetPrecedence()
        {
            IInfixParselet infixParselet;
            _infixParselets.TryGetValue(Peek().Type, out infixParselet);

            return infixParselet != null ? infixParselet.Precedence : 0;
        }
    }
}
