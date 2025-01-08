#pragma warning disable RS1033 // Define diagnostic description correctly
#pragma warning disable RS1036 // Specify analyzer banned API enforcement setting
#pragma warning disable RS2008 // Enable analyzer release tracking

namespace RoslynHelpers.TestAnalyzers;

using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynHelpers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class TestAnalyzer0 : DiagnosticAnalyzer
{
    public const string DiagnosticId = "TEST0000";

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
    public static readonly Resources Resources = new();
    public static readonly string TitleString = Resources.AnalyzerTitle;
    public static readonly string MessageFormatString = Resources.AnalyzerMessageFormat;
    public static readonly string DescriptionString = Resources.AnalyzerDescription;
    private const string Category = "Usage";

    public static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        CultureInfo Culture = Resources.Culture;
        Resources.Culture = Culture;

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.LocalDeclarationStatement);
    }

    private void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        LocalDeclarationStatementSyntax LocalDeclaration = (LocalDeclarationStatementSyntax)context.Node;
        TypeSyntax VariableTypeName = LocalDeclaration.Declaration.Type;

        ITypeSymbol? VariableType = VariableTypeName.GetTypeValidType(context);

        // Ensure that all variables in the local declaration have initializers that are assigned with constant values.
        foreach (VariableDeclaratorSyntax variable in LocalDeclaration.Declaration.Variables)
            if (!IsVariableAssignedToConstantValue(context, VariableType, variable))
                return;

        // Perform data flow analysis on the local declaration.
        DataFlowAnalysis? dataFlowAnalysis = context.SemanticModel.AnalyzeDataFlow(LocalDeclaration);

        foreach (VariableDeclaratorSyntax variable in LocalDeclaration.Declaration.Variables)
        {
            // Retrieve the local symbol for each variable in the local declaration and ensure that it is not written outside of the data flow analysis region.
            ISymbol? variableSymbol = context.SemanticModel.GetDeclaredSymbol(variable, context.CancellationToken);

            if (dataFlowAnalysis is not null && variableSymbol is not null && dataFlowAnalysis.WrittenOutside.Contains(variableSymbol))
                return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation(), LocalDeclaration.Declaration.Variables.First().Identifier.ValueText));
    }

    private bool IsVariableAssignedToConstantValue(SyntaxNodeAnalysisContext context, ITypeSymbol? variableType, VariableDeclaratorSyntax variable)
    {
        EqualsValueClauseSyntax initializer = variable.Initializer!;

        Optional<object?> constantValue = context.SemanticModel.GetConstantValue(initializer.Value, context.CancellationToken);
        if (!constantValue.HasValue)
            return false;

        // Ensure that the initializer value can be converted to the type of the local declaration without a user-defined conversion.
        Conversion conversion = context.SemanticModel.ClassifyConversion(initializer.Value, variableType!);
        if (!conversion.Exists || conversion.IsUserDefined)
            return false;

        return true;
    }
}
