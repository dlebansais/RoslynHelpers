namespace RoslynHelpers.Test;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCSType = CSharpLatest.Test.CSharpAnalyzerVerifier<TestAnalyzers.TestAnalyzer5>;

public partial class UsingDirectiveHelperTest
{
    [TestMethod]
    public async Task Sorted1_NoDiagnostic()
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
    public async Task Sorted2_NoDiagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using System;
using Contracts;

class Program
{
}
");
    }

    [TestMethod]
    public async Task Sorted3_NoDiagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using System.IO;
using Contracts;

class Program
{
}
");
    }

    [TestMethod]
    public async Task Sorted4_NoDiagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using global::System;
using Contracts;

class Program
{
}
");
    }

    [TestMethod]
    public async Task Sorted5_NoDiagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using global::System.IO;
using Contracts;

class Program
{
}
");
    }

    [TestMethod]
    public async Task Unsorted1_Diagnostic()
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
    public async Task Unsorted2_Diagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using Contracts;
using System.Threading.Tasks;
using FileStream = System.IO.FileStream;
using System.IO;
using System;
using System.Threading;

[|class Program
{
}|]
");
    }

    [TestMethod]
    public async Task Unsorted3_Diagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using Contracts;
using System;
using FileStream = System.IO.FileStream;

[|class Program
{
}|]
");
    }

    [TestMethod]
    public async Task Unsorted4_Diagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using Contracts;
using System;

[|class Program
{
}|]
");
    }

    [TestMethod]
    public async Task Unsorted5_Diagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using Contracts;
using System.IO;

[|class Program
{
}|]
");
    }

    [TestMethod]
    public async Task Unsorted6_Diagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using Contracts;
using global::System;

[|class Program
{
}|]
");
    }

    [TestMethod]
    public async Task Unsorted7_Diagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using Contracts;
using global::System.IO;

[|class Program
{
}|]
");
    }

    [TestMethod]
    public async Task Unsorted16_Diagnostic()
    {
        await VerifyCSType.VerifyAnalyzerAsync(@"
using System.IO;
using System;
using System.Threading.Tasks;
using System.Threading;

[|class Program
{
}|]
");
    }
}
