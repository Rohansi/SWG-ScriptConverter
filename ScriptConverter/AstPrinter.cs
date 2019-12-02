using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScriptConverter.Ast;
using ScriptConverter.Ast.Declarations;
using ScriptConverter.Ast.Expressions;
using ScriptConverter.Ast.Statements;
using ScriptConverter.Parser;

namespace ScriptConverter
{
    // TODO: make == to .equals() conversion use type info

    class AstPrinter : IAstVisitor<int, int, int, int>
    {
        private IndentTextWriter _writer;
        private readonly string _package;
        private readonly string _name;
        private readonly bool _isLib;

        public AstPrinter(string package, string name, bool isLib)
            : this(Console.Out, package, name, isLib)
        {
            
        }

        public AstPrinter(TextWriter writer, string package, string name, bool isLib)
        {
            _writer = new IndentTextWriter(writer, "  ");
            _package = package;
            _name = name;
            _isLib = isLib;

            _scopeStack = new Stack<Dictionary<string, ScriptType>>();
        }

        public int Visit(Document document)
        {
            _writer.Write("package ");
            _writer.Write(_package);
            _writer.WriteLine(";");
            _writer.WriteLine();

            _writer.WriteLine("import script.*;");
            _writer.WriteLine("import script.base_class.*;");
            _writer.WriteLine("import script.combat_engine.*;");
            _writer.WriteLine("import java.util.Arrays;");
            _writer.WriteLine("import java.util.Hashtable;");
            _writer.WriteLine("import java.util.Vector;");
            _writer.WriteLine("import script.base_script;");
            _writer.WriteLine();

            foreach (var include in document.Declarations.OfType<IncludeDeclaration>())
            {
                include.Accept(this);
            }

            _writer.WriteLine();

            var inherits = document.Declarations.OfType<InheritsDeclaration>().SingleOrDefault();

            _writer.Write("public class ");
            _writer.Write(_name);

            if (_name == "base_script")
            {
                _writer.Write(" extends script.base_class");
            }
            else
            {
                _writer.Write(" extends script.");
                _writer.Write(inherits != null ? inherits.Name : "base_script");
            }

            _writer.WriteLine();
            _writer.WriteLine("{");
            _writer.Indent++;

            _writer.Write("public ");
            _writer.Write(_name);
            _writer.WriteLine("() { }");
            _writer.WriteLine();

            PushScope();

            foreach (var constant in document.Declarations.OfType<FieldDeclaration>())
            {
                constant.SetParent();
                constant.Accept(this);
            }

            _writer.WriteLine();

            foreach (var method in document.Declarations.OfType<MethodDeclaration>())
            {
                method.SetParent();
                method.Accept(this);
                _writer.WriteLine();
            }

            PopScope();

            _writer.Indent--;
            _writer.WriteLine("}");

            _writer.WriteLine();
            return 0;
        }

        #region Declarations

        public int Visit(FieldDeclaration declaration)
        {
            Declare(declaration.Name, declaration.Type);

            _writer.WriteLeading(declaration.ModifierToken);
            if (declaration.IsPublic)
                _writer.Write("public");
            if (declaration.IsPublic && declaration.IsConstant)
                _writer.Write(' ');
            if (declaration.IsConstant)
                _writer.Write("static final");
            _writer.WriteTrailing(declaration.ModifierToken);

            _writer.WriteLeading(declaration.TypeToken);
            _writer.Write(declaration.Type);
            _writer.WriteTrailing(declaration.TypeToken);
            
            _writer.WriteLeading(declaration.NameToken);
            _writer.Write(declaration.Name);
            _writer.WriteTrailing(declaration.NameToken);

            _writer.WriteLeading(declaration.AssignmentToken);
            _writer.Write("=");
            _writer.WriteTrailing(declaration.AssignmentToken);

            declaration.Value.Accept(this);

            _writer.WriteLeading(declaration.SemicolonToken);
            _writer.Write(";");
            _writer.WriteTrailing(declaration.SemicolonToken);
            return 0;
        }

        public int Visit(IncludeDeclaration declaration)
        {
            _writer.Write("import ");

            if (!declaration.Name.StartsWith("java."))
                _writer.Write("script.");

            _writer.Write(declaration.Name);
            _writer.WriteLine(";");
            return 0;
        }

        public int Visit(InheritsDeclaration declaration)
        {
            throw new NotSupportedException();
        }

