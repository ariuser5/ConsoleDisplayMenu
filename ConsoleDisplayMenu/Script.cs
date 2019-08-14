using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	public class Script : JsonObject
	{

		public static implicit operator Script(string json) {
			Script newScript = new Script() {
				json = json,
				name = "Script" + unnamedPresetCount,
			};
			unnamedPresetCount++;

			return newScript;
		}


		//todo
		[JsonProperty]
		public string source;
		[JsonProperty]
		public string @namespace;
		[JsonProperty]
		public string className;
		[JsonProperty]
		public string methodName;
		[JsonProperty]
		public string[] args;
		[JsonProperty]
		public bool isStatic;


		public Script() : base(JsonObjectType.Script) { }


		public object Invoke() {
			return RuntimeCompile.Script.Execute(source, @namespace, className, methodName, isStatic, args);
		}

	}
}
