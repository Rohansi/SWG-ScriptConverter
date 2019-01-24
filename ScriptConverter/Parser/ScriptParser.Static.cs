using System.Collections.Generic;
using ScriptConverter.Parser.Parselets;
using ScriptConverter.Parser.Parselets.Declarations;
using ScriptConverter.Parser.Parselets.Expressions;
using ScriptConverter.Parser.Parselets.Statements;

namespace ScriptConverter.Parser
{
    partial class ScriptParser
    {
        private static Dictionary<ScriptTokenType, IDeclarationParselet> _declarationParselets;
        private static Dictionary<ScriptTokenType, IStatementParselet> _statementParselets;
        private static Dictionary<ScriptTokenType, IPrefixParselet> _prefixParselets;
        private static Dictionary<ScriptTokenType, IInfixParselet> _infixParselets;

        static ScriptParser()
        {
            _declarationParselets = new Dictionary<ScriptTokenType, IDeclarationParselet>();
            _statementParselets = new Dictionary<ScriptTokenType, IStatementParselet>();
            _prefixParselets = new Dictionary<ScriptTokenType, IPrefixParselet>();
            _infixParselets = new Dictionary<ScriptTokenType, IInfixParselet>();

            // declarations
            RegisterDeclaration(ScriptTokenType.Const, new ConstParselet());
            RegisterDeclaration(ScriptTokenType.Include, new IncludeParselet());
            RegisterDeclaration(ScriptTokenType.Inherits, new InheritsParselet());
            RegisterDeclaration(ScriptTokenType.Identifier, new MethodParselet());
            RegisterDeclaration(ScriptTokenType.Public, new PublicParselet());

            // statements
            RegisterStatement(ScriptTokenType.LeftBrace, new BlockParselet());
            RegisterStatement(ScriptTokenType.Break, new BreakParselet());
            RegisterStatement(ScriptTokenType.Const, new ConstVariableParselet());
            RegisterStatement(ScriptTokenType.Continue, new ContinueParselet());
            RegisterStatement(ScriptTokenType.Do, new DoParselet());
            RegisterStatement(ScriptTokenType.Semicolon, new EmptyParselet());
            RegisterStatement(ScriptTokenType.For, new ForParselet());
            RegisterStatement(ScriptTokenType.If, new IfParselet());
            RegisterStatement(ScriptTokenType.Return, new ReturnParselet());
            RegisterStatement(ScriptTokenType.Switch, new SwitchParselet());
            RegisterStatement(ScriptTokenType.Throw, new ThrowParselet());
            RegisterStatement(ScriptTokenType.Try, new TryParselet());
            RegisterStatement(ScriptTokenType.Identifier, new VariableParselet());
            RegisterStatement(ScriptTokenType.While, new WhileParselet());

            // expressions
            RegisterPrefix(ScriptTokenType.LeftBrace, new ArrayInitializerParselet());
            RegisterInfix(ScriptTokenType.Add, new BinaryOperatorParselet(PrecedenceValue.Additive, false));
            RegisterInfix(ScriptTokenType.Subtract, new BinaryOperatorParselet(PrecedenceValue.Additive, false));
            RegisterInfix(ScriptTokenType.Multiply, new BinaryOperatorParselet(PrecedenceValue.Multiplicative, false));
            RegisterInfix(ScriptTokenType.Divide, new BinaryOperatorParselet(PrecedenceValue.Multiplicative, false));
            RegisterInfix(ScriptTokenType.Remainder, new BinaryOperatorParselet(PrecedenceValue.Multiplicative, false));
            RegisterInfix(ScriptTokenType.BitwiseShiftLeft, new BinaryOperatorParselet(PrecedenceValue.BitwiseShift, false));
            RegisterInfix(ScriptTokenType.BitwiseShiftRight, new BinaryOperatorParselet(PrecedenceValue.BitwiseShift, false));
            RegisterInfix(ScriptTokenType.BitwiseAnd, new BinaryOperatorParselet(PrecedenceValue.BitwiseAnd, false));
            RegisterInfix(ScriptTokenType.BitwiseOr, new BinaryOperatorParselet(PrecedenceValue.BitwiseOr, false));
            RegisterInfix(ScriptTokenType.BitwiseXor, new BinaryOperatorParselet(PrecedenceValue.BitwiseXor, false));
            RegisterInfix(ScriptTokenType.LogicalAnd, new BinaryOperatorParselet(PrecedenceValue.LogicalAnd, false));
            RegisterInfix(ScriptTokenType.LogicalOr, new BinaryOperatorParselet(PrecedenceValue.LogicalOr, false));
            RegisterInfix(ScriptTokenType.EqualTo, new BinaryOperatorParselet(PrecedenceValue.Equality, false));
            RegisterInfix(ScriptTokenType.NotEqualTo, new BinaryOperatorParselet(PrecedenceValue.Equality, false));
            RegisterInfix(ScriptTokenType.GreaterThan, new BinaryOperatorParselet(PrecedenceValue.Relational, false));
            RegisterInfix(ScriptTokenType.GreaterThanOrEqual, new BinaryOperatorParselet(PrecedenceValue.Relational, false));
            RegisterInfix(ScriptTokenType.LessThan, new BinaryOperatorParselet(PrecedenceValue.Relational, false));
            RegisterInfix(ScriptTokenType.LessThanOrEqual, new BinaryOperatorParselet(PrecedenceValue.Relational, false));
            RegisterInfix(ScriptTokenType.Assign, new BinaryOperatorParselet(PrecedenceValue.Assignment, true));
            RegisterInfix(ScriptTokenType.AddAssign, new BinaryOperatorParselet(PrecedenceValue.Assignment, true));
            RegisterInfix(ScriptTokenType.SubtractAssign, new BinaryOperatorParselet(PrecedenceValue.Assignment, true));
            RegisterInfix(ScriptTokenType.MultiplyAssign, new BinaryOperatorParselet(PrecedenceValue.Assignment, true));
            RegisterInfix(ScriptTokenType.DivideAssign, new BinaryOperatorParselet(PrecedenceValue.Assignment, true));
            RegisterInfix(ScriptTokenType.RemainderAssign, new BinaryOperatorParselet(PrecedenceValue.Assignment, true));
            RegisterInfix(ScriptTokenType.BitwiseShiftLeftAssign, new BinaryOperatorParselet(PrecedenceValue.Assignment, true));
            RegisterInfix(ScriptTokenType.BitwiseShiftRightAssign, new BinaryOperatorParselet(PrecedenceValue.Assignment, true));
            RegisterInfix(ScriptTokenType.BitwiseAndAssign, new BinaryOperatorParselet(PrecedenceValue.Assignment, true));
            RegisterInfix(ScriptTokenType.BitwiseOrAssign, new BinaryOperatorParselet(PrecedenceValue.Assignment, true));
            RegisterInfix(ScriptTokenType.BitwiseXorAssign, new BinaryOperatorParselet(PrecedenceValue.Assignment, true));
            RegisterPrefix(ScriptTokenType.True, new BoolParselet(true));
            RegisterPrefix(ScriptTokenType.False, new BoolParselet(false));
            RegisterInfix(ScriptTokenType.LeftParen, new CallParselet());
            RegisterInfix(ScriptTokenType.Dot, new FieldParselet());
            RegisterPrefix(ScriptTokenType.LeftParen, new GroupParselet());
            RegisterPrefix(ScriptTokenType.Hash, new HashParselet());
            RegisterPrefix(ScriptTokenType.Identifier, new IdentifierParselet());
            RegisterInfix(ScriptTokenType.LeftSquare, new IndexerParselet());
            RegisterInfix(ScriptTokenType.InstanceOf, new InstanceOfParselet());
            RegisterPrefix(ScriptTokenType.New, new NewParselet());
            RegisterPrefix(ScriptTokenType.Null, new NullParselet());
            RegisterPrefix(ScriptTokenType.Number, new NumberParselet());
            RegisterInfix(ScriptTokenType.Increment, new PostfixOperatorParselet(PrecedenceValue.Suffix));
            RegisterInfix(ScriptTokenType.Decrement, new PostfixOperatorParselet(PrecedenceValue.Suffix));
            RegisterPrefix(ScriptTokenType.Add, new PrefixOperatorParselet(PrecedenceValue.Prefix));
            RegisterPrefix(ScriptTokenType.Subtract, new PrefixOperatorParselet(PrecedenceValue.Prefix));
            RegisterPrefix(ScriptTokenType.BitwiseNot, new PrefixOperatorParselet(PrecedenceValue.Prefix));
            RegisterPrefix(ScriptTokenType.LogicalNot, new PrefixOperatorParselet(PrecedenceValue.Prefix));
            RegisterPrefix(ScriptTokenType.Increment, new PrefixOperatorParselet(PrecedenceValue.Prefix));
            RegisterPrefix(ScriptTokenType.Decrement, new PrefixOperatorParselet(PrecedenceValue.Prefix));
            RegisterPrefix(ScriptTokenType.String, new StringParselet(false));
            RegisterPrefix(ScriptTokenType.SingleString, new StringParselet(true));
            RegisterInfix(ScriptTokenType.QuestionMark, new TernaryParselet());
        }

        static void RegisterDeclaration(ScriptTokenType type, IDeclarationParselet parselet)
        {
            _declarationParselets.Add(type, parselet);
        }

        static void RegisterStatement(ScriptTokenType type, IStatementParselet parselet)
        {
            _statementParselets.Add(type, parselet);
        }

        static void RegisterPrefix(ScriptTokenType type, IPrefixParselet parselet)
        {
            _prefixParselets.Add(type, parselet);
        }

        static void RegisterInfix(ScriptTokenType type, IInfixParselet parselet)
        {
            _infixParselets.Add(type, parselet);
        }
    }
}