        public int Visit(MethodDeclaration declaration)
        {
            List<MethodDeclaration.Parameter> extraParameters;
            var isEventHandler = EventHandlers.TryGetValue(declaration.ReturnType.ToString(), out extraParameters);

            var extras = isEventHandler ? extraParameters : Enumerable.Empty<MethodDeclaration.Parameter>();
            var parameters = extras.Concat(declaration.Parameters).ToList();

            _methodReturnType = declaration.ReturnType;
            _methodParameters = parameters.ToDictionary(p => p.Name, p => p.Type);

            _writer.Write("public ");

            if (_isLib)
                _writer.Write("static ");

            _writer.Write(isEventHandler ? "int" : declaration.ReturnType.ToString());
            _writer.Write(" ");
            _writer.Write(declaration.Name);
            _writer.Write("(");

            var first = true;
            foreach (var parameter in parameters)
            {
                if (!first)
                    _writer.Write(", ");

                first = false;

                _writer.Write(parameter.Type);
                _writer.Write(" ");
                _writer.Write(parameter.Name);
            }

            _writer.WriteLine(") throws InterruptedException");

            declaration.Block.Accept(this);

            return 0;
        }

        #endregion

        #region Statements

        public int Visit(BlockStatement statement)
        {
            if (!statement.IsSwitch)
            {
                _writer.WriteLine("{");
                PushScope();
            }

            _writer.Indent++;

            foreach (var subStatement in statement.Statements)
            {
                var nakedStmt = subStatement as NakedStatement;
                if (nakedStmt != null)
                    nakedStmt.Expression.SetParent(null);

                subStatement.Accept(this);
            }

            _writer.Indent--;

            if (!statement.IsSwitch)
            {
                PopScope();
                _writer.WriteLine("}");
            }

            return 0;
        }

        public int Visit(BreakStatement statement)
        {
            _writer.WriteLine("break;");
            return 0;
        }

        public int Visit(ContinueStatement statement)
        {
            _writer.WriteLine("continue;");
            return 0;
        }

        public int Visit(DoStatement statement)
        {
            _writer.WriteLine("do");
            statement.Block.Accept(this);
            _writer.Write("while (");
            statement.Condition.Accept(this);
            _writer.WriteLine(");");
            return 0;
        }

        public int Visit(EmptyStatement statement)
        {
            _writer.WriteLine(";");
            return 0;
        }

        public int Visit(ForStatement statement)
        {
            _writer.Write("for (");

            if (statement.Initializers != null)
            {
                var first = true;
                foreach (var initializer in statement.Initializers)
                {
                    if (!first)
                        _writer.Write(", ");

                    first = false;

                    var initVariable = initializer as VariableStatement;
                    if (initVariable != null)
                    {
                        if (initVariable.Final)
                            throw new Exception("why");

                        _writer.Write(initVariable.BaseType);
                        _writer.Write(" ");

                        var firstVar = true;
                        foreach (var definition in initVariable.Definitions)
                        {
                            if (!firstVar)
                                _writer.Write(", ");

                            firstVar = false;

                            _writer.Write(definition.Name);

                            var i = initVariable.BaseType.ArrayDimensions;
                            while (definition.Type.ArrayDimensions > i++)
                            {
                                _writer.Write("[]");
                            }

                            if (definition.Value != null)
                            {
                                _writer.Write(" = ");
                                definition.Value.Accept(this);
                            }
                        }
                    }

                    var initExpr = initializer as NakedStatement;
                    if (initExpr != null)
                    {
                        initExpr.Expression.Accept(this);
                    }
                }
            }

            _writer.Write("; ");

            if (statement.Condition != null)
                statement.Condition.Accept(this);

            _writer.Write("; ");

            if (statement.Increment != null)
            {
                var first = true;
                foreach (var increment in statement.Increment)
                {
                    if (!first)
                        _writer.Write(", ");

                    first = false;
                    increment.Accept(this);
                }
            }

            _writer.WriteLine(")");
            statement.Block.Accept(this);
            return 0;
        }

        public int Visit(IfStatement statement)
        {
            for (var i = 0; i < statement.Branches.Count; i++)
            {
                var branch = statement.Branches[i];

                _writer.Write(i == 0 ? "if" : "else if");

                _writer.Write(" (");
                branch.Condition.Accept(this);
                _writer.WriteLine(")");

                branch.Block.Accept(this);
            }

            if (statement.DefaultBranch != null)
            {
                _writer.WriteLine("else ");
                statement.DefaultBranch.Block.Accept(this);
            }

            return 0;
        }

