namespace RoslynHelpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

/// <summary>
/// Helpers to check whether the == operator is overloaded.
/// </summary>
public static class OperatorOverloadHelper
{
    /// <summary>
    /// Checks whether the type of <paramref name="expression"/> is overloading the == operator.
    /// </summary>
    /// <param name="expression">The expression to check.</param>
    /// <param name="context">The analysis context.</param>
    /// <param name="referenceTypeOnly">Set to <see langword="true"/> to check only if the type of <paramref name="expression"/> is a reference type.</param>
    /// <returns><see langword="true"/> if the type of <paramref name="expression"/> is overloading the == operator; otherwise, <see langword="false"/>.</returns>
    public static bool IsEqualsOperatorOverloadedInType(this ExpressionSyntax expression, SyntaxNodeAnalysisContext context, bool referenceTypeOnly = false)
    {
        ITypeSymbol? ExpressionType = context.SemanticModel.GetTypeInfo(expression, context.CancellationToken).Type;

        if (ExpressionType is not null && (!referenceTypeOnly || ExpressionType.IsReferenceType))
            return ExpressionType.IsOverloadingEqualsOperator(context);

        return false;
    }

    /// <summary>
    /// Checks whether <paramref name="expressionType"/> is overloading the == operator.
    /// </summary>
    /// <param name="expressionType">The type to check.</param>
    /// <param name="context">The analysis context.</param>
    /// <returns><see langword="true"/> if <paramref name="expressionType"/> is overloading the == operator; otherwise, <see langword="false"/>.</returns>
    public static bool IsOverloadingEqualsOperator(this ITypeSymbol expressionType, SyntaxNodeAnalysisContext context)
    {
        var Symbols = expressionType.GetMembers();

        foreach (var Symbol in Symbols)
            if (!Symbol.IsImplicitlyDeclared && Symbol.IsOperatorOverload(context, SyntaxKind.EqualsEqualsToken))
                return true;

        return false;
    }

    /// <summary>
    /// Checks whether the type of <paramref name="expression"/> is overloading the != operator.
    /// </summary>
    /// <param name="expression">The expression to check.</param>
    /// <param name="context">The analysis context.</param>
    /// <param name="referenceTypeOnly">Set to <see langword="true"/> to check only if the type of <paramref name="expression"/> is a reference type.</param>
    /// <returns><see langword="true"/> if the type of <paramref name="expression"/> is overloading the != operator; otherwise, <see langword="false"/>.</returns>
    public static bool IsExclamationEqualsOperatorOverloadedInType(this ExpressionSyntax expression, SyntaxNodeAnalysisContext context, bool referenceTypeOnly = false)
    {
        ITypeSymbol? ExpressionType = context.SemanticModel.GetTypeInfo(expression, context.CancellationToken).Type;

        if (ExpressionType is not null && (!referenceTypeOnly || ExpressionType.IsReferenceType))
            return ExpressionType.IsOverloadingExclamationEqualsOperator(context);

        return false;
    }

    /// <summary>
    /// Checks whether <paramref name="expressionType"/> is overloading the != operator.
    /// </summary>
    /// <param name="expressionType">The type to check.</param>
    /// <param name="context">The analysis context.</param>
    /// <returns><see langword="true"/> if <paramref name="expressionType"/> is overloading the != operator; otherwise, <see langword="false"/>.</returns>
    public static bool IsOverloadingExclamationEqualsOperator(this ITypeSymbol expressionType, SyntaxNodeAnalysisContext context)
    {
        foreach (var Symbol in expressionType.GetMembers())
            if (!Symbol.IsImplicitlyDeclared && Symbol.IsOperatorOverload(context, SyntaxKind.ExclamationEqualsToken))
                return true;

        return false;
    }

    private static bool IsOperatorOverload(this ISymbol symbol, SyntaxNodeAnalysisContext context, SyntaxKind operatorKind)
    {
        foreach (var SyntaxReference in symbol.DeclaringSyntaxReferences)
        {
            SyntaxNode Declaration = SyntaxReference.GetSyntax(context.CancellationToken);

            if (Declaration is OperatorDeclarationSyntax OperatorDeclaration && OperatorDeclaration.OperatorToken.IsKind(operatorKind))
                return true;
        }

        return false;
    }
}
