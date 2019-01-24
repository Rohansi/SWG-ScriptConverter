using System;
using System.Collections;
using System.Collections.Generic;

namespace ScriptConverter.Parser
{
    abstract partial class Lexer<TToken, TTokenType>
    {
        protected static TToken EofToken;

        static Lexer()
        {
            EofToken = null;
        }

        protected class OperatorDictionary<T> : IEnumerable<object>
        {
            private readonly GenericComparer<Tuple<string, T>> _comparer;
            private Dictionary<char, List<Tuple<string, T>>> _operatorDictionary;

            public OperatorDictionary()
            {
                _comparer = new GenericComparer<Tuple<string, T>>((a, b) => b.Item1.Length - a.Item1.Length);
                _operatorDictionary = new Dictionary<char, List<Tuple<string, T>>>();
            }

            public void Add(string op, T type)
            {
                List<Tuple<string, T>> list;
                if (!_operatorDictionary.TryGetValue(op[0], out list))
                {
                    list = new List<Tuple<string, T>>();
                    _operatorDictionary.Add(op[0], list);
                }

                list.Add(Tuple.Create(op, type));
                list.Sort(_comparer);
            }

            public IEnumerable<Tuple<string, T>> Lookup(char ch)
            {
                List<Tuple<string, T>> list;
                if (!_operatorDictionary.TryGetValue(ch, out list))
                    return null;

                return list;
            }

            public IEnumerator<object> GetEnumerator()
            {
                throw new NotSupportedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
