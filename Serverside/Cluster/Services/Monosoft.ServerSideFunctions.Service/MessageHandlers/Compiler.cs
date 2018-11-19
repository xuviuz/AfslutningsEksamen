﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Monosoft.ServerSideFunctions.Service.MessageHandlers
{
    class Compiler
    {
        private static string[] defaultNamespaces;

        private static string runtimePath;

        private static MetadataReference[] defaultReferences;

        private static CSharpCompilationOptions options;


        public Compiler()
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

        public string CreateDll(string functionName, string functionString)
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

        public object RunDll(string functionName, params object[] parameters)
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

        public object[] ConvertToObjectArray(string inputString)
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
            string paramFunctionString = "namespace ConvertParams { public class MyClass { public object[] GetArray() { " + variables + " return new object[] { " + returnArray + " }; } } }";

            return GetParametersArray(paramFunctionString);
        }

        public object[] GetParametersArray(string inputString)
        {
            string functionName = "GetArray";
            string fileName = functionName + ".dll";

            SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(inputString);

            var compilation = CSharpCompilation.Create(fileName, new SyntaxTree[] { syntaxTree }, defaultReferences, options);

            string res = "Failed";
            object[] outputArray = new object[] { };

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (result.Success)
                {
                    res = "Succeed";
                    ms.Seek(0, SeekOrigin.Begin);

                    var dll = Assembly.Load(ms.ToArray());
                    foreach (Type type in dll.GetExportedTypes())
                    {
                        dynamic c = Activator.CreateInstance(type);
                        outputArray = type.InvokeMember(functionName, BindingFlags.InvokeMethod, null, c, new object[] { });
                    }
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
            Console.WriteLine(res);
            return outputArray;
        }
    }
}
