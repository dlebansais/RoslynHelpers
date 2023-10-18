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
    /// <param name="expression">The expression.</param>
    /// <param name="context">The analysis context.</param>
    /// <returns>The type of <paramref name="expression"/>if a valid type; otherwise, <see langword="null"/>.</returns>
    public static ITypeSymbol? GetExpressionValidType(this ExpressionSyntax expression, SyntaxNodeAnalysisContext context)
    {
        TypeInfo ExpressionTypeInfo = context.SemanticModel.GetTypeInfo(expression, context.CancellationToken);
        ITypeSymbol? ExpressionType = ExpressionTypeInfo.Type;

        return ExpressionType is ITypeSymbol ValidResult && ExpressionType is not IErrorTypeSymbol ? ValidResult : null;
    }
}