        public int Visit(NakedStatement statement)
        {
            statement.Expression.Accept(this);
            _writer.WriteLine(";");
            return 0;
        }

        public int Visit(ReturnStatement statement)
        {
            if (statement.Value != null)
            {
                var valueIdentifier = statement.Value as IdentifierExpression;
                if (valueIdentifier != null)
                {
                    var type = GetType(valueIdentifier.Name);
                    if (type != null && type.IsResizable)
                    {
                        if (_methodReturnType != null && _methodReturnType.IsArray && !_methodReturnType.IsResizable)
                        {
                            var result = ResizableToArray(valueIdentifier.Name, _methodReturnType);
                            _writer.WriteLine("return {0};", result);
                            return 0;
                        }
                    }
                }

                _writer.Write("return ");
                statement.Value.Accept(this);
                _writer.WriteLine(";");
                return 0;
            }

            _writer.WriteLine("return;");
            return 0;
        }

        public int Visit(SwitchStatement statement)
        {
            _writer.Write("switch (");
            statement.Expression.Accept(this);
            _writer.WriteLine(")");

            _writer.WriteLine("{");
            _writer.Indent++;

            for (var i = 0; i < statement.Branches.Count; i++)
            {
                var branch = statement.Branches[i];

                foreach (var condition in branch.Conditions)
                {
                    if (condition == null)
                    {
                        _writer.WriteLine("default:");
                        continue;
                    }

                    _writer.Write("case ");
                    condition.Accept(this);
                    _writer.WriteLine(":");
                }

                branch.Block.Accept(this);

                if (i < statement.Branches.Count - 1)
                    _writer.WriteLine();
            }

            _writer.WriteLine();

            _writer.Indent--;
            _writer.WriteLine("}");
            return 0;
        }

        public int Visit(ThrowStatement statement)
        {
            _writer.Write("throw ");
            statement.Value.Accept(this);
            _writer.WriteLine(";");
            return 0;
        }

        public int Visit(TryStatement statement)
        {
            _writer.WriteLine("try");
            statement.Block.Accept(this);

            foreach (var branch in statement.Branches)
            {
                _writer.Write("catch (");
                _writer.Write(branch.Type);
                _writer.Write(" ");
                _writer.Write(branch.Name);
                _writer.WriteLine(")");
                branch.Block.Accept(this);
            }

            return 0;
        }

        public int Visit(VariableStatement statement)
        {
            if (statement.Definitions.Count == 1)
            {
                var def = statement.Definitions[0];

                var targetType = def.Type;

                var valueIdent = def.Value as IdentifierExpression;
                if (valueIdent != null)
                {
                    var valueType = GetType(valueIdent.Name);
                    if (valueType != null)
                    {
                        if (targetType.IsResizable && (valueType.IsArray && !valueType.IsResizable))
                        {
                            Declare(def.Name, def.Type);
                            // Vector def = arrayIdent;

                            var result = ArrayToResizable(valueIdent.Name);
                            _writer.WriteLine("Vector {0} = {1};", def.Name, result);

                            return 0;
                        }

                        if ((targetType.IsArray && !targetType.IsResizable) && valueType.IsResizable)
                        {
                            Declare(def.Name, def.Type);
                            // arrayType[] def = vectorIdent;

                            var result = ResizableToArray(valueIdent.Name, def.Type);
                            _writer.WriteLine("{0} {1} = {2};", targetType, def.Name, result);

                            return 0;
                        }
                    }
                }

                var valueCast = def.Value as CastExpression;
                if (valueCast != null && targetType.IsResizable)
                {
                    var valueType = valueCast.Type;

                    if (valueType.IsResizable && targetType.Name == valueType.Name)
                    {
                        valueType = new ScriptType(valueType.Name, valueType.ArrayDimensions);

                        Declare(def.Name, def.Type);
                        // resizable arrayType[] def = (resizable arrayType[])arrayExpr;

                        var result = ArrayToResizable(valueType, valueCast.Value);
                        _writer.WriteLine("Vector {0} = {1};", def.Name, result);

                        return 0;
                    }
                }
            }

            if (statement.Final)
                _writer.Write("final ");

            if (statement.BaseType.IsResizable)
            {
                var def = statement.Definitions.Single();

                Declare(def.Name, def.Type);

                _writer.Write("Vector ");
                _writer.Write(def.Name);

                if (def.Value == null)
                {
                    _writer.WriteLine(";");
                    return 0;
                }

                _writer.Write(" = ");

                if (def.Value is NewExpression)
                {
                    _writer.WriteLine("new Vector();");
                }
                else
                {
                    def.Value.Accept(this);
                    _writer.WriteLine(";");
                }

                return 0;
            }

            _writer.Write(statement.BaseType);
            _writer.Write(" ");

            var first = true;
            foreach (var definition in statement.Definitions)
            {
                Declare(definition.Name, definition.Type);

                if (!first)
                    _writer.Write(", ");

                first = false;
                
                _writer.Write(definition.Name);

                var i = statement.BaseType.ArrayDimensions;
                while (definition.Type.ArrayDimensions > i++)
                {
                    _writer.Write("[]");
                }

                if (definition.Value != null)
                {
                    _writer.Write(" = ");
                    definition.Value.Accept(this);
                }
            }

            _writer.WriteLine(";");
            return 0;
        }

