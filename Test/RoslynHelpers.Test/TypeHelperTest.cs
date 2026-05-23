namespace RoslynHelpers.Test;

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using VerifyCSType = CSharpLatest.Test.CSharpAnalyzerVerifier<TestAnalyzers.TestAnalyzer0>;
using VerifyCSExpression = CSharpLatest.Test.CSharpAnalyzerVerifier<TestAnalyzers.TestAnalyzer1>;
using NUnit.Framework;

[TestFixture]
internal partial class TypeHelperTest
{
    [Test]
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
").ConfigureAwait(false);
    }

    [Test]
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
", DiagnosticResult.CompilerError("CS8754").WithSpan(8, 17, 8, 22).WithArguments("new()")).ConfigureAwait(false);
    }

    [Test]
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
").ConfigureAwait(false);
    }

    [Test]
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
", DiagnosticResult.CompilerError("CS0103").WithSpan(10, 13, 10, 14).WithArguments("x")).ConfigureAwait(false);
    }

    [Test]
    public async Task ExpressionNotApplicable_NoDiagnostic()
    {
        await VerifyCSExpression.VerifyAnalyzerAsync(@"
#nullable enable

using System;

class Program
{
    static void Main(string[] args)
    {
        string? s = args.Length > 0 ? null : ""test"";

        if (null == s)
            Console.WriteLine(string.Empty);
    }
}
").ConfigureAwait(false);
    }
}
