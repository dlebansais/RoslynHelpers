namespace RoslynHelpers.Test;

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCSType = CSharpLatest.Test.CSharpAnalyzerVerifier<TestAnalyzers.TestAnalyzer0>;
using VerifyCSExpression = CSharpLatest.Test.CSharpAnalyzerVerifier<TestAnalyzers.TestAnalyzer1>;

[TestClass]
public partial class TypeHelperTest
{
    [TestMethod]
    public async Task IntType_Diagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using System;

class Program
{
    static void Main()
    {
        [|int i = 0;|]
        Console.WriteLine(i);
    }
}
");
    }

    [TestMethod]
    public async Task InvalidDeclaration_NoDiagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using System;

class Program
{
    static void Main()
    {
        var i = new();
        Console.WriteLine(i++);
    }
}
", DiagnosticResult.CompilerError("CS8754").WithSpan(8, 17, 8, 22).WithArguments("new()"));
    }

    [TestMethod]
    public async Task EqualsEqualsExpression_Diagnostic()
    {
        await VerifyCSExpression.VerifyAnalyzerAsync(@"
#nullable enable

using System;

class Program
{
    static void Main(string[] args)
    {
        string? s = args.Length > 0 ? null : ""test"";

        if ([|s == null|])
            Console.WriteLine(string.Empty);
    }
}
");
    }

    [TestMethod]
    public async Task UnknownEqualsEqualsExpression_Diagnostic()
    {
        await VerifyCSExpression.VerifyAnalyzerAsync(@"
#nullable enable

using System;

class Program
{
    static void Main(string[] args)
    {
        if (x == null)
            Console.WriteLine(string.Empty);
    }
}
", DiagnosticResult.CompilerError("CS0103").WithSpan(10, 13, 10, 14).WithArguments("x"));
    }
}