        public int Visit(WhileStatement statement)
        {
            _writer.Write("while (");
            statement.Condition.Accept(this);
            _writer.WriteLine(")");
            statement.Block.Accept(this);
            return 0;
        }

        #endregion

        #region Expressions

        public int Visit(ArrayInitializerExpression expression)
        {
            _writer.Write("{ ");

            var first = true;
            foreach (var value in expression.Values)
            {
                if (!first)
                    _writer.Write(", ");

                first = false;
                value.Accept(this);
            }

            _writer.Write(" }");
            return 0;
        }

        public int Visit(BinaryOperatorExpression expression)
        {
            if (expression.Operation == ScriptTokenType.EqualTo ||
                expression.Operation == ScriptTokenType.NotEqualTo)
            {
                var leftString = expression.Left as StringExpression;
                var rightString = expression.Right as StringExpression;

                if ((leftString != null && !leftString.IsSingleQuote) || (rightString != null && !rightString.IsSingleQuote))
                {
                    if (expression.Operation == ScriptTokenType.NotEqualTo)
                        _writer.Write("!");

                    expression.Left.Accept(this);
                    _writer.Write(".equals(");
                    expression.Right.Accept(this);
                    _writer.Write(")");

                    return 0;
                }
            }

            if (expression.Operation == ScriptTokenType.Assign)
            {
                var leftIndexer = expression.Left as IndexerExpression;
                if (leftIndexer != null)
                {
                    var leftIdentifier = leftIndexer.Left as IdentifierExpression;
                    if (leftIdentifier != null)
                    {
                        var type = GetType(leftIdentifier.Name);
                        if (type != null && type.IsResizable)
                        {
                            _writer.Write(leftIdentifier.Name);
                            _writer.Write(".set(");
                            leftIndexer.Index.Accept(this);
                            _writer.Write(", ");
                            expression.Right.Accept(this);
                            _writer.Write(")");

                            return 0;
                        }
                    }
                }

                var leftIdent = expression.Left as IdentifierExpression;
                var rightIdent = expression.Right as IdentifierExpression;
                if (expression.Parent == null && leftIdent != null && rightIdent != null)
                {
                    var leftType = GetType(leftIdent.Name);
                    var rightType = GetType(rightIdent.Name);
                    if (leftType != null && rightType != null)
                    {
                        if (leftType.IsResizable && (rightType.IsArray && !rightType.IsResizable))
                        {
                            // <vector> = <array>

                            var result = ArrayToResizable(rightIdent.Name);
                            _writer.Write("{0} = {1}", leftIdent.Name, result);

                            return 0;
                        }

                        if ((leftType.IsArray && !leftType.IsResizable) && rightType.IsResizable)
                        {
                            // <array> = <vector>

                            var result = ResizableToArray(rightIdent.Name, leftType);
                            _writer.Write("{0} = {1}", leftIdent.Name, result);

                            return 0;
                        }
                    }
                }
            }

            var needParens = expression.Parent != null;

            if (needParens)
                _writer.Write("(");

            expression.Left.Accept(this);
            _writer.Write(" ");
            _writer.Write(BinaryOperatorMap[expression.Operation]);
            _writer.Write(" ");
            expression.Right.Accept(this);

            if (needParens)
                _writer.Write(")");

            return 0;
        }

