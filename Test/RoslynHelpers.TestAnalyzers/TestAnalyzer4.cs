#pragma warning disable RS1033 // Define diagnostic description correctly
#pragma warning disable RS1036 // Specify analyzer banned API enforcement setting
#pragma warning disable RS2008 // Enable analyzer release tracking

namespace RoslynHelpers.TestAnalyzers;

using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class TestAnalyzer4 : DiagnosticAnalyzer
{
    public const string DiagnosticId = "TEST0004";

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

        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration);
    }

    private void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        MemberDeclarationSyntax MemberDeclaration = (MemberDeclarationSyntax)context.Node;

        Debug.Assert(MemberDeclaration.Parent is CompilationUnitSyntax);
        CompilationUnitSyntax CompilationUnit = (CompilationUnitSyntax)MemberDeclaration.Parent!;
        /*
        if (MemberDeclaration.Parent is not CompilationUnitSyntax CompilationUnit)
            return;
        */

        string Usings = string.Empty;

        foreach (UsingDirectiveSyntax UsingDirective in CompilationUnit.Usings)
            Usings += $"{UsingDirective}\n";


        bool HasGlobalSystem = UsingDirectiveHelper.HasGlobalSystem(Usings) || UsingDirectiveHelper.HasGlobalSystem(string.Empty);

        if (HasGlobalSystem)
            return;

        context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation(), " *Diagnostic*"));
    }
}
