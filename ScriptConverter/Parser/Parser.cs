using System;
using System.Collections.Generic;

namespace ScriptConverter.Parser
{
    abstract class Parser
    {
        private IEnumerator<ScriptToken> _tokens;
        private List<ScriptToken> _read;

        protected Parser(IEnumerable<ScriptToken> tokens)
        {
            _tokens = tokens.GetEnumerator();
            _read = new List<ScriptToken>(8);
        }

        /// <summary>
        /// Returns the token that was most recently taken.
        /// </summary>
        public ScriptToken Previous { get; private set; }

        public IEnumerable<T> ParseSeparatedBy<T>(ScriptTokenType separator, Func<Parser, bool, T> parseFunc) where T : class
        {
            var first = parseFunc(this, true);

            if (first == null)
                yield break;

            yield return first;

            while (MatchAndTake(separator))
            {
                var next = parseFunc(this, false);

                if (next == null)
                    yield break;

                yield return next;
            }
        }

        /// <summary>
        /// Check if the next token matches the given type. If they match, take the token.
        /// </summary>
        public bool MatchAndTake(ScriptTokenType type)
        {
            var isMatch = Match(type);
            if (isMatch)
                Take();

            return isMatch;
        }

        /// <summary>
        /// Check if the next token matches the given type.
        /// </summary>
        public bool Match(ScriptTokenType type, int distance = 0)
        {
            return Peek(distance).Type == type;
        }

        /// <summary>
        /// Take a token from the stream. Throws an exception if the given type does not match the token type.
        /// </summary>
        public ScriptToken Take(ScriptTokenType type)
        {
            var token = Take();

            if (token.Type != type)
                throw new CompilerException(token, CompilerError.ExpectedButFound, type, token);

            return token;
        }

        /// <summary>
        /// Take a token from the stream.
        /// </summary>
        public ScriptToken Take()
        {
            Peek();

            var result = _read[0];
            _read.RemoveAt(0);

            Previous = result;

            return result;
        }

        /// <summary>
        /// Peek at future tokens in the stream. Distance is the number of tokens from the current one.
        /// </summary>
        public ScriptToken Peek(int distance = 0)
        {
            if (distance < 0)
                throw new ArgumentOutOfRangeException("distance", "distance can't be negative");

            while (_read.Count <= distance)
            {
                _tokens.MoveNext();
                _read.Add(_tokens.Current);

                //Console.WriteLine(_tokens.Current.Type);
            }

            return _read[distance];
        }
    }
}
