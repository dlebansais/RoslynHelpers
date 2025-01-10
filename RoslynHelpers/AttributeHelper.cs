namespace RoslynHelpers;

using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using Contracts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

/// <summary>
/// Helper to perform operations on attributes.
/// </summary>
public static partial class AttributeHelper
{
    /// <summary>
    /// Gets all supported attributes of a method or property.
    /// If <paramref name="context"/> is not <see langword="null"/>, it will check if the attribute is in the same namespace and assembly; otherwise, only it will only check for matching names.
    /// </summary>
    /// <param name="context">The analysis context. Can be <see langword="null"/> if no context is available.</param>
    /// <param name="memberDeclaration">The method or property.</param>
    /// <param name="supportedAttributeTypes">The list of supported attributes.</param>
    [RequireNotNull(nameof(memberDeclaration), nameof(supportedAttributeTypes))]
    private static Collection<AttributeSyntax> GetMemberSupportedAttributesVerified(SyntaxNodeAnalysisContext? context, MemberDeclarationSyntax memberDeclaration, Collection<Type> supportedAttributeTypes)
    {
        Collection<AttributeSyntax> Result = [];

        for (int IndexList = 0; IndexList < memberDeclaration.AttributeLists.Count; IndexList++)
        {
            AttributeListSyntax AttributeList = memberDeclaration.AttributeLists[IndexList];

            for (int Index = 0; Index < AttributeList.Attributes.Count; Index++)
            {
                AttributeSyntax Attribute = AttributeList.Attributes[Index];

                foreach (Type supportedAttributeType in supportedAttributeTypes)
                {
                    AddttributeIfMatch(context, Result, Attribute, supportedAttributeType);
                }
            }
        }

        return Result;
    }

    private static void AddttributeIfMatch(SyntaxNodeAnalysisContext? context, Collection<AttributeSyntax> result, AttributeSyntax attribute, Type supportedAttributeType)
    {
        string SupportedTypeName = supportedAttributeType.Name;
        bool IsSameNamespaceAssembly;

        if (context is SyntaxNodeAnalysisContext AvailableContext)
        {
            IsSameNamespaceAssembly = false;

            SymbolInfo SymbolInfo = AvailableContext.SemanticModel.GetSymbolInfo(attribute);
            if (SymbolInfo.Symbol is ISymbol AttributeSymbol)
            {
                string? SupportedTypeFullName = supportedAttributeType.FullName;
                if (SupportedTypeFullName is not null)
                {
                    ImmutableArray<INamedTypeSymbol> MatchingTypeSymbols = AvailableContext.Compilation.GetTypesByMetadataName(SupportedTypeFullName);
                    if (MatchingTypeSymbols.FirstOrDefault(symbol => symbol.ContainingAssembly.Identity.ToString() == supportedAttributeType.Assembly.FullName) is ITypeSymbol SupportedTypeSymbol)
                    {
                        INamespaceSymbol ContainingNamespace = Contract.AssertNotNull(SupportedTypeSymbol.ContainingNamespace);

                        if (SymbolEqualityComparer.Default.Equals(ContainingNamespace, AttributeSymbol.ContainingNamespace))
                            IsSameNamespaceAssembly = true;
                    }
                }
            }
        }
        else
        {
            IsSameNamespaceAssembly = true;
        }

        if (IsSameNamespaceAssembly)
        {
            string AttributeName = ToAttributeName(attribute);

            if (AttributeName == SupportedTypeName)
                result.Add(attribute);
        }
    }

    /// <summary>
    /// Returns the full name of an attribute.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    private static string ToAttributeName(AttributeSyntax attribute) => $"{attribute.Name.GetText()}{nameof(Attribute)}";
}
