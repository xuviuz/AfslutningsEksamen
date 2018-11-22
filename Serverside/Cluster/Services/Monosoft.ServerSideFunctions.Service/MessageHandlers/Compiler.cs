using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
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

        public string CreateDll(string functionName, string functionString, DTO.FunctionDefinitions jsonObj)
        {
            string path = Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\";

            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Dller\" + functionName);
                Directory.CreateDirectory(path + functionName + "BackUp");
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

        public object RunDll(string functionName, object[] parameters)
        {

            string path = Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\";
            string fileName = functionName + ".dll";
            if(Directory.Exists(path))
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
            else
            {
                return "FUNCTION DOES NOT EXSIST!";
            }
            
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

        public bool DeleteFunc(string functionName)
        {
            string path = Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\";

            bool returnBool = false;
            if (File.Exists(path + functionName + ".dll"))
            {
                Directory.Delete(path, true);
                returnBool = true;
            }

            return returnBool;
        }
        public string UpdateDLL(string functionName, string functionString, DTO.FunctionDefinitions jsonobj)
        {

            string path = Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\";
            string pathForBackUp = path + functionName + @"BackUp\" + functionName + "BackUp" + (Directory.GetDirectories(path + functionName + @"BackUp").Count() + 1) + @"\" + functionName + "BackUp" + (Directory.GetDirectories(path + functionName + @"BackUp").Count() + 1);

            if (File.Exists(path + functionName + ".dll"))
            {
                Directory.CreateDirectory(path + functionName + @"BackUp\" + functionName + "BackUp" +  (Directory.GetDirectories(path + functionName + @"BackUp").Count() + 1));


                File.Move(path + functionName + ".dll", pathForBackUp + ".dll");
                File.Move(path + functionName + ".json", pathForBackUp + ".json");


                string fileName = functionName + ".dll";
                var pathToEmit = Path.Combine(Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\", fileName);

                SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(functionString);

                var compilation = CSharpCompilation.Create(fileName, new SyntaxTree[] { syntaxTree }, defaultReferences, options);

                string res = "Error whole updating " + functionName;
                try
                {
                    var result = compilation.Emit(pathToEmit);
                    if (result.Success)
                    {
                        res = functionName + " was updated!";

                        File.WriteAllText(Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\" + functionName + ".json", JsonConvert.SerializeObject(jsonobj));


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
                return "FUNCTION DOES NOT EXSIST!";
            }

        }

        public string ReadDll(string name)
        {
            string path = Directory.GetCurrentDirectory() + @"\Dller\" + name + @"\";
            if (File.Exists(path + name + ".json"))
            {
                DTO.FunctionDefinitions func = JsonConvert.DeserializeObject<DTO.FunctionDefinitions>(File.ReadAllText(path + name + ".json"));
                return func.FunctionData;
            }
            else
            {
                return "NAVN GIVET FUNCTION FINDES IKKE!";
            }



        }
        public string ReadAllDll()
        {
            string path = Directory.GetCurrentDirectory() + @"\Dller";
            var directories = Directory.GetDirectories(path);
            int directoriesCount = directories.Length;
            string directoriesString = "";

            for (int i = 0; i < directoriesCount; i++)
            {
                directoriesString += directories[i].Remove(0, path.Length).Replace(@"\", " ") + "\r\n";
            }

            return directoriesString;
        }
    }
}
