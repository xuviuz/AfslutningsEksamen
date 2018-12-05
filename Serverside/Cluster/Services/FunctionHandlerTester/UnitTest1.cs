using System;
using Xunit;
using Monosoft.ServerSideFunctions.Service.DTO;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace FunctionHandlerTester
{   
   
    public class UnitTest1
    {
        #region CreateFuncTest

        

        [Fact]
        public void CreateFuncTest()
        {
            var actualResult = CreateFunction("ForTesting", "namespace ye\r\n{\r\npublic class ForTesting\r\n{\r\npublic int ForTesting()\r\n{\r\n\r\nint t = 3;\r\n\r\nreturn Yepser2(t);\r\n\r\n}\r\nprivate int Yepser2(int x)\r\n{\r\nint i = x*2;\r\nreturn i;\r\n}\r\n}\r\n}\r\n",);
        }


        private static string[] defaultNamespaces;

        private static string runtimePath;

        private static MetadataReference[] defaultReferences;

        private static CSharpCompilationOptions options;

        public UnitTest1()
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
        public string CreateFunction(string functionName, string functionString, FunctionDefinitions jsonObj)
        {
            string path = Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Dller\" + functionName);
                Directory.CreateDirectory(path + functionName + "BackUps");
                string fileName = functionName + ".dll";
                var pathToEmit = Path.Combine(path, fileName);

                SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(functionString);

                var compilation = CSharpCompilation.Create(fileName, new SyntaxTree[] { syntaxTree }, defaultReferences, options);

                string res = "Error whole creating " + functionName;
                try
                {
                    var result = compilation.Emit(pathToEmit);
                    if (result.Success)
                    {
                        res = functionName + " was created";

                        File.WriteAllText(path + functionName + ".json", JsonConvert.SerializeObject(jsonObj));
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
            else
            {
                return "FUNCTION ALREADY EXSISTS!";
            }


        }
        #endregion
        #region RunFuncTest
        [Fact]
        public void RunFuncTest()
        {
            var actualResult = RunFunction("Yepser", null);
            int expectedResult = 6;

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void RunFuncTestFail()
        {
            var actualResult = RunFunction("wurg", null);
            string expectedResult = "FUNCTION DOES NOT EXSIST!";

            Assert.Equal(expectedResult, actualResult);
            
        }

        public object RunFunction(string functionName, object[] parameters)
        {
            string path, runFunctionName;
            if (!functionName.Contains("BackUp"))
            {
                runFunctionName = functionName;
                path = @"C:\AFSLUTNINGSEKSAMEN\Serverside\Cluster\Services\Monosoft.ServerSideFunctions.Service\bin\Debug\netcoreapp2.1\Dller\" + functionName + @"\";
            }
            else
            {
                var folderName = functionName.Split("BackUp")[0];
                runFunctionName = folderName;
                path = Directory.GetCurrentDirectory() + @"\Dller\" + folderName + @"\" + folderName + @"BackUps\" + functionName + @"\";
            }
            string fileName = functionName + ".dll";
            if (Directory.Exists(path))
            {
                var pathToRead = Path.Combine(path, fileName);

                object result = new object();
                try
                {
                    byte[] tempFileArray = File.ReadAllBytes(pathToRead);
                    var dll = Assembly.Load(tempFileArray);

                    foreach (Type type in dll.GetExportedTypes())
                    {
                        dynamic c = Activator.CreateInstance(type);
                        result = type.InvokeMember(runFunctionName, BindingFlags.InvokeMethod, null, c, parameters);

                        Console.WriteLine(result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return ex.ToString();
                }
                return result;
            }
            else
            {
                return "FUNCTION DOES NOT EXSIST!";
            }
        }
        #endregion
        #region DeleteFuncTests
        [Fact]
        public void DeleteFuncTest()
        {
            var actualResult = DeleteFunction("YepperDepper");
            bool expectedResult = true;

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void DeleteFuncTestFail()
        {
            var actualResult = DeleteFunction("gwegweg");
            bool expectedResult = false;

            Assert.Equal(expectedResult, actualResult);
        }


        public bool DeleteFunction(string functionName)
        {
            string path = @"C:\AFSLUTNINGSEKSAMEN\Serverside\Cluster\Services\Monosoft.ServerSideFunctions.Service\bin\Debug\netcoreapp2.1\Dller\" + functionName + @"\";

            bool returnBool = false;
            if (File.Exists(path + functionName + ".dll"))
            {
                Directory.Delete(path, true);
                returnBool = true;
            }

            return returnBool;
        }
        #endregion
    }
}
