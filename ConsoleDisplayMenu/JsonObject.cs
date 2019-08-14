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
			JsonObject, Text, Preset, Script, Div, Page
		}

		public static implicit operator JsonObject(string json) {
			var metas = ReadMetaComponents(json);

			if(metas.Count() == 0) {
				throw new Exception("Unexpected behaviour");

			} else if(metas.Count() == 1) {
				return new JsonObject(JsonObjectType.Div) { metaDefined = true }; ;

			} else {

				switch(metas.Single().First()) {
					case '#':
						return new JsonObject(JsonObjectType.Preset) { metaDefined = true };

					case '$':
						return new JsonObject(JsonObjectType.Script) { metaDefined = true };

					default:
						return new JsonObject(JsonObjectType.Text) { metaDefined = true };
				}
			}
		}



		internal static int unnamedTextCount = 0;
		internal static int unnamedPresetCount = 0;
		internal static int unnamedScritpCount = 0;
		internal static int unnamedDivCount = 0;
		internal static int unnamedPageCount = 0;


		public static string PresetsFile = "Presets.json";


		public static void Render(string page) {
			//todo
		}


		public static JsonObject Deserialize(string json) {
			JsonObject jsonObj = JsonConvert.DeserializeObject<JsonObject>(json);
			jsonObj.json = json;

			switch(jsonObj.type) {
				case JsonObjectType.Text:
					return jsonObj.ConvertTo(JsonObjectType.Text);

				case JsonObjectType.Preset:
					return jsonObj.ConvertTo(JsonObjectType.Preset);

				case JsonObjectType.Script:
					return jsonObj.ConvertTo(JsonObjectType.Script);

				case JsonObjectType.Div:
					return jsonObj.ConvertTo(JsonObjectType.Div);

				case JsonObjectType.Page:
					return jsonObj.ConvertTo(JsonObjectType.Page);

				default: throw new NotImplementedException();
			}
		}

		public static IEnumerable<string> ReadJsonComponents(string json) {
			List<string> jsons = new List<string>();
			StringBuilder sb = new StringBuilder();

			using(JsonTextReader reader = new JsonTextReader(new StringReader(json))) {
				using(JsonTextWriter writer = new JsonTextWriter(new StringWriter(sb))) {

					if(reader.Path == "components") {
						writer.WriteToken(reader);
						jsons.Add(sb.ToString());
						sb.Clear();

					}
				}
			}

			return jsons;
		}

		public static IEnumerable<string> ReadMetaComponents(string json) {
			List<string> metas = new List<string>();
			string temp0 = string.Empty;
			string temp1 = string.Empty;
			int index = 0;

			bool escaped() {
				if(index == 0) return false;
				if(json[index - 1] == '\\') return true;
				return false;
			}
			void readInner() {
				int depth = 0;

				if(escaped()) return;
				else temp1 += json[index];

				for(int i = index + 1 ; i < json.Length ; i++) {

					switch(json[i]) {
						case '{':
							depth++;
							break;
						case '}':
							depth--;
							break;
					}

					temp1 += json[i];

					if(depth == 0) {

						if(json[i] == '}') {
							index = i + 1;
							return;
						} else {
							temp1 = string.Empty;
							return;
						}
					}

				}

				temp0 = string.Concat(temp0, temp1);
				temp1 = string.Empty;
				index = json.Length;
			}
			void read() {

				while(index < json.Length) {

					if(json[index] == '#' || json[index] == '$') {
						readInner();

						if(temp1 != string.Empty) {
							metas.Add(temp0);
							metas.Add(temp1);
							temp0 = string.Empty;
							temp1 = string.Empty;
							continue;
						}
					}

					temp0 += json[index];
					index++;
				}

				if(temp0 != string.Empty) metas.Add(temp0);

			}

			read();

			return metas;
		}


		protected bool metaDefined;

		[JsonIgnore]
		public string json;
		[JsonProperty]
		public string name;
		[JsonProperty]
		public JsonObjectType type;


		public JsonObject() {
			metaDefined = false;
			type = JsonObjectType.JsonObject;
		}

		public JsonObject(JsonObjectType type) : base() {
			this.type = type;
		}

		public JsonObject ConvertTo(JsonObjectType type) {
			JsonObject newObj;

			switch(type) {
				case JsonObjectType.JsonObject:
					throw new Exception("Converting a JsonObject instance to itself is not allowed");

				case JsonObjectType.Text:
					newObj = JsonConvert.DeserializeObject<Text>(json);
					newObj.json = json;
					newObj.metaDefined = metaDefined;

					if(name == null) {
						name = "Text" + unnamedTextCount;
						unnamedTextCount++;
					}

					break;
				case JsonObjectType.Preset:
					newObj = JsonConvert.DeserializeObject<Preset>(json);
					newObj.json = json;
					newObj.metaDefined = metaDefined;

					if(name == null) {
						name = "Preset" + unnamedPresetCount;
						unnamedPresetCount++;
					}

					break;
				case JsonObjectType.Script:
					newObj = JsonConvert.DeserializeObject<Script>(json);
					newObj.json = json;
					newObj.metaDefined = metaDefined;

					if(name == null) {
						name = "Script" + unnamedScritpCount;
						unnamedScritpCount++;
					}

					break;

				case JsonObjectType.Div:
					newObj = JsonConvert.DeserializeObject<Div>(json);
					newObj.json = json;
					newObj.metaDefined = metaDefined;
					((Container) newObj).DeserializeChildren();

					if(name == null) {
						name = "Div" + unnamedDivCount;
						unnamedDivCount++;
					}

					break;
				case JsonObjectType.Page:
					newObj = JsonConvert.DeserializeObject<Page>(json);
					newObj.json = json;
					newObj.metaDefined = metaDefined;
					((Container) newObj).DeserializeChildren();

					if(name == null) {
						name = "Page" + unnamedPageCount;
						unnamedPageCount++;
					}

					break;

				default:
					throw new NotImplementedException();
			}

			return newObj;
		}

	}
}
