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
    class FunctionHandler
    {
        private readonly IEnumerable<string> defaultNamespaces;

        private readonly string runtimePath;

        private readonly IEnumerable<MetadataReference> defaultReferences;

        private readonly CSharpCompilationOptions options;


        public FunctionHandler()
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

        public string CreateFunction(DTO.FunctionDefinitions jsonObj)
        {
            string functionName = jsonObj.Name;
            string functionString = jsonObj.FunctionData;

            string path = Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\";

            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Dller\" + functionName);
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
                return "FUNCTION '" + functionName + "' ALREADY EXSISTS!";
            }

            
        }

        public object RunFunction(string functionName, object[] parameters)
        {
            string path, runFunctionName;
            if (!functionName.Contains("BackUp"))
            {
                runFunctionName = functionName;
                path = Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\";
            }
            else
            {
                var folderName = functionName.Split("BackUp")[0];
                runFunctionName = folderName;
                path = Directory.GetCurrentDirectory() + @"\Dller\" + folderName + @"\" + folderName + @"BackUps\" + functionName + @"\";
            }
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
                return "FUNCTION '" + functionName + "' DOES NOT EXSIST!";
            }
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
            string paramFunctionString = "namespace ConvertParams { public class MyClass { public object[] GetArray() { " + 
                variables + " return new object[] { " + returnArray + " }; } } }";

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
     
        public string DeleteFunction(string functionName)
        {
            string path = Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\";

            if (File.Exists(path + functionName + ".dll"))
            {
                Directory.Delete(path, true);
                return "FUNCTION '" + functionName + "' WAS DELETED!";
            }
            return "FUNCTION '" + functionName + "' DOES NOT EXIST!";
        }

        public string UpdateFunction(DTO.FunctionDefinitions jsonobj)
        {
            string functionName = jsonobj.Name;
            string functionString = jsonobj.FunctionData;
            string path = Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\";

            if (File.Exists(path + functionName + ".dll"))
            {
                Directory.CreateDirectory(path + functionName + @"BackUps\" + functionName + "BackUp" +  (Directory.GetDirectories(path + functionName + @"BackUps").Count() + 1));
                string pathForBackUp = path + functionName + @"BackUps\" + functionName + "BackUp" + (Directory.GetDirectories(path + functionName + @"BackUps").Count()) + @"\" + functionName + "BackUp" + (Directory.GetDirectories(path + functionName + @"BackUps").Count());

                string fileName = functionName + "Holder.dll";
                var pathToEmit = Path.Combine(path, fileName);

                SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(functionString);

                var compilation = CSharpCompilation.Create(fileName, new SyntaxTree[] { syntaxTree }, defaultReferences, options);

                string res = "ERROR WHILE UPDATING '" + functionName + "'";
                try
                {
                    var result = compilation.Emit(pathToEmit);
                    if (result.Success)
                    {
                        res = "FUNCTION '" + functionName + "' WAS UPDATED!";

                        File.Move(path + functionName + ".dll", pathForBackUp + ".dll");
                        File.Move(path + functionName + ".json", pathForBackUp + ".json");

                        File.Move(path + functionName + "Holder.dll", path + functionName + ".dll");

                        File.WriteAllText(Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\" + functionName + ".json", JsonConvert.SerializeObject(jsonobj));
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

        public string ReadFunction(string functionName)
        {
            string path;
            if (!functionName.Contains("BackUp"))
            {
                path = Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\";
            }
            else
            {
                var folderName = functionName.Split("BackUp")[0];
                path = Directory.GetCurrentDirectory() + @"\Dller\" + folderName + @"\" + folderName + @"BackUps\" + functionName + @"\";
            }

            if (File.Exists(path + functionName + ".json"))
            {
                DTO.FunctionDefinitions func = JsonConvert.DeserializeObject<DTO.FunctionDefinitions>(File.ReadAllText(path + functionName + ".json"));
                return func.FunctionData;
            }
            else
            {
                return "FUNCTION '" + functionName + "' DOES NOT EXIST!";
            }
        }

        public string ReadAllFunctions(string functionName)
        {
            string path;
            if (string.IsNullOrEmpty(functionName))
            {
                path = Directory.GetCurrentDirectory() + @"\Dller";
            }
            else
            {
                if (functionName.Contains("BackUp"))
                {
                    return "FUNCTION '" + functionName + "' DOES NOT CONTAIN BACKUPS";
                }
                path = Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\" + functionName + "BackUps";
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
    }
}
