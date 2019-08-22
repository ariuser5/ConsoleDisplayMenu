using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeCompile
{
	public class Script
	{

		private static Assembly BuildAssembly(string code) {

			CSharpCodeProvider provider = new CSharpCodeProvider();
			CompilerParameters compilerparams = new CompilerParameters {
				GenerateExecutable = false,
				GenerateInMemory = true
			};

			CompilerResults results =
			   provider.CompileAssemblyFromSource(compilerparams, code);

			if(results.Errors.HasErrors) {
				StringBuilder errors = new StringBuilder("Compiler Errors :\r\n");
				foreach(CompilerError error in results.Errors) {
					errors.AppendFormat("Line {0},{1}\t: {2}\n",
						   error.Line, error.Column, error.ErrorText);
				}
				throw new Exception(errors.ToString());
			} else {
				return results.CompiledAssembly;
			}
		}

		public static object Execute(
			string source,
			string typeName = "",
			string methodName = "Main",
			bool isStatic = true,
			params object[] args) {

			object retVal = null;
			string code = File.ReadAllText(source);
			object instance = null;
			Type type = null;
			Assembly asm = BuildAssembly(code);

			if(isStatic) {
				type = asm.GetType(typeName);
			} else {
				instance = asm.CreateInstance(typeName);
				type = instance.GetType();
			}

			MethodInfo method = type.GetMethod(methodName);
			retVal = method.Invoke(instance, args);
			return retVal;
		}

	}
}
