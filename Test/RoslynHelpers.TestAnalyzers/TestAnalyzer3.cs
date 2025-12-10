#pragma warning disable RS1033 // Define diagnostic description correctly
#pragma warning disable RS1036 // Specify analyzer banned API enforcement setting
#pragma warning disable RS2008 // Enable analyzer release tracking

namespace RoslynHelpers.TestAnalyzers;

using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Contracts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class TestAnalyzer3 : DiagnosticAnalyzer
{
    public const string DiagnosticId = "TEST0003";

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

        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);
    }

    private void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        MethodDeclarationSyntax MethodDeclaration = (MethodDeclarationSyntax)context.Node;

        Type CollectionType = typeof(Collection<>);
        Type ItemType = CollectionType.GetGenericArguments()[0];
        Collection<Type> SupportedAttributes = [typeof(AccessAttribute), CollectionType, ItemType];

        Collection<AttributeSyntax> SupportedAttributesNoContext = AttributeHelper.GetMemberSupportedAttributes(null, MethodDeclaration, SupportedAttributes);
        Collection<AttributeSyntax> SupportedAttributesContext = AttributeHelper.GetMemberSupportedAttributes(context, MethodDeclaration, SupportedAttributes);

        if (SupportedAttributesNoContext.Count == 1 && SupportedAttributesContext.Count == 1)
            return;

        context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation(), "*Diagnostic*"));
    }
}