        public int Visit(BoolExpression expression)
        {
            _writer.Write(expression.Value ? "true" : "false");
            return 0;
        }

        public int Visit(CallExpression expression)
        {
            expression.Left.Accept(this);
            _writer.Write("(");

            var first = true;
            foreach (var parameter in expression.Parameters)
            {
                if (!first)
                    _writer.Write(", ");

                first = false;
                parameter.Accept(this);
            }

            _writer.Write(")");
            return 0;
        }

        public int Visit(CastExpression expression)
        {
            _writer.Write("(");
            _writer.Write("(");
            _writer.Write(expression.Type);
            _writer.Write(")");
            expression.Value.Accept(this);
            _writer.Write(")");
            return 0;
        }

        public int Visit(FieldExpression expression)
        {
            if (expression.Name == "length")
            {
                var leftIdentifier = expression.Left as IdentifierExpression;
                if (leftIdentifier != null)
                {
                    var type = GetType(leftIdentifier.Name);
                    if (type != null && type.IsResizable)
                    {
                        _writer.Write(leftIdentifier.Name);
                        _writer.Write(".size()");
                        return 0;
                    }
                }
            }
            expression.Left.Accept(this);
            _writer.Write(".");
            _writer.Write(expression.Name);
            return 0;
        }

        public int Visit(IdentifierExpression expression)
        {
            _writer.Write(expression.Name);
            return 0;
        }

        public int Visit(IndexerExpression expression)
        {
            var leftIdentifier = expression.Left as IdentifierExpression;
            if (leftIdentifier != null)
            {
                var type = GetType(leftIdentifier.Name);
                if (type != null && type.IsResizable)
                {
                    _writer.Write("((");
                    _writer.Write(type.Name);
                    _writer.Write(")");
                    _writer.Write(leftIdentifier.Name);
                    _writer.Write(".get(");
                    expression.Index.Accept(this);
                    _writer.Write("))");

                    return 0;
                }
            }

            expression.Left.Accept(this);
            _writer.Write("[");
            expression.Index.Accept(this);
            _writer.Write("]");
            return 0;
        }

        public int Visit(InstanceOfExpression expression)
        {
            _writer.Write("(");
            expression.Left.Accept(this);
            _writer.Write(" instanceof ");

            var typeName = expression.Type.ToString();

            switch (typeName)
            {
                case "int":
                    typeName = "Integer";
                    break;
                case "float":
                    typeName = "Float";
                    break;
                case "boolean":
                    typeName = "Boolean";
                    break;
            }

            _writer.Write(typeName);
            _writer.Write(")");
            return 0;
        }

        public int Visit(NewExpression expression)
        {
            _writer.Write("new ");
            _writer.Write(expression.Type);
            _writer.Write(expression.IsArray ? "[" : "(");

            {
                var first = true;
                foreach (var parameter in expression.Parameters)
                {
                    if (!first)
                        _writer.Write(", ");

                    first = false;
                    parameter.Accept(this);
                }
            }

            _writer.Write(expression.IsArray ? "]" : ")");

            if (expression.IsArray)
            {
                for (var i = 1; i < expression.ArrayDimensions; i++)
                {
                    _writer.Write("[]");
                }
            }

            if (expression.Initializer != null)
            {
                _writer.Write("{ ");

                var first = true;
                foreach (var value in expression.Initializer)
                {
                    if (!first)
                        _writer.Write(", ");

                    first = false;
                    value.Accept(this);
                }

                _writer.Write(" }");
            }

            return 0;
        }

        public int Visit(NullExpression expression)
        {
            _writer.Write("null");
            return 0;
        }

        public int Visit(NumberExpression expression)
        {
            _writer.Write(expression.Value);
            return 0;
        }

        public int Visit(PostfixOperatorExpression expression)
        {
            expression.Left.Accept(this);
            _writer.Write(PostfixOperatorMap[expression.Operation]);
            return 0;
        }

        public int Visit(PrefixOperatorExpression expression)
        {
            _writer.Write(PrefixOperatorMap[expression.Operation]);
            expression.Right.Accept(this);
            return 0;
        }

