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

	public class MethodCall : Container
	{

		public static implicit operator MethodCall(string meta) {
			var addr = MethodAddress.Parse(meta);
			var argsMeta = Tools.ReadBetween(meta, '(', ')');
			var args = argsMeta.
				Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).
				Select(str => str.Trim()).
				AsEnumerable<object>();

			return new MethodCall(addr.source, addr.namespaces, addr.className, addr.methodName, true, args);
		}




		[DefaultValue("")]
		[JsonProperty(Order = 2)]
		public string source;

		[DefaultValue("")]
		[JsonProperty(Order = 3)]
		public List<string> namespaces;

		[DefaultValue("")]
		[JsonProperty(Order = 4)]
		public string className;

		[DefaultValue("")]
		[JsonProperty(Order = 5)]
		public string methodName;

		[DefaultValue(true)]
		[JsonProperty(Order = 6, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
		public bool isStatic;




		[JsonIgnore]
		public string TypeName {
			get => namespaces.Aggregate(string.Empty, (seed, w) => seed + w + '.') + className;
		}

		[JsonIgnore]
		public IEnumerable<JsonObject> Args {
			get => children;
			set => BaseChildren = value;
		}

		[JsonIgnore]
		public object[] CompiledArgs {
			get {
				return (from arg in Args select arg.Evaluate()).ToArray();
			}
		}

		[JsonIgnore]
		public Page BasePage {
			get {
				Container current = _parent;

				while(current?._parent != null && current?.type != JsonObjectType.Page)
					current = current._parent;

				return current as Page;
			}
		}



		[JsonConstructor]
		private MethodCall(
			string name = null,
			string source = null,
			IEnumerable<string> namespaces = null,
			string className = null,
			string methodName = null,
			bool isStatic = false,
			[JsonProperty("args")]IEnumerable<object> args = null
			) : base(name, JsonObjectType.Script, args) {

			this.source = source;
			this.namespaces = namespaces.ToList();
			this.className = className;
			this.methodName = methodName;
			this.isStatic = isStatic;
		}

		public MethodCall(
			string source = "",
			IEnumerable<string> namespaces = null,
			string className = "",
			string methodName = "",
			bool isStatic = true,
			IEnumerable<object> args = null
		) : this(null, source, namespaces, className, methodName, isStatic, args) { }


		private MethodAddress GetAddress() {
			return new MethodAddress() {
				source = source,
				namespaces = namespaces,
				className = className,
				methodName = methodName
			};
		}



		public override object Evaluate() {
			if(source == string.Empty) {

				object retVal = null;
				Type type = null;
				object instance = null;
				Assembly assem = Assembly.GetExecutingAssembly();
				MethodInfo method = null;

				if(isStatic) {
					type = assem.GetType(TypeName);
				} else {
					instance = assem.CreateInstance(TypeName);
					type = instance.GetType();
				}

				method = type.GetMethod(methodName);
				retVal = method.Invoke(instance, CompiledArgs);
				return retVal;

			} else {
				return RuntimeCompile.Script.Execute(source, TypeName, methodName, isStatic, CompiledArgs);
			}

			//var temp = GetAddress();

			//if(temp.source == string.Empty) {
			//	var envAddress = !string.IsNullOrEmpty(BasePage?.script) ? MethodAddress.Parse(BasePage.script) : null;

			//	if(envAddress == null) throw new Exception("Missing address. Method cannot be evaluated");

			//	temp.source = temp.source != string.Empty ? temp.source : envAddress.source;

			//	if(temp.className == string.Empty) {
			//		temp.className = envAddress.className;
			//		temp.namespaces = envAddress.namespaces;
			//	}
			//}

			//return RuntimeCompile.Script.Execute(temp.source, temp.TypeName, temp.methodName, isStatic, CompiledArgs);
		}

		public override string ToString() {
			return "Script_" + name;
		}





		private class MethodAddress
		{

			public static MethodAddress Parse(string text) {
				var address = new string(text.TakeWhile(ch => ch != '(').ToArray());
				var source = string.Empty;
				var className = string.Empty;
				var methodName = string.Empty;
				var scope = string.Empty;
				List<string> namespaces = new List<string>();

				Stack<string> addrSlots;

				if(address.Contains('>')) {
					source = new string(text.TakeWhile(ch => ch != '>').ToArray());
					scope = address.Substring(source.Length + 1);
				} else {
					scope = address.Substring(source.Length);
				}

				addrSlots = new Stack<string>(scope.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

				if(text.Contains('(')) {
					methodName = addrSlots.Pop();
					className = addrSlots.Count > 1 ? addrSlots.Pop() : className;
				} else {
					className = addrSlots.Pop();
				}

				while(addrSlots.Count > 0)
					namespaces.Add(addrSlots.Pop());

				namespaces.Reverse();

				return new MethodAddress() {
					source = source,
					namespaces = namespaces,
					className = className,
					methodName = methodName
				};
			}


			public string source;
			public string className;
			public string methodName;
			public List<string> namespaces;



			public string TypeName {
				get => namespaces.Aggregate(string.Empty, (seed, w) => seed + w + '.') + className;
			}

			public override string ToString() {
				return TypeName + '.' + methodName;
			}

		}

	}
}
