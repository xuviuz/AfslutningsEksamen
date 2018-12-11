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
            FunctionDefinitions funcDef = new FunctionDefinitions();
            funcDef.Id = "1";
            funcDef.Name = "ForTesting";
            funcDef.FunctionData = "";

            var actualResult = CreateFunction(funcDef);
            var expectedResult = funcDef.Name + " was created";

            Assert.Equal(expectedResult, actualResult);
        }
        [Fact]
        public void CreateFuncTestFail()
        {
            FunctionDefinitions funcDef = new FunctionDefinitions();
            funcDef.Id = "1";
            funcDef.Name = "Blergh";
            funcDef.FunctionData = "iwhegiwheg";

            var actualResult = CreateFunction(funcDef);
            var expectedResult = "Error while creating " + funcDef.Name;

            Assert.Equal(expectedResult, actualResult);

        }
        [Fact]
        public void CreateFuncTestAlreadyExsists()
        {
            FunctionDefinitions funcDef = new FunctionDefinitions();
            funcDef.Id = "1";
            funcDef.Name = "ForTesting";
            funcDef.FunctionData = "";

            var actualResult = CreateFunction(funcDef);
            var expectedResult = "FUNCTION ALREADY EXSISTS!";

            Assert.Equal(expectedResult, actualResult);
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
        public string CreateFunction(FunctionDefinitions jsonObj)
        {
            string functionName = jsonObj.Name;
            string functionString = jsonObj.FunctionData;

            string path = @"C:\AFSLUTNINGSEKSAMEN\Serverside\Cluster\Services\Monosoft.ServerSideFunctions.Service\bin\Debug\netcoreapp2.1\Dller\" + functionName + @"\";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(@"C:\AFSLUTNINGSEKSAMEN\Serverside\Cluster\Services\Monosoft.ServerSideFunctions.Service\bin\Debug\netcoreapp2.1\Dller\" + functionName);
                Directory.CreateDirectory(path + functionName + "BackUps");
                string fileName = functionName + ".dll";
                var pathToEmit = Path.Combine(path, fileName);

                SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(functionString);

                var compilation = CSharpCompilation.Create(fileName, new SyntaxTree[] { syntaxTree }, defaultReferences, options);

                string res = "Error while creating " + functionName;
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
                        DeleteFunction(functionName);
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
        public void RunFuncBackupTest()
        {
            var actualResult = RunFunction("YepserBackUp1", null);
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
                path = @"C:\AFSLUTNINGSEKSAMEN\Serverside\Cluster\Services\Monosoft.ServerSideFunctions.Service\bin\Debug\netcoreapp2.1\Dller\" + folderName + @"\" + folderName + @"BackUps\" + functionName + @"\";
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
            var actualResult = DeleteFunction("ForTesting");
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
        #region ParameterConvert
        [Fact]
        public void ParameterConvertTestInt()
        {
            var actualResult = ConvertToObjectArray("int: 1");
            var expectedResult = 1;

            Assert.Equal(expectedResult, actualResult[0]);
        }
        [Fact]
        public void ParameterConvertTestString()
        {

            var actualResult = ConvertToObjectArray($"string: \"hej\" ");
            var expectedResult = "hej";

            Assert.Equal(expectedResult, actualResult[0]);
        }
        [Fact]
        public void ParameterConvertTestStringAndInt()
        {
            var actualResult = ConvertToObjectArray("int: 1,string: \"hej\" ");
            var expectedResult1 = 1;
            var expectedResult2 = "hej";


            Assert.Equal(expectedResult1, actualResult[0]);
            Assert.Equal(expectedResult2, actualResult[1]);
        }
        [Fact]
        public void ParamaterConvertTestFail()
        {
            var actualResult = ConvertToObjectArray("srh" +
                "");
            var expectedResult = 0;

            Assert.Equal(expectedResult, actualResult.Length);
        }

        public object[] ConvertToObjectArray(string inputString)
        {
            if (string.IsNullOrEmpty(inputString.Replace(" ", "")) || !inputString.Contains(":"))
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
        #endregion
        #region UpdateFuncTest
        [Fact]
        public void UpdateFuncTesting()
        {
            FunctionDefinitions funcDef = new FunctionDefinitions();
            funcDef.Id = "1";
            funcDef.Name = "ForTesting";
            funcDef.FunctionData = "";

            var actualResult = UpdateFunction(funcDef);
            var expectedResult = funcDef.Name + " was updated!";

            Assert.Equal(expectedResult, actualResult);
        }
        [Fact]
        public void UpdateFuncTestingFail()
        {
            FunctionDefinitions funcDef = new FunctionDefinitions();
            funcDef.Id = "1";
            funcDef.Name = "ForTesting";
            funcDef.FunctionData = "egrs";

            var actualResult = UpdateFunction(funcDef);
            var expectedResult = "Error while updating " + funcDef.Name;

            Assert.Equal(expectedResult, actualResult);
        }
        [Fact]
        public void UpdateFuncTestingDoesNotExist()
        {

            FunctionDefinitions funcDef = new FunctionDefinitions();
            funcDef.Id = "1";
            funcDef.Name = "3rgwet";
            funcDef.FunctionData = "egrs";

            var actualResult = UpdateFunction(funcDef);
            var expectedResult = "FUNCTION '" + funcDef.Name + "' DOES NOT EXIST!";

            Assert.Equal(expectedResult, actualResult);
        }

        public string UpdateFunction(FunctionDefinitions jsonobj)
        {
            string functionName = jsonobj.Name;
            string functionString = jsonobj.FunctionData;
            string path = @"C:\AFSLUTNINGSEKSAMEN\Serverside\Cluster\Services\Monosoft.ServerSideFunctions.Service\bin\Debug\netcoreapp2.1\Dller\" + functionName + @"\";

            if (File.Exists(path + functionName + ".dll"))
            {
                Directory.CreateDirectory(path + functionName + @"BackUps\" + functionName + "BackUp" + (Directory.GetDirectories(path + functionName + @"BackUps").Count() + 1));
                string pathForBackUp = path + functionName + @"BackUps\" + functionName + "BackUp" + (Directory.GetDirectories(path + functionName + @"BackUps").Count()) + @"\" + functionName + "BackUp" + (Directory.GetDirectories(path + functionName + @"BackUps").Count());




                string fileName = functionName + "Holder.dll";
                var pathToEmit = Path.Combine(path, fileName);

                SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(functionString);

                var compilation = CSharpCompilation.Create(fileName, new SyntaxTree[] { syntaxTree }, defaultReferences, options);

                string res = "Error while updating " + functionName;
                try
                {
                    var result = compilation.Emit(pathToEmit);
                    if (result.Success)
                    {
                        res = functionName + " was updated!";

                        File.Move(path + functionName + ".dll", pathForBackUp + ".dll");
                        File.Move(path + functionName + ".json", pathForBackUp + ".json");

                        File.Move(path + functionName + "Holder.dll", path + functionName + ".dll");

                        File.WriteAllText(@"C:\AFSLUTNINGSEKSAMEN\Serverside\Cluster\Services\Monosoft.ServerSideFunctions.Service\bin\Debug\netcoreapp2.1\Dller\" + functionName + @"\" + functionName + ".json", JsonConvert.SerializeObject(jsonobj));


                    }
                    else
                    {
                        File.Delete(path + functionName + "Holder.dll");
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
                return "FUNCTION '" + functionName + "' DOES NOT EXIST!";
            }

        }
        #endregion
        #region ReadFuncTest
        [Fact]
        public void ReadFuncTest()
        {
            var actualResult = ReadFunction("ForTesting");
            var expectedResult = "";

            Assert.Equal(expectedResult, actualResult);
        }
        [Fact]
        public void ReadFuncTestFail()
        {
            var actualResult = ReadFunction("wegweg");
            var expectedResult = "FUNCTION 'wegweg' DOES NOT EXIST!";

            Assert.Equal(expectedResult, actualResult);
        }
        [Fact]
        public void ReadFuncTestBackup()
        {
            var actualResult = ReadFunction("ForTestingBackUp1");
            var expectedResult = "";

            Assert.Equal(expectedResult, actualResult);
        }
        [Fact]
        public void ReadFuncTestBackupFail()
        {
            var actualResult = ReadFunction("wegwegBackUp");
            var expectedResult = "FUNCTION 'wegwegBackUp' DOES NOT EXIST!";

            Assert.Equal(expectedResult, actualResult);
        }

        public string ReadFunction(string functionName)
        {
            string path;
            if (!functionName.Contains("BackUp"))
            {
                path = @"C:\AFSLUTNINGSEKSAMEN\Serverside\Cluster\Services\Monosoft.ServerSideFunctions.Service\bin\Debug\netcoreapp2.1\Dller\" + functionName + @"\";
            }
            else
            {
                var folderName = functionName.Split("BackUp")[0];
                path = @"C:\AFSLUTNINGSEKSAMEN\Serverside\Cluster\Services\Monosoft.ServerSideFunctions.Service\bin\Debug\netcoreapp2.1\Dller\" + folderName + @"\" + folderName + @"BackUps\" + functionName + @"\";
            }

            if (File.Exists(path + functionName + ".json"))
            {
                FunctionDefinitions func = JsonConvert.DeserializeObject<FunctionDefinitions>(File.ReadAllText(path + functionName + ".json"));
                return func.FunctionData;
            }
            else
            {
                return "FUNCTION '" + functionName + "' DOES NOT EXIST!";
            }
        }
        #endregion
        #region ReadAllFuncTest

        [Fact]
        public void ReadAllFuncTest()
        {
            var acctualResult = ReadAllFunctions("");
            Assert.NotEmpty(acctualResult);
        }
        [Fact]
        public void ReadAllFuncBackupTest()
        {
            var actualResult = ReadAllFunctions("ForTesting");
            var expectedResult = "ForTestingBackUp1";

            Assert.Contains(expectedResult, actualResult);
        }
        [Fact]
        public void ReadAllFuncFail()
        {
            var actualResult = ReadAllFunctions("wegwg");
            var expectedResult = "FUNCTION 'wegwg' DOES NOT EXIST!";

            Assert.Equal(expectedResult, actualResult);

        }
        [Fact]
        public void ReadAllFuncBackupFail()
        {
            var actualResult = ReadAllFunctions("ForTestingBackUp");
            var expectedResult = "FUNCTION 'ForTestingBackUp' DOES NOT CONTAIN BACKUPS";

            Assert.Equal(expectedResult, actualResult);
        }
        public string ReadAllFunctions(string functionName)
        {
            string path;
            if (string.IsNullOrEmpty(functionName))
            {
                path = @"C:\AFSLUTNINGSEKSAMEN\Serverside\Cluster\Services\Monosoft.ServerSideFunctions.Service\bin\Debug\netcoreapp2.1\Dller\";
            }
            else
            {
                if (functionName.Contains("BackUp"))
                {
                    return "FUNCTION '" + functionName + "' DOES NOT CONTAIN BACKUPS";
                }
                path = @"C:\AFSLUTNINGSEKSAMEN\Serverside\Cluster\Services\Monosoft.ServerSideFunctions.Service\bin\Debug\netcoreapp2.1\Dller\" + functionName + @"\" + functionName + "BackUps";
                if (!Directory.Exists(path))
                {
                    return "FUNCTION '" + functionName + "' DOES NOT EXIST!";
                }
            }
            var directories = Directory.GetDirectories(path);
            if (directories.Length == 0)
            {
                return "THERE ARE NO FUNCTIONS / BACKUPS";
            }
            string directoriesString = "";

            for (int i = 0; i < directories.Length; i++)
            {
                directoriesString += directories[i].Remove(0, path.Length).Replace(@"\", " ") + "\r\n";
            }

            return directoriesString;
        }
        #endregion
    }
}
