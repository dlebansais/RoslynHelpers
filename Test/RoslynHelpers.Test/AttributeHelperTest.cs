namespace RoslynHelpers.Test;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCSType = CSharpLatest.Test.CSharpAnalyzerVerifier<TestAnalyzers.TestAnalyzer3>;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

[TestClass]
public partial class AttributeHelperTest
{
    [TestMethod]
    public async Task NoAttribute_Diagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using System;

class Program
{
    [|static void Main()
    {
    }|]
}
");
    }

    [TestMethod]
    public async Task OneAttribute_NoDiagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using System;
using Contracts;

class Program
{
    [Access(""static"")]
    static void Main()
    {
    }
}
");
    }

    [TestMethod]
    public async Task UnknownAttribute_Diagnostic()
    {
        DiagnosticDescriptor DescriptorCS0246_1 = new(
            "CS0246",
            "title",
            "The type or namespace name 'Foo' could not be found (are you missing a using directive or an assembly reference?)",
            "description",
            DiagnosticSeverity.Error,
            true
            );

        DiagnosticResult Expected1 = new(DescriptorCS0246_1);
        Expected1 = Expected1.WithLocation("/0/Test0.cs", 7, 6);

        DiagnosticDescriptor DescriptorCS0246_2 = new(
            "CS0246",
            "title",
            "The type or namespace name 'FooAttribute' could not be found (are you missing a using directive or an assembly reference?)",
            "description",
            DiagnosticSeverity.Error,
            true
            );

        DiagnosticResult Expected2 = new(DescriptorCS0246_2);
        Expected2 = Expected2.WithLocation("/0/Test0.cs", 7, 6);

        await VerifyCSType.VerifyAnalyzerAsync(@"
using System;
using Contracts;

class Program
{
    [|[Foo]
    static void Main()
    {
    }|]
}
", Expected1, Expected2);
    }

    [TestMethod]
    public async Task UnsupportedAttribute_Diagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using System;

class Program
{
    [|[Obsolete]
    static void Main()
    {
    }|]
}
");
    }

    [TestMethod]
    public async Task WrongNamespaceAttribute_Diagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using System;

class AccessAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}

class Program
{
    [|[Access(""static"")]
    static void Main()
    {
    }|]
}
");
    }
}
