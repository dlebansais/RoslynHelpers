namespace RoslynHelpers.Test;

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
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

    [TestMethod]
    public async Task ExclamationEqualsOperator_NoDiagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(@"
#nullable enable

using System;

class Program
{
    static void Main(string[] args)
    {
        Foo? x = args.Length > 0 ? null : new();

        if (x != null)
            Console.WriteLine(string.Empty);
    }
}

class Foo
{
    public static void Other()
    {
    }

    public static bool operator==(Foo? foo1, Foo? foo2)
    {
        if (object.Equals(foo2, null)) throw new Exception(""oops"");
            return object.Equals(foo1, foo2);
    }

    public static bool operator!=(Foo? foo1, Foo? foo2)
    {
        if (object.Equals(foo2, null)) throw new Exception(""oops"");
            return !object.Equals(foo1, foo2);
    }
}
");
    }

    [TestMethod]
    public async Task NullableStructExclamationEqualsOperator_NoDiagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(@"
#nullable enable

using System;

class Program
{
    static void Main(string[] args)
    {
        Foo? x = args.Length > 0 ? null : new();

        if (x != null)
            Console.WriteLine(string.Empty);
    }
}

struct Foo
{
    public static bool operator==(Foo foo1, Foo foo2)
    {
        if (object.Equals(foo2, null)) throw new Exception(""oops"");
            return object.Equals(foo1, foo2);
    }

    public static bool operator!=(Foo foo1, Foo foo2)
    {
        if (object.Equals(foo2, null)) throw new Exception(""oops"");
            return !object.Equals(foo1, foo2);
    }
}
");
    }

    [TestMethod]
    public async Task StructExclamationEqualsOperator_NoDiagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(@"
#nullable enable

using System;

class Program
{
    static void Main(string[] args)
    {
        Foo x;

        if (x != null)
            Console.WriteLine(string.Empty);
    }
}

struct Foo
{
}
", DiagnosticResult.CompilerError("CS0019").WithSpan(12, 13, 12, 22).WithArguments("!=", "Foo", "<null>"));
    }

    [TestMethod]
    public async Task VoidExclamationEqualsOperator_Diagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(@"
#nullable enable

using System;

class Program
{
    static void Main(string[] args)
    {
        if (Test() != null)
            Console.WriteLine(string.Empty);
    }

    static void Test()
    {
    }
}
", DiagnosticResult.CompilerError("CS0019").WithSpan(10, 13, 10, 27).WithArguments("!=", "void", "<null>"));
    }

    [TestMethod]
    public async Task UnknownExclamationEqualsOperator_Diagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(@"
#nullable enable

using System;

class Program
{
    static void Main(string[] args)
    {
        if (x != null)
            Console.WriteLine(string.Empty);
    }
}
", DiagnosticResult.CompilerError("CS0103").WithSpan(10, 13, 10, 14).WithArguments("x"));
    }

    [TestMethod]
    public async Task ArrayExclamationEqualsOperator_Diagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(@"
#nullable enable

using System;

class Program
{
    static void Main(string[] args)
    {
        string[]? s = args.Length > 0 ? null : new string[] { ""test"" };

        if ([|s != null|])
            Console.WriteLine(string.Empty);
    }
}
");
    }
}
