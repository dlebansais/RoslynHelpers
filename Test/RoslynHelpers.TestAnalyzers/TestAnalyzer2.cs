#pragma warning disable RS1033 // Define diagnostic description correctly
#pragma warning disable RS1036 // Specify analyzer banned API enforcement setting
#pragma warning disable RS2008 // Enable analyzer release tracking

namespace RoslynHelpers.TestAnalyzers;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class TestAnalyzer2 : DiagnosticAnalyzer
{
    public const string DiagnosticId = "TEST0002";

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
    private const string Category = "Usage";

    public static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.NotEqualsExpression);
    }

    private void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        BinaryExpressionSyntax BinaryExpression = (BinaryExpressionSyntax)context.Node;

        ITypeSymbol? ExpressionType = BinaryExpression.Left.GetExpressionValidType(context);

        if (ExpressionType is null)
            return;

        if (ExpressionType.IsOverloadingExclamationEqualsOperator(context))
            return;

        if (!ExpressionType.IsReferenceType && ExpressionType.NullableAnnotation != NullableAnnotation.Annotated)
            return;

        context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation(), "*Diagnostic*"));
    }
}
