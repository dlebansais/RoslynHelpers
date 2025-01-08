namespace RoslynHelpers;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Contracts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

/// <summary>
/// Helpers to check whether the == operator is overloaded.
/// </summary>
public static partial class OperatorOverloadHelper
{
    /// <summary>
    /// Checks whether <paramref name="typeSymbol"/> is overloading the == operator.
    /// </summary>
    /// <param name="typeSymbol">The type to check.</param>
    /// <param name="context">The analysis context.</param>
    /// <returns><see langword="true"/> if <paramref name="typeSymbol"/> is overloading the == operator; otherwise, <see langword="false"/>.</returns>
    [RequireNotNull(nameof(typeSymbol))]
    private static bool IsOverloadingEqualsEqualsOperatorVerified(this ITypeSymbol typeSymbol, SyntaxNodeAnalysisContext context)
        => IsOverloadingEqualsOperator(typeSymbol, context, SyntaxKind.EqualsEqualsToken);

    /// <summary>
    /// Checks whether <paramref name="typeSymbol"/> is overloading the != operator.
    /// </summary>
    /// <param name="typeSymbol">The type to check.</param>
    /// <param name="context">The analysis context.</param>
    /// <returns><see langword="true"/> if <paramref name="typeSymbol"/> is overloading the != operator; otherwise, <see langword="false"/>.</returns>
    [RequireNotNull(nameof(typeSymbol))]
    private static bool IsOverloadingExclamationEqualsOperatorVerified(this ITypeSymbol typeSymbol, SyntaxNodeAnalysisContext context)
        => IsOverloadingEqualsOperator(typeSymbol, context, SyntaxKind.ExclamationEqualsToken);

    /// <summary>
    /// Checks whether <paramref name="typeSymbol"/> is overloading the <paramref name="operatorKind"/> operator.
    /// </summary>
    /// <param name="typeSymbol">The type to check.</param>
    /// <param name="context">The analysis context.</param>
    /// <param name="operatorKind">The operator.</param>
    /// <returns><see langword="true"/> if <paramref name="typeSymbol"/> is overloading the <paramref name="operatorKind"/> operator; otherwise, <see langword="false"/>.</returns>
    private static bool IsOverloadingEqualsOperator(this ITypeSymbol typeSymbol, SyntaxNodeAnalysisContext context, SyntaxKind operatorKind)
    {
        // If the type is a nullable struct.
        if (!typeSymbol.IsReferenceType && typeSymbol.NullableAnnotation == NullableAnnotation.Annotated)
        {
            INamedTypeSymbol NamedTypeSymbol = (INamedTypeSymbol)typeSymbol;
            INamedTypeSymbol OriginalDefinition = Contract.AssertNotNull(NamedTypeSymbol.OriginalDefinition);

            Contract.Assert(OriginalDefinition.SpecialType == SpecialType.System_Nullable_T);

            // Get the original struct type. This is the type to inspect for overloaded operators.
            typeSymbol = NamedTypeSymbol.TypeArguments[0];
        }

        ImmutableArray<ISymbol> Symbols = typeSymbol.GetMembers();
        IEnumerable<ISymbol> Overloads = Symbols.Where(symbol => symbol.IsOperatorOverload(context));
        bool HasOverloadOfKind = Overloads.Any(symbol => symbol.IsOperatorOverloadOfKind(context, operatorKind));

        return HasOverloadOfKind;
    }

    private static bool IsOperatorOverload(this ISymbol symbol, SyntaxNodeAnalysisContext context)
    {
        if (symbol.DeclaringSyntaxReferences.Length == 0)
            return false;

        SyntaxReference SyntaxReference = symbol.DeclaringSyntaxReferences[0];
        bool IsOperator = SyntaxReference.GetSyntax(context.CancellationToken) is OperatorDeclarationSyntax;

        return IsOperator;
    }

    private static bool IsOperatorOverloadOfKind(this ISymbol symbol, SyntaxNodeAnalysisContext context, SyntaxKind operatorKind)
    {
        Contract.Assert(symbol.DeclaringSyntaxReferences.Length > 0);

        SyntaxReference SyntaxReference = symbol.DeclaringSyntaxReferences[0];
        SyntaxNode Node = SyntaxReference.GetSyntax(context.CancellationToken);
        Contract.Assert(Node is OperatorDeclarationSyntax);

        OperatorDeclarationSyntax OperatorDeclaration = (OperatorDeclarationSyntax)Node;

        return OperatorDeclaration.OperatorToken.IsKind(operatorKind);
    }
}
