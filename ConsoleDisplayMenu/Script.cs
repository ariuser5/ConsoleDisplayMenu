using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{

	public class Script : JsonObject, IReferenceValue
	{

		public static implicit operator Script(string json) {
			string[] readAddress() {
				if(json.StartsWith("$")) {
					var retStr = string.Empty;
					var depth = 0;

					foreach(char ch in json) {

						switch(ch) {
							case '{':
								depth++;
								if(depth == 1) continue;
								break;

							case '}':
								depth--;
								break;
						}

						if(retStr.Length > 0 && depth == 0)
							return retStr.Split('>');

						if(depth > 0)
							retStr += ch;

					}

					throw new Exception("Unexpected behaviour");

				} else return json.Split('>');
			}

			var address = readAddress();
			var callId = address[1].Split('(')[0].Split('.');

			Script newScript = new Script() {
				inner = JsonToMeta(json),
				name = "Script" + unnamedScriptCount,
				metaDefined = true,
				source = address[0],
				@namespace = callId.Count() == 3 ? callId[0] : string.Empty,
				className = callId.Count() == 3 ? callId[1] : callId[0],
				methodName = callId.Count() == 3 ? callId[2] : callId[1],
				argBlock = address[1].Split('(')[1].Split(')')[0]
			};

			newScript.DeserializeArgs();
			unnamedScriptCount++;

			return newScript;
		}




		[DefaultValue("")]
		[JsonProperty(Order = 2)]
		public string source;

		[DefaultValue("")]
		[JsonProperty(Order = 3)]
		public string @namespace;

		[DefaultValue("")]
		[JsonProperty(Order = 4)]
		public string className;

		[DefaultValue("")]
		[JsonProperty(Order = 5)]
		public string methodName;

		[DefaultValue(true)]
		[JsonProperty(Order = 6, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
		public bool isStatic;

		[JsonProperty("args", Order = 7)]
		public List<JsonObject> args;

		[JsonIgnore]
		public string TypeAddress {
			get => @namespace != string.Empty ? @namespace + '.' + className : className;
		}



		[JsonConstructor]
		private Script(
			string name,
			string source = "",
			string @namespace = "",
			string className = "",
			string methodName = "",
			bool isStatic = true,
			IEnumerable<JsonObject> args = null
			) : base(name, JsonObjectType.Script) {

			this.source = source;
			this.@namespace = @namespace;
			this.className = className;
			this.methodName = methodName;
			this.isStatic = isStatic;
			this.args = args.ToList();

			instances.Add(this);
		}

		public Script(
			string source = "",
			string @namespace = "",
			string className = "",
			string methodName = "",
			bool isStatic = true,
			IEnumerable<JsonObject> args = null
		) : this(null, source, @namespace, className, methodName, isStatic, args) { }

		public Script() : this(name: null) { }


		public object Invoke() {
			return RuntimeCompile.Script.Execute(source, @namespace, className, methodName, isStatic, args);
		}

		internal void DeserializeArgs(IEnumerable<string> inners) {
			foreach(string inner in inners) {
				//todo
				args.Add(Deserialize(inner));
			}
		}

		private IEnumerable<object> CompiledArguments() {
			var argValues = new List<object>();

			foreach(object arg in args)
				if(arg as IReferenceValue != null) argValues.Add(((IReferenceValue) arg).Invoke());
				else argValues.Add(((Text) arg).value);

			return argValues;
		}

		public override string ToString() {
			return "Script_" + name;
		}

		~Script() => instances.Remove(this);

	}
}
