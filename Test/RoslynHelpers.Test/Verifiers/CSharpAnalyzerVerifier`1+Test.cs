namespace CSharpLatest.Test;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Contracts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

public static partial class CSharpAnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public class Test : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
    {
        public Test()
        {
            SolutionTransforms.Add((solution, projectId) =>
            {
                CompilationOptions? compilationOptions = solution.GetProject(projectId)!.CompilationOptions;
                compilationOptions = compilationOptions!.WithSpecificDiagnosticOptions(compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings));
                solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

                string RuntimePath = GetRuntimePath();

                List<MetadataReference> DefaultReferences =
                [
                    //MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(AccessAttribute).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(string.Format(CultureInfo.InvariantCulture, RuntimePath, "mscorlib")),
                    MetadataReference.CreateFromFile(string.Format(CultureInfo.InvariantCulture, RuntimePath, "System")),
                    MetadataReference.CreateFromFile(string.Format(CultureInfo.InvariantCulture, RuntimePath, "System.Core")),
                    MetadataReference.CreateFromFile(string.Format(CultureInfo.InvariantCulture, RuntimePath, "System.Xaml")),
                    MetadataReference.CreateFromFile(string.Format(CultureInfo.InvariantCulture, RuntimePath, "PresentationCore")),
                    MetadataReference.CreateFromFile(string.Format(CultureInfo.InvariantCulture, RuntimePath, "PresentationFramework")),
                    MetadataReference.CreateFromFile(string.Format(CultureInfo.InvariantCulture, RuntimePath, @"Facades\System.Runtime")),
                    MetadataReference.CreateFromFile(string.Format(CultureInfo.InvariantCulture, RuntimePath, @"Facades\System.Collections")),
                ];

                solution = solution.WithProjectMetadataReferences(projectId, DefaultReferences);

                return solution;
            });
        }

        private static string GetRuntimePath()
        {
            const string RuntimeDirectoryBase = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework";
            string RuntimeDirectory = string.Empty;

            foreach (string FolderPath in GetRuntimeDirectories(RuntimeDirectoryBase))
                if (IsValidRuntimeDirectory(FolderPath))
                    RuntimeDirectory = FolderPath;

            string RuntimePath = RuntimeDirectory + @"\{0}.dll";

            return RuntimePath;
        }

        private static List<string> GetRuntimeDirectories(string runtimeDirectoryBase)
        {
            string[] Directories = Directory.GetDirectories(runtimeDirectoryBase);
            List<string> DirectoryList = [.. Directories];
            DirectoryList.Sort(CompareIgnoreCase);

            return DirectoryList;
        }

        private static int CompareIgnoreCase(string s1, string s2) => string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase);

        private static bool IsValidRuntimeDirectory(string folderPath)
        {
            string FolderName = Path.GetFileName(folderPath);
            const string Prefix = "v";

            Contract.Assert(FolderName.StartsWith(Prefix, StringComparison.Ordinal));

            string[] Parts = FolderName[Prefix.Length..].Split('.');
            foreach (string Part in Parts)
                if (!int.TryParse(Part, out _))
                    return false;

            return true;
        }
    }
}
