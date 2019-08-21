using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	public enum JsonObjectType
	{
		JsonObject, Text, Pref, Script, Div, Page
	}


	public abstract class JsonObject : IEvaluate
	{

		//public static implicit operator JsonObject(string json) {
		//	var metas = ReadMetaComponents(json);

		//	if(metas.Count() == 0) {
		//		throw new Exception("Unexpected behaviour");

		//	} else if(metas.Count() == 1) {
		//		switch(metas.Single().First()) {
		//			case '#': return new JsonObject(JsonObjectType.Pref);
		//			case '$': return new JsonObject(JsonObjectType.Script);
		//			default: return new JsonObject(JsonObjectType.Text);
		//		}

		//	} else
		//		return new JsonObject(JsonObjectType.Div);
		//}

		public static implicit operator JsonObject(string json) {
			return new Proto();
		}


		protected static List<JsonObject> instances = new List<JsonObject>();




		public static string PresetsFile = "Presets.json";


		public static void Evaluate(string src) {
			object recursiveObject = src;

			do {
				if(recursiveObject.ToString().EndsWith(".json")) {
					var json = File.ReadAllText(recursiveObject.ToString());
					var jObject = Deserialize(json);

					recursiveObject = jObject.Evaluate();

				} else {
					var script = (Script) recursiveObject.ToString();

					recursiveObject = script.Evaluate();
					Console.WriteLine(recursiveObject);
				}

			} while(recursiveObject != null);
		}


		internal static JsonObject Deserialize(string json) {
			JsonSerializerSettings settings = new JsonSerializerSettings() {
				DefaultValueHandling = DefaultValueHandling.Ignore,
				ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,

			};
			Proto protoObject = JsonConvert.DeserializeObject<Proto>(json, settings);

			switch(protoObject.type) {
				case JsonObjectType.JsonObject:
					throw new Exception("Unexpected Behaviour");

				case JsonObjectType.Text:
					return JsonConvert.DeserializeObject<Text>(json, settings);

				case JsonObjectType.Pref:
					return JsonConvert.DeserializeObject<Pref>(json, settings);

				case JsonObjectType.Script:
					return JsonConvert.DeserializeObject<Script>(json, settings);

				case JsonObjectType.Div:
					return JsonConvert.DeserializeObject<Div>(json, settings);

				case JsonObjectType.Page:
					return JsonConvert.DeserializeObject<Page>(json, settings);

				default: throw new NotImplementedException();
			}
		}

		internal static JsonObject DeserializeMeta(string metaText) {
			var metas = ParseMetaText(metaText);

			if(metas.Count() == 0) {
				throw new Exception("Unexpected Behaviour");
			} else if(metas.Count() == 1) {

				var meta = metas.Single();

				switch(meta.First()) {
					case '#': return (Pref) Tools.ReadBetween(meta, '{', '}');
					case '$': return (Script) Tools.ReadBetween(meta, '{', '}');
					default: return (Text) meta;
				}

			} else {

				var newDiv = new Div() {
					layout = Container.LayoutType.Horizontal
				};

				foreach(string meta in metas) {
					var deserialized = DeserializeMeta(meta);

					newDiv.components.Add(deserialized);
				}

				return new Div();
			}
		}

		internal static IEnumerable<string> ParseMetaText(string meta) {
			List<string> metas = new List<string>();
			string temp0 = string.Empty;
			string temp1 = string.Empty;
			int depth = 0;
			int index = 0;

			bool Escaped() {
				if(index == 0) return false;
				if(meta[index - 1] == '\\') return true;
				return false;
			}
			void AddMetas() {
				if(temp0 != string.Empty && temp0.Any(ch => !char.IsWhiteSpace(ch))) {
					metas.Add(temp0.Trim());
					temp0 = string.Empty;
				}

				if(temp1 != string.Empty && temp1.Any(ch => !char.IsWhiteSpace(ch))) {
					metas.Add(temp1.Trim());
					temp1 = string.Empty;
				}
			}
			void ReadInner() {

				if(Escaped()) return;
				else temp1 += meta[index];

				for(int i = index + 1 ; i < meta.Length ; i++) {

					switch(meta[i]) {
						case '{':
							depth++;
							break;
						case '}':
							depth--;
							break;
					}

					temp1 += meta[i];

					if(depth == 0) {
						index = i + 1;
						return;
					}

				}

				temp0 = string.Concat(temp0, temp1);
				temp1 = string.Empty;
				depth = 0;
				index = meta.Length;
			}
			void Read() {

				while(index < meta.Length) {

					if(meta[index] == '#' || meta[index] == '$') {
						ReadInner();

						if(temp1 != string.Empty) {
							AddMetas();
							continue;
						}
					}

					if(index < meta.Length)
						temp0 += meta[index];
					index++;
				}

				if(temp0 != string.Empty) AddMetas();
			}

			Read();

			return metas;
		}

		internal static string ReadPropertyValue(string json, string propertyName) {
			var sb = new StringBuilder();

			using(JsonTextReader reader = new JsonTextReader(new StringReader(json))) {
				using(JsonTextWriter writer = new JsonTextWriter(new StringWriter(sb)) { Formatting = Formatting.Indented }) {

					while(reader.Read()) {
						if(reader.Path == propertyName) {
							reader.Read();
							writer.WriteToken(reader);
							return sb.ToString();
						}
					}

				}
			}

			throw new ArgumentException(string.Format("Property with name \"{0}\" could not be found", propertyName));
		}

		internal static IEnumerable<string> ReadPropertyArray(string json, string propertyArrayName) {
			List<string> jsons = null;
			var match = false;
			var sb = new StringBuilder();

			using(JsonTextReader reader = new JsonTextReader(new StringReader(json))) {
				using(JsonTextWriter writer = new JsonTextWriter(new StringWriter(sb)) { Formatting = Formatting.Indented }) {

					while(reader.Read()) {

						if(!match) {
							if(reader.Path == propertyArrayName && reader.TokenType == JsonToken.StartArray) {
								jsons = new List<string>();
								match = true;
							}
						} else {

							if(reader.Depth <= 1) break;

							writer.WriteToken(reader);
							jsons.Add(sb.ToString());
							sb.Clear();
						}

					}

				}
			}

			return jsons;
		}

		internal static bool IsMeta(string text) {
			if(text.StartsWith("{") && text.EndsWith("}")) return false;
			else return true;
		}


		internal static string GetDefaultName(JsonObjectType requestType) {
			switch(requestType) {
				case JsonObjectType.JsonObject: throw new Exception("Unexpected exception");
				case JsonObjectType.Text: return "Text";
				case JsonObjectType.Pref: return "Pref";
				case JsonObjectType.Script: return "Script";
				case JsonObjectType.Div: return "Div";
				case JsonObjectType.Page: return "Page";
				default: throw new NotImplementedException();
			}
		}

		internal static string AssignName(JsonObjectType type) {
			var prefix = GetDefaultName(type);
			int count = 0;

			while(true) {
				if(!instances.Any(item => item.name == prefix + count))
					return prefix + count;

				count++;
			}
		}

		internal static string AssignName(string name, JsonObjectType type) {
			if(name == null) return AssignName(type);

			var current = name;
			var suffix = string.Empty;
			int count = 1;

			while(true) {
				if(!instances.Any(item => item.name == current + suffix))
					return current + suffix;

				suffix = string.Format("({0})", count);
				count++;
			}
		}






		[JsonProperty(Order = 0)]
		public string name;

		[JsonProperty(Order = 1)]
		public JsonObjectType type;


		[JsonConstructor]
		private JsonObject() {
			type = JsonObjectType.JsonObject;
		}

		protected JsonObject(string name, JsonObjectType type) : base() {
			this.type = type;
			this.name = AssignName(name, type);

			instances.Add(this);
		}

		~JsonObject() => instances.Remove(this);



		public abstract object Evaluate();

		public string ToJson() => JsonConvert.SerializeObject(this, formatting: Formatting.Indented);


		internal class Proto : JsonObject
		{
			public override object Evaluate() {
				throw new Exception(string.Format("{0} as JsonObject derived type cannot be evaulate", typeof(Proto).ToString()));
			}
		}
	}
}
