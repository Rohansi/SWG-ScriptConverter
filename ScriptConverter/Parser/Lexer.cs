using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace ScriptConverter.Parser
{
    abstract partial class Lexer<TToken, TTokenType> : IEnumerable<TToken>
           where TToken : Token<TTokenType>
    {
        protected delegate bool LexerRule(char ch, out TToken token);

        private readonly IEnumerable<char> _sourceEnumerable;
        private IEnumerator<char> _source;
        private int _length;
        private List<char> _read;
        private int _index;

        protected string FileName { get; private set; }
        protected int Line { get; private set; }
        protected int Column { get; private set; }
        protected int StartLine { get; private set; }
        protected int StartColumn { get; private set; }

        protected List<LexerRule> Rules { get; set; }

        protected bool AtEof
        {
            get { return _index >= _length; }
        }

        protected Lexer(IEnumerable<char> source, string fileName = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            FileName = fileName;
            _sourceEnumerable = source;
        }

        public IEnumerator<TToken> GetEnumerator()
        {
            if (Rules == null)
                throw new Exception("Rules must be set");

            if (EofToken == null)
                throw new Exception("EofToken must be set");

            _length = int.MaxValue;
            _source = _sourceEnumerable.GetEnumerator();
            _read = new List<char>(16);

            _index = 0;

            Line = 1;
            Column = 1;

            while (_index < _length)
            {
                SkipWhiteSpace();

                if (SkipComment())
                    continue;

                if (_index >= _length)
                    break;

                StartLine = Line;
                StartColumn = Column;

                var ch = PeekChar();
                TToken token = null;

                if (!Rules.Any(rule => rule(ch, out token)))
                    throw Error("Unexpected character '{0}'", ch);

                yield return token;
            }

            while (true)
                yield return EofToken;
        }

        protected virtual bool SkipComment()
        {
            return false;
        }

        protected void SkipWhiteSpace()
        {
            while (_index < _length)
            {
                var ch = PeekChar();

                if (!char.IsWhiteSpace(ch))
                    break;

                TakeChar();
            }
        }

        protected bool TakeIfNext(string value)
        {
            if (!IsNext(value))
                return false;

            for (var i = 0; i < value.Length; i++)
                TakeChar();

            return true;
        }

        protected bool IsNext(string value)
        {
            if (_index + value.Length > _length)
                return false;

            return !value.Where((t, i) => PeekChar(i) != t).Any();
        }

        protected string TakeWhile(Func<char, bool> condition)
        {
            var sb = new StringBuilder();

            while (_index < _length)
            {
                var ch = PeekChar();

                if (!condition(ch))
                    break;

                sb.Append(TakeChar());
            }

            return sb.ToString();
        }

        protected char TakeChar()
        {
            var result = TakeCharImpl();

            if (result == '\n')
            {
                Line++;
                Column = 0;
            }

            if (result == '\r')
            {
                if (PeekChar() == '\n')
                    TakeCharImpl();

                Line++;
                Column = 0;
            }

            Column++;

            return result;
        }

        private char TakeCharImpl()
        {
            PeekChar();

            var result = _read[0];
            _read.RemoveAt(0);
            _index++;

            return result;
        }

        protected string PeekString(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException("length", "distance must be at least 1");

            var sb = new StringBuilder(length);

            for (var i = 0; i < length; i++)
            {
                sb.Append(PeekChar(i));
            }

            return sb.ToString();
        }

        protected char PeekChar(int distance = 0)
        {
            if (distance < 0)
                throw new ArgumentOutOfRangeException("distance", "distance can't be negative");

            while (_read.Count <= distance)
            {
                var success = _source.MoveNext();
                _read.Add(success ? _source.Current : '\0');

                if (!success)
                    _length = _index + _read.Count - 1;
            }

            return _read[distance];
        }

        [StringFormatMethod("format")]
        protected Exception Error(string format, params object[] args)
        {
            return new LexerException(FileName, new SourcePosition(Line, Column), format, args);
        }

        [StringFormatMethod("format")]
        protected Exception ErrorStart(string format, params object[] args)
        {
            return new LexerException(FileName, new SourcePosition(StartLine, StartColumn), format, args);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