        public int Visit(StringExpression expression)
        {
            _writer.Write(expression.IsSingleQuote ? "\'" : "\"");
            _writer.Write(expression.Value);
            _writer.Write(expression.IsSingleQuote ? "\'" : "\"");
            return 0;
        }

        public int Visit(TernaryExpression expression)
        {
            _writer.Write("(");
            expression.Condition.Accept(this);
            _writer.Write(" ? ");
            expression.TrueExpression.Accept(this);
            _writer.Write(" : ");
            expression.FalseExpression.Accept(this);
            _writer.Write(")");
            return 0;
        }

        #endregion

        #region Resizable Conversions

        private static int _resizableIndex;

        private string ResizableToArray(ScriptType inputType, Expression inputExpression, ScriptType arrayType)
        {
            var temp = "__" + _resizableIndex++ + "_expr";

            _writer.Write("{0} {1} = ", inputType, temp);
            inputExpression.Accept(this);
            _writer.WriteLine(";");

            return ResizableToArray(temp, arrayType);
        }

        private string ArrayToResizable(ScriptType inputType, Expression inputExpression)
        {
            var temp = "__" + _resizableIndex++ + "_expr";

            _writer.Write("{0} {1} = ", inputType, temp);
            inputExpression.Accept(this);
            _writer.WriteLine(";");

            return ArrayToResizable(temp);
        }

        private string ResizableToArray(string inputIdentifier, ScriptType arrayType)
        {
            /*
                arrayType[] __array = new arrayType[0];
                if (input != null)
                {
                    __array = new arrayType[input.size()];
                    for (int __i = 0; __i < input.size(); ++__i)
                    {
                        __array[__i] = (arrayType)input.get(__i);
                    }
                }
            */

            var prefix = "__" + (_resizableIndex++) + "_";
            var arrayTemp = prefix + "array";
            var loopTemp = prefix + "i";

            _writer.WriteLine("{1}[] {0} = new {1}[0];", arrayTemp, arrayType.Name);
            _writer.WriteLine("if ({0} != null)", inputIdentifier);
            _writer.WriteLine("{");
            _writer.Indent++;
            _writer.WriteLine("{0} = new {1}[{2}.size()];", arrayTemp, arrayType.Name, inputIdentifier);
            _writer.WriteLine("for (int {0} = 0; {0} < {1}.size(); ++{0})", loopTemp, inputIdentifier);
            _writer.WriteLine("{");
            _writer.Indent++;
            _writer.WriteLine("{0}[{1}] = ({2}){3}.get({1});", arrayTemp, loopTemp, arrayType.Name, inputIdentifier);
            _writer.Indent--;
            _writer.WriteLine("}");
            _writer.Indent--;
            _writer.WriteLine("}");

            return arrayTemp;
        }

        private string ArrayToResizable(string inputIdentifier)
        {
            /*
                Vector __array = new Vector();
                if (input != null)
                {
                    __array.setSize(input.length);
                    for (int __i = 0; __i < input.length; ++__i)
                    {
                        __array.set(__i, input[__i]);
                    }
                }
            */

            var prefix = "__" + (_resizableIndex++) + "_";
            var vectorTemp = prefix + "vector";
            var loopTemp = prefix + "i";

            _writer.WriteLine("Vector {0} = new Vector();", vectorTemp);
            _writer.WriteLine("if ({0} != null)", inputIdentifier);
            _writer.WriteLine("{");
            _writer.Indent++;
            _writer.WriteLine("{0}.setSize({1}.length);", vectorTemp, inputIdentifier);
            _writer.WriteLine("for (int {0} = 0; {0} < {1}.length; ++{0})", loopTemp, inputIdentifier);
            _writer.WriteLine("{");
            _writer.Indent++;
            _writer.WriteLine("{0}.set({1}, {2}[{1}]);", vectorTemp, loopTemp, inputIdentifier);
            _writer.Indent--;
            _writer.WriteLine("}");
            _writer.Indent--;
            _writer.WriteLine("}");

            return vectorTemp;
        }

        #endregion

        #region Scope

        private ScriptType _methodReturnType;
        private Dictionary<string, ScriptType> _methodParameters;
        private Stack<Dictionary<string, ScriptType>> _scopeStack;

