namespace RoslynHelpers.Test;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCSType = CSharpLatest.Test.CSharpAnalyzerVerifier<TestAnalyzers.TestAnalyzer4>;

[TestClass]
public partial class UsingDirectiveHelperTest
{
    [TestMethod]
    public async Task SortedNoGlobal_NoDiagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using System;
using System.IO;
using Contracts;
using FileStream = System.IO.FileStream;

class Program
{
}
");
    }

    [TestMethod]
    public async Task UnsortedNoGlobal_Diagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using Contracts;
using System.Threading.Tasks;
using System.IO;
using FileStream = System.IO.FileStream;
using System;
using System.Threading;

[|class Program
{
}|]
");
    }

    [TestMethod]
    public async Task UnsortedGlobalSystem_NoDiagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using Contracts;
using FileStream = System.IO.FileStream;
using global::System.IO;
using global::System;

class Program
{
}
");
    }

    [TestMethod]
    public async Task GlobalSystemOnly_NoDiagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
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
}
