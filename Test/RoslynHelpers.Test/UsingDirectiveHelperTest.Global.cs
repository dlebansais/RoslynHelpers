namespace RoslynHelpers.Test;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCSType = CSharpLatest.Test.CSharpAnalyzerVerifier<TestAnalyzers.TestAnalyzer4>;

public partial class UsingDirectiveHelperTest
{
    [TestMethod]
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
");
    }

    [TestMethod]
    public async Task GlobalSystem_NoDiagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using Contracts;
using FileStream = System.IO.FileStream;
using global::System;

class Program
{
}
");
    }

    [TestMethod]
    public async Task GlobalSystemIo_NoDiagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using Contracts;
using FileStream = System.IO.FileStream;
using global::System.IO;

class Program
{
}
");
    }
}
