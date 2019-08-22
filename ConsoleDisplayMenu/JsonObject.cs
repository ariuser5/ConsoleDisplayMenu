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
		Proto, Text, Pref, Script, Div, Page
	}


	public abstract class JsonObject : IRenderable
	{

		public static implicit operator JsonObject(string json) {
			return new Proto();
		}


		protected static List<JsonObject> instances = new List<JsonObject>();



		internal static JsonObject Deserialize(string json) {
			JsonSerializerSettings settings = new JsonSerializerSettings() {
				DefaultValueHandling = DefaultValueHandling.Ignore,
				ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,

			};
			Proto protoObject = JsonConvert.DeserializeObject<Proto>(json, settings);
			var protoType = protoObject.type;

			switch(protoType) {
				case JsonObjectType.Proto:
					throw new Exception("Unexpected Behaviour");

				case JsonObjectType.Text:
					return JsonConvert.DeserializeObject<Unit>(json, settings);

				case JsonObjectType.Pref:
					return JsonConvert.DeserializeObject<Pref>(json, settings);

				case JsonObjectType.Script:
					return JsonConvert.DeserializeObject<MethodCall>(json, settings);

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
					case '$': return (MethodCall) Tools.ReadBetween(meta, '{', '}');
					default: return (Unit) meta;
				}

			} else {
				return new Div(null, children: new List<object>(metas));
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
				case JsonObjectType.Proto: return "Proto";
				case JsonObjectType.Text: return "Text";
				case JsonObjectType.Pref: return "Pref";
				case JsonObjectType.Script: return "Script";
				case JsonObjectType.Div: return "Div";
				case JsonObjectType.Page: return "Page";
				default: throw new NotImplementedException();
			}
		}



		[JsonIgnore]
		protected internal Container _parent;


		[JsonProperty(Order = 0)]
		public string name;

		[JsonProperty(Order = 1)]
		public JsonObjectType type;


		[JsonIgnore]
		public Container Parent {
			get => _parent;
			set {
				if(value == _parent) return;
				if(_parent != null) _parent.children.Remove(this);

				_parent = value;
				value?.children.Add(this);
			}
		}

		[JsonIgnore]
		public Container Root {
			get {
				Container current = _parent;

				while(current?._parent != null) current = current._parent;

				return current;
			}
		}


		protected JsonObject(string name, JsonObjectType type) : base() {
			this.type = type;
			this.name = Tools.GetUniqueName(instances.Select(i => i.name), name, GetDefaultName(type));

			if(type != JsonObjectType.Proto)
				instances.Add(this);
		}

		internal virtual void Dump() {
			instances.Remove(this);

			_parent?.children.Remove(this);
			_parent = null;
		}

		public abstract object Evaluate();

		public string ToJson() => JsonConvert.SerializeObject(this, formatting: Formatting.Indented);


		internal class Proto : JsonObject
		{
			public Proto() : base("proto", JsonObjectType.Proto) { }

			public override object Evaluate() {
				throw new Exception(string.Format("{0} as JsonObject derived type cannot be evaulate", typeof(Proto).ToString()));
			}
		}
	}
}
