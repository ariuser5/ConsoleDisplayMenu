using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	public class JsonObject
	{

		public enum JsonObjectType
		{
			JsonObject, Text, Pref, Script, Div, Page
		}

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
			return new JsonObject("MetaJsonObject", JsonObjectType.JsonObject);
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


		public static JsonObject Deserialize(string json) {
			JsonSerializerSettings settings = new JsonSerializerSettings() {
				DefaultValueHandling = DefaultValueHandling.Ignore,
				ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,

			};
			JsonObject jsonObject = JsonConvert.DeserializeObject<JsonObject>(json, settings);

			switch(jsonObject.type) {
				case JsonObjectType.JsonObject:
					throw new Exception("Unexpected Behaviour");

				case JsonObjectType.Text:
					jsonObject = JsonConvert.DeserializeObject<Text>(json, settings);
					break;

				case JsonObjectType.Pref:
					jsonObject = JsonConvert.DeserializeObject<Pref>(json, settings);
					break;

				case JsonObjectType.Script:
					jsonObject = JsonConvert.DeserializeObject<Script>(json, settings);
					((Script) jsonObject).DeserializeArgs(ReadPropertyArray(json, "args"));
					break;

				case JsonObjectType.Div:
					jsonObject = JsonConvert.DeserializeObject<Div>(json, settings);
					((Container) jsonObject).DeserializeComponents(ReadPropertyArray(json, "components"));
					break;

				case JsonObjectType.Page:
					jsonObject = JsonConvert.DeserializeObject<Page>(json, settings);
					((Container) jsonObject).DeserializeComponents(ReadPropertyArray(json, "components"));
					break;

				default: throw new NotImplementedException();
			}

			return jsonObject;
		}

		public static string ReadPropertyValue(string json, string propertyName) {
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

		public static IEnumerable<string> ReadPropertyArray(string json, string propertyArrayName) {
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

		internal static JsonObject DeserializeMeta(string metaText) {
			var metas = ParseMetaText(metaText);

			if(metas.Count() == 0) {
				throw new Exception("Unexpected Behaviour");
			} else if(metas.Count() == 1) {

				switch(metas.Single().First()) {
					case '#': return (Pref) metas.Single();
					case '$': return (Script) metas.Single();
					default: return (Text) metas.Single();
				}

			} else {

				var newDiv = new Div() {
					layout = Container.LayoutType.Horizontal
				};

				foreach(string meta in metas) {
					var deserialized = DeserializeMeta(meta);

					newDiv.components.Add(deserialized);
				}

				return newDiv;
			}
		}

		internal static IEnumerable<string> ParseMetaText(string meta) {
			List<string> metas = new List<string>();
			string temp0 = string.Empty;
			string temp1 = string.Empty;
			int depth = 0;
			int index = 0;

			bool escaped() {
				if(index == 0) return false;
				if(meta[index - 1] == '\\') return true;
				return false;
			}
			void addMetas() {
				if(temp0 != string.Empty) {
					metas.Add(temp0);
					temp0 = string.Empty;
				}

				if(temp1 != string.Empty) {
					metas.Add(temp1);
					temp1 = string.Empty;
				}
			}
			void readInner() {

				if(escaped()) return;
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
			void read() {

				while(index < meta.Length) {

					if(meta[index] == '#' || meta[index] == '$') {
						readInner();

						if(temp1 != string.Empty) {
							addMetas();
							continue;
						}
					}

					if(index < meta.Length)
						temp0 += meta[index];
					index++;
				}

				if(temp0 != string.Empty) addMetas();
			}

			read();

			return metas;
		}

		internal static string ReadMeta(string rawMeta) {
			return rawMeta.Substring(1, rawMeta.Length - 2);
			var meta = string.Empty;

			for(int i = 0 ; i < rawMeta.Length - 1 ; i++) {
				if(rawMeta[i] == '\\' && rawMeta[i - 1] == '\\') continue;

				meta += rawMeta[i];
			}

			return meta;
		}

		internal static bool IsMeta(string text) {
			if(text.StartsWith("\"") && text.EndsWith("\"")) return true;
			else return false;
		}



		//internal static string JsonToMeta(string jsonText) {
		//	var metaText = string.Empty;

		//	foreach(char ch in jsonText) {
		//		if(ch == '\\') metaText += ch;

		//		metaText += ch;
		//	}

		//	return string.Concat('"', metaText, '"');
		//}

		//internal static string MetaToJson(string metaText) {
		//	var json = string.Empty;

		//	for(int i = 1 ; i < metaText.Length - 2 ; i++) {
		//		if(metaText[i] == '\\' && metaText[i - 1] == '\\') continue;
		//		json += metaText[i];
		//	}

		//	return json;
		//}


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
		}

		public string ToJson() => JsonConvert.SerializeObject(this, formatting: Formatting.Indented);

	}
}
