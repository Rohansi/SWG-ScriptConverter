using ScriptConverter.Ast.Declarations;
using ScriptConverter.Ast.Expressions;
using ScriptConverter.Ast.Statements;

namespace ScriptConverter.Ast
{
    interface IAstVisitor<out TDoc, out TDecl, out TStmt, out TExpr>
    {
        TDoc Visit(Document document);

        TDecl Visit(FieldDeclaration declaration);
        TDecl Visit(IncludeDeclaration declaration);
        TDecl Visit(InheritsDeclaration declaration);
        TDecl Visit(MethodDeclaration declaration);

        TStmt Visit(BlockStatement statement);
        TStmt Visit(BreakStatement statement);
        TStmt Visit(ContinueStatement statement);
        TStmt Visit(DoStatement statement);
        TStmt Visit(EmptyStatement statement);
        TStmt Visit(ForStatement statement);
        TStmt Visit(IfStatement statement);
        TStmt Visit(NakedStatement statement);
        TStmt Visit(ReturnStatement statement);
        TStmt Visit(SwitchStatement statement);
        TStmt Visit(ThrowStatement statement);
        TStmt Visit(TryStatement statement);
        TStmt Visit(VariableStatement statement);
        TStmt Visit(WhileStatement statement);

        TExpr Visit(ArrayInitializerExpression expression);
        TExpr Visit(BinaryOperatorExpression expression);
        TExpr Visit(BoolExpression expression);
        TExpr Visit(CallExpression expression);
        TExpr Visit(CastExpression expression);
        TExpr Visit(FieldExpression expression);
        TExpr Visit(IdentifierExpression expression);
        TExpr Visit(IndexerExpression expression);
        TExpr Visit(InstanceOfExpression expression);
        TExpr Visit(NewExpression expression);
        TExpr Visit(NullExpression expression);
        TExpr Visit(NumberExpression expression);
        TExpr Visit(PostfixOperatorExpression expression);
        TExpr Visit(PrefixOperatorExpression expression);
        TExpr Visit(StringExpression expression);
        TExpr Visit(TernaryExpression expression);
    }
}
