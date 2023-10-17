namespace RoslynHelpers.Test;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = CSharpLatest.Test.CSharpAnalyzerVerifier<TestAnalyzers.TestAnalyzer2>;

public partial class OperatorOverloadHelperTest
{
    [TestMethod]
    public async Task ExclamationEqualsOperator_Diagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(@"
#nullable enable

using System;

class Program
{
    static void Main(string[] args)
    {
        string? s = args.Length > 0 ? null : ""test"";

        if ([|s != null|])
            Console.WriteLine(string.Empty);
    }
}
");
    }
}
