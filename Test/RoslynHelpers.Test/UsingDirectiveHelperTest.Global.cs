namespace RoslynHelpers.Test;

using System.Threading.Tasks;
using NUnit.Framework;
using VerifyCSType = CSharpLatest.Test.CSharpAnalyzerVerifier<TestAnalyzers.TestAnalyzer4>;

internal partial class UsingDirectiveHelperTest
{
    [Test]
    public async Task NoGlobal_Diagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using System;
using System.IO;
using Contracts;
using FileStream = System.IO.FileStream;

[|class Program
{
}|]
").ConfigureAwait(false);
    }

    [Test]
    public async Task GlobalSystem_NoDiagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using Contracts;
using FileStream = System.IO.FileStream;
using global::System;

class Program
{
}
").ConfigureAwait(false);
    }

    [Test]
    public async Task GlobalSystemIo_NoDiagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using Contracts;
using FileStream = System.IO.FileStream;
using global::System.IO;

class Program
{
}
").ConfigureAwait(false);
    }
}
