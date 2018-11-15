using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Monosoft.ServerSideFunctions.Service.MessageHandlers
{
    static class Compiler
    {
        private static string[] defaultNamespaces;

        private static string runtimePath;

        private static MetadataReference[] defaultReferences;

        private static CSharpCompilationOptions options;


        static Compiler()
        {
            defaultNamespaces = new[]
           {
                "System",
                "System.IO",
                "System.Net",
                "System.Linq",
                "System.Text",
                "System.Text.RegularExpressions",
                "System.Collections.Generic"
           };

            runtimePath = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6\{0}.dll";
            defaultReferences = new[]
            {
                MetadataReference.CreateFromFile(string.Format(runtimePath, "mscorlib")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System.Core"))
            };

            options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOverflowChecks(true).WithOptimizationLevel(OptimizationLevel.Release)
                    .WithUsings(defaultNamespaces);
        }

        public static string CreateDll(string functionName, string functionString)
        {
            string fileName = functionName + ".dll";
            var path = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(functionString);

            var compilation = CSharpCompilation.Create(fileName, new SyntaxTree[] { syntaxTree }, defaultReferences, options);

            string res = "Failed";
            try
            {
                var result = compilation.Emit(path);
                if (result.Success)
                {
                    res = "Succeed";
                }
                else
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return res;
        }

        public static object RunDll(string functionName, params object[] parameters)
        {
            string fileName = functionName + ".dll";
            var path = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            object result = new object();
            try
            {
                var dll = Assembly.LoadFile(path);

                foreach (Type type in dll.GetExportedTypes())
                {
                    dynamic c = Activator.CreateInstance(type);
                    //int temp = c.Sum(2, 3);
                    result = type.InvokeMember(functionName, BindingFlags.InvokeMethod, null, c, parameters);

                    Console.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return result;
        }

        public static object[] ConvertToObjectArray(string inputString)
        {
            if (string.IsNullOrEmpty(inputString.Replace(" ", "")))
            {
                return new object[] { };
            }
            string[] tempArray = inputString.Split(",");
            string variables = string.Empty;
            string returnArray = string.Empty;
            for (int i = 0; i < tempArray.Length; i++)
            {
                string[] item = tempArray[i].Split(":");
                variables += item[0] + " value" + i.ToString() + " = " + item[1] + ";";
                returnArray += i == 0 ? item[1] : ", " + item[1];
            }

            string testString2 = "namespace ConvertParams { public class MyClass { public object[] GetArray() { " + variables + " return new object[] { " + returnArray + " }; } } }";

            string operationResult = CreateDll("GetArray", testString2);
            Console.WriteLine(operationResult);

            return RunGetArrayDll("GetArray", new object[] { });
        }

        private static object[] RunGetArrayDll(string functionName, object[] parameters)
        {
            string fileName = functionName + ".dll";
            var path = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            object[] res = null;
            try
            {
                var dll = Assembly.LoadFile(path);

                foreach (Type type in dll.GetExportedTypes())
                {
                    dynamic c = Activator.CreateInstance(type);
                    //int temp = c.Sum(2, 3);
                    res = type.InvokeMember(functionName, BindingFlags.InvokeMethod, null, c, parameters);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return res;
        }
    }
}
