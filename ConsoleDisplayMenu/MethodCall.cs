using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{

	public class Script : JsonObject
	{

		public static implicit operator Script(string meta) {
			var address = new string(meta.TakeWhile(ch => ch != '(').ToArray());
			var source = string.Empty;
			var @namespace = string.Empty;
			var className = string.Empty;
			var methodName = string.Empty;
			IEnumerable<string> scope = null;
			IEnumerable<object> args = null;

			if(address.Contains('>')) {
				var delimiterIndex = address.IndexOf('>');

				source = address.Substring(0, delimiterIndex);
				scope = address.Substring(delimiterIndex + 1).Split('.');
			} else {
				scope = address.Split('.');
			}

			if(scope.Count() == 1) {
				methodName = scope.ElementAt(0);

			} else if(scope.Count() == 2) {
				className = scope.ElementAt(0);
				methodName = scope.ElementAt(1);

			} else if(scope.Count() == 3) {
				@namespace = scope.ElementAt(0);
				className = scope.ElementAt(1);
				methodName = scope.ElementAt(2);

			} else throw new Exception(
				string.Format(
					"{0} object scope could not be determined from: {1}",
					typeof(Script).ToString(),
					meta
				)
			);

			var temp = Tools.ReadBetween(meta, '(', ')');
			args = temp.
				Split(",".ToArray(), options: StringSplitOptions.RemoveEmptyEntries).
				Select(arg => arg.Trim());

			return new Script(null, source, @namespace, className, methodName, true, args);
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
			string name = null,
			string source = null,
			string @namespace = null,
			string className = null,
			string methodName = null,
			bool isStatic = false,
			IEnumerable<object> args = null
			) : base(name, JsonObjectType.Script) {

			this.source = source;
			this.@namespace = @namespace;
			this.className = className;
			this.methodName = methodName;
			this.isStatic = isStatic;

			DeserializeArgs(args);
		}

		public Script(
			string source = "",
			string @namespace = "",
			string className = "",
			string methodName = "",
			bool isStatic = true,
			IEnumerable<object> args = null
		) : this(null, source, @namespace, className, methodName, isStatic, args) { }




		public override object Evaluate() {
			if(source == string.Empty) {
				//Internal source
				if(@namespace != string.Empty)
					throw new Exception(string.Format("{0} objects that are calling internal methods must have namespace property as empty string", typeof(Script).ToString()));

				if(className != string.Empty)
					throw new Exception(string.Format("{0} objects that are calling internal methods must have className property as empty string", typeof(Script).ToString()));

				var method = typeof(JsonObject).GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
				return method.Invoke(null, CompiledArguments());
			} else {
				//External source
				return RuntimeCompile.Script.Execute(source, @namespace, className, methodName, isStatic, CompiledArguments());
			}
		}

		private void DeserializeArgs(IEnumerable<object> inners) {
			args = new List<JsonObject>();

			foreach(string inner in inners.Select(obj => obj.ToString())) {
				var deserialized = IsMeta(inner) ? DeserializeMeta(inner) : Deserialize(inner);

				args.Add(deserialized);
			}
		}

		private object[] CompiledArguments() {
			return (from arg in args select arg.Evaluate()).ToArray();
		}

		public override string ToString() {
			return "Script_" + name;
		}

	}
}
