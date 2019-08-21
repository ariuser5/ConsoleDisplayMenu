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

		[JsonProperty(Order = 2)]
		public LayoutType layout;
		[JsonProperty("components")]
		public List<JsonObject> components;

		[JsonProperty(Order = 3)]
		public int Width;
		[JsonProperty(Order = 4)]
		public int Height;

		[JsonProperty(Order = 5)]
		public int LeftMargin;
		[JsonProperty(Order = 6)]
		public int TopMargin;
		[JsonProperty(Order = 7)]
		public int RightMargin;
		[JsonProperty(Order = 8)]
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


		protected Container(string name, JsonObjectType type, IEnumerable<object> components) : base(name, type) {
			DeserializeComponents(components);
		}



		public override object Evaluate() {
			var sb = string.Empty;

			foreach(JsonObject child in components) {
				sb += child.Evaluate().ToString();

				switch(layout) {
					case LayoutType.Horizontal:
						sb += " ";
						break;

					case LayoutType.Vertical:
						sb += '\n';
						break;

					default: throw new NotImplementedException();
				}
			}

			return sb;
		}

		private void DeserializeComponents(IEnumerable<object> inners) {
			components = new List<JsonObject>();

			foreach(string inner in inners.Select(obj => obj.ToString())) {
				var deserialized = IsMeta(inner) ? DeserializeMeta(inner) : Deserialize(inner);

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
