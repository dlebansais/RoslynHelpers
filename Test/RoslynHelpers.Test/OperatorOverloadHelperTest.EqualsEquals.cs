namespace RoslynHelpers.Test;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = CSharpLatest.Test.CSharpAnalyzerVerifier<TestAnalyzers.TestAnalyzer1>;

public partial class OperatorOverloadHelperTest
{
    [TestMethod]
    public async Task EqualsEqualsOperator_Diagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(@"
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
    public async Task EqualsEqualsOperator_NoDiagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(@"
#nullable enable

using System;

class Program
{
    static void Main(string[] args)
    {
        Foo? x = args.Length > 0 ? null : new();

        if (x == null)
            Console.WriteLine(string.Empty);
    }
}

class Foo
{
    public static bool operator ==(Foo? foo1, Foo? foo2)
    {
        if (object.Equals(foo2, null)) throw new Exception(""oops"");
            return object.Equals(foo1, foo2);
    }
    public static bool operator !=(Foo? foo1, Foo? foo2)
    {
        if (object.Equals(foo2, null)) throw new Exception(""oops"");
            return !object.Equals(foo1, foo2);
    }
}
");
    }
}