        private ScriptType GetType(string name)
        {
            ScriptType type = null;

            if (_methodParameters != null && _methodParameters.TryGetValue(name, out type))
                return type;

            if (_scopeStack.Any(scope => scope.TryGetValue(name, out type)))
                return type;

            // TODO: hopefully nothing inherits a resizable type...
            return null;
            throw new Exception("undefined variable: " + name);
        }

        private void Declare(string name, ScriptType type)
        {
            _scopeStack.Peek().Add(name, type);
        }

        private void PushScope()
        {
            _scopeStack.Push(new Dictionary<string, ScriptType>());
        }

        private void PopScope()
        {
            _scopeStack.Pop();
        }

        #endregion

        #region Operator Maps

        private static readonly Dictionary<ScriptTokenType, string> BinaryOperatorMap = new Dictionary<ScriptTokenType, string>
        {
            { ScriptTokenType.Add, "+" },
            { ScriptTokenType.Subtract, "-" },
            { ScriptTokenType.Multiply, "*" },
            { ScriptTokenType.Divide, "/" },
            { ScriptTokenType.Remainder, "%" },
            { ScriptTokenType.BitwiseShiftLeft, "<<" },
            { ScriptTokenType.BitwiseShiftRight, ">>" },
            { ScriptTokenType.BitwiseAnd, "&" },
            { ScriptTokenType.BitwiseOr, "|" },
            { ScriptTokenType.BitwiseXor, "^" },
            { ScriptTokenType.LogicalAnd, "&&" },
            { ScriptTokenType.LogicalOr, "||" },
            { ScriptTokenType.EqualTo, "==" },
            { ScriptTokenType.NotEqualTo, "!=" },
            { ScriptTokenType.GreaterThan, ">" },
            { ScriptTokenType.GreaterThanOrEqual, ">=" },
            { ScriptTokenType.LessThan, "<" },
            { ScriptTokenType.LessThanOrEqual, "<=" },
            { ScriptTokenType.Assign, "=" },
            { ScriptTokenType.AddAssign, "+=" },
            { ScriptTokenType.SubtractAssign, "-=" },
            { ScriptTokenType.MultiplyAssign, "*=" },
            { ScriptTokenType.DivideAssign, "/=" },
            { ScriptTokenType.RemainderAssign, "%=" },
            { ScriptTokenType.BitwiseShiftLeftAssign, "<<=" },
            { ScriptTokenType.BitwiseShiftRightAssign, ">>=" },
            { ScriptTokenType.BitwiseAndAssign, "&=" },
            { ScriptTokenType.BitwiseOrAssign, "|=" },
            { ScriptTokenType.BitwiseXorAssign, "^=" },
        };

        private static readonly Dictionary<ScriptTokenType, string> PostfixOperatorMap = new Dictionary<ScriptTokenType, string>
        {
            { ScriptTokenType.Increment, "++" },
            { ScriptTokenType.Decrement, "--" },
        };

        private static readonly Dictionary<ScriptTokenType, string> PrefixOperatorMap = new Dictionary<ScriptTokenType, string>
        {
            { ScriptTokenType.Add, "+" },
            { ScriptTokenType.Subtract, "-" },
            { ScriptTokenType.BitwiseNot, "~" },
            { ScriptTokenType.LogicalNot, "!" },
            { ScriptTokenType.Increment, "++" },
            { ScriptTokenType.Decrement, "--" },
        };

        #endregion

        #region Event Handlers

        private static readonly Dictionary<string, List<MethodDeclaration.Parameter>> EventHandlers = new Dictionary<string, List<MethodDeclaration.Parameter>>
        {
            {
                "messageHandler", new List<MethodDeclaration.Parameter>
                {
                    new MethodDeclaration.Parameter(new ScriptType("obj_id"), "self"),
                    new MethodDeclaration.Parameter(new ScriptType("dictionary"), "params"),
                }
            },
            {
                "commandHandler", new List<MethodDeclaration.Parameter>
                {
                    new MethodDeclaration.Parameter(new ScriptType("obj_id"), "self"),
                    new MethodDeclaration.Parameter(new ScriptType("obj_id"), "target"),
                    new MethodDeclaration.Parameter(new ScriptType("String"), "params"),
                    new MethodDeclaration.Parameter(new ScriptType("float"), "defaultTime"),
                }
            },
            {
                "trigger", new List<MethodDeclaration.Parameter>
                {
                    new MethodDeclaration.Parameter(new ScriptType("obj_id"), "self"),
                }
            },
        };

        #endregion
    }
}
