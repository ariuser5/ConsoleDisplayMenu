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
		[JsonIgnore]
		public List<JsonObject> children;

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
				//todo
				//Return all JsonObject instances in this Container
				return null;
			}
		}


		protected Container(JsonObjectType type) : base(type) { }


		internal void DeserializeChildren() {
			IEnumerable<string> jsons = metaDefined ? ReadMetaComponents(json) : ReadJsonComponents(json);

			children = new List<JsonObject>();

			foreach(string component in jsons)
				children.Add(Deserialize(component));
		}

		public JsonObject Find(string tag) => Components.First(c => c.name == tag);
		public T Find<T>(string tag) where T : JsonObject => (T) Components.First(c => c.name == tag);
		public Text FindText(string tag) => (Text) Components.First(c => c.name == tag);
		public Preset FindPref(string tag) => (Preset) Components.First(c => c.name == tag);
		public Div FindDiv(string tag) => (Div) Components.First(c => c.name == tag);
		public Page FindPage(string tag) => (Page) Components.First(c => c.name == tag);
		public JsonObject Find(string name) => Components.First(c => c.name == name);
		public T Find<T>(string name) where T : JsonObject => (T) Components.First(c => c.name == name);
		public Text FindText(string name) => (Text) Components.First(c => c.name == name);
		public Pref FindPref(string name) => (Pref) Components.First(c => c.name == name);
		public Div FindDiv(string name) => (Div) Components.First(c => c.name == name);
		public Page FindPage(string name) => (Page) Components.First(c => c.name == name);

	}
}
