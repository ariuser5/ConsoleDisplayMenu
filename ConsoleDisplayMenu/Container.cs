using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	public class Container : JsonObject
	{
		public enum LayoutType
		{
			Horizontal,
			Vertical
		}

		[JsonProperty]
		public LayoutType layout;
		[JsonProperty("components")]
		public List<JsonObject> components;

		[JsonProperty]
		public int Width;
		[JsonProperty]
		public int Height;

		[JsonProperty]
		public int LeftMargin;
		[JsonProperty]
		public int TopMargin;
		[JsonProperty]
		public int RightMargin;
		[JsonProperty]
		public int BottomMargin;


		[JsonIgnore]
		public IEnumerable<JsonObject> Components {
			get {
				IEnumerable<JsonObject> aggregateChildren(List<JsonObject> seed, IEnumerable<JsonObject> children) {
					foreach(JsonObject ch in children) {
						seed.Add(ch);

						if(ch as Container != null)
							aggregateChildren(seed, (ch as Container).components);
					}

					return seed;
				}

				return aggregateChildren(new List<JsonObject>(), components);
			}
		}


		protected Container(string name, JsonObjectType type) : base(name, type) { }


		internal void DeserializeComponents(IEnumerable<string> inners) {
			components = new List<JsonObject>();

			foreach(string inner in inners) {
				var deserialized = IsMeta(inner) ? DeserializeMeta(ReadMeta(inner)) : Deserialize(inner);

				components.Add(deserialized);
			}
		}

		public JsonObject Find(string name) => Components.First(c => c.name == name);
		public T Find<T>(string name) where T : JsonObject => (T) Components.First(c => c.name == name);
		public Text FindText(string name) => (Text) Components.First(c => c.name == name);
		public Pref FindPref(string name) => (Pref) Components.First(c => c.name == name);
		public Div FindDiv(string name) => (Div) Components.First(c => c.name == name);
		public Page FindPage(string name) => (Page) Components.First(c => c.name == name);

	}
}
