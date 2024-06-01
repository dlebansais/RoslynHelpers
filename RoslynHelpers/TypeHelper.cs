namespace RoslynHelpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

/// <summary>
/// Helpers to perform operations on types.
/// </summary>
public static class TypeHelper
{
    /// <summary>
    /// Gets the type of <paramref name="expression"/> as a valid type, or <see langword="null"/>.
    /// </summary>
    /// <param name="expression">The expression syntax to check.</param>
    /// <param name="context">The analysis context.</param>
    /// <returns>The type of <paramref name="expression"/>if a valid type; otherwise, <see langword="null"/>.</returns>
    public static ITypeSymbol? GetExpressionValidType(this ExpressionSyntax expression, SyntaxNodeAnalysisContext context)
    {
        TypeInfo ExpressionTypeInfo = context.SemanticModel.GetTypeInfo(expression, context.CancellationToken);
        ITypeSymbol? ExpressionType = ExpressionTypeInfo.Type;

        if (!IsTypeSymbolAndNotError(ExpressionTypeInfo, out ITypeSymbol ValidResult))
            return null;

        return ValidResult;
    }

    /// <summary>
    /// Gets the type of <paramref name="type"/> as a valid type, or <see langword="null"/>.
    /// </summary>
    /// <param name="type">The type syntax to check.</param>
    /// <param name="context">The analysis context.</param>
    /// <returns>The type of <paramref name="type"/>if a valid type; otherwise, <see langword="null"/>.</returns>
    public static ITypeSymbol? GetTypeValidType(this TypeSyntax type, SyntaxNodeAnalysisContext context)
    {
        TypeInfo TypeTypeInfo = context.SemanticModel.GetTypeInfo(type, context.CancellationToken);
        ITypeSymbol? TypeType = TypeTypeInfo.Type;

        if (!IsTypeSymbolAndNotError(TypeTypeInfo, out ITypeSymbol ValidResult))
            return null;

        return ValidResult;
    }

    private static bool IsTypeSymbolAndNotError<T>(TypeInfo typeInfo, out T result)
        where T : class
    {
        ITypeSymbol? Type = typeInfo.Type;

        if (Type is T ValidResult && Type is not IErrorTypeSymbol)
        {
            result = ValidResult;
            return true;
        }

        result = null!;
        return false;
    }
}
