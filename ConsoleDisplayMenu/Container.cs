using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	public abstract class Container : JsonObject
	{

		[JsonProperty("components", Order = 9)]
		protected internal List<JsonObject> children;


		[JsonIgnore]
		protected IEnumerable<JsonObject> BaseChildren {
			set {
				foreach(JsonObject child in children)
					child.Parent = null;

				foreach(JsonObject obj in value)
					obj.Parent = this;
			}
		}

		[JsonIgnore]
		public IEnumerable<JsonObject> Components {
			get {
				IEnumerable<JsonObject> aggregateChildren(List<JsonObject> seed, IEnumerable<JsonObject> children) {
					foreach(JsonObject ch in children) {
						seed.Add(ch);

						if(ch as Container != null)
							aggregateChildren(seed, (ch as Container).children);
					}

					return seed;
				}

				return aggregateChildren(new List<JsonObject>(), children);
			}
		}


		protected Container(string name, JsonObjectType type, IEnumerable<object> children) : base(name, type) {
			if(type == JsonObjectType.Script) DeserializeArgs(children);
			else DeserializeComponents(children);
		}

		internal override void Dump() {
			base.Dump();

			foreach(JsonObject child in children)
				child.Parent = null;
		}



		private void DeserializeComponents(IEnumerable<object> inners) {
			children = new List<JsonObject>();

			foreach(string inner in inners.Select(obj => obj.ToString())) {
				var deserialized = IsMeta(inner) ? DeserializeMeta(inner) : Deserialize(inner);

				deserialized.Parent = this;
			}
		}

		private void DeserializeArgs(IEnumerable<object> inners) {
			children = new List<JsonObject>();

			foreach(string inner in inners.Select(obj => obj.ToString())) {
				var deserialized = IsMeta(inner) ? DeserializeMeta(inner) : Deserialize(inner);

				deserialized.Parent = this;
			}
		}


		public JsonObject Find(string name) => Components.First(c => c.name == name);
		public T Find<T>(string name) where T : JsonObject => (T) Components.First(c => c.name == name);
		public Unit FindText(string name) => (Unit) Components.First(c => c.name == name);
		public MethodCall FindMethodCall(string name) => (MethodCall) Components.First(c => c.name == name);
		public Pref FindPref(string name) => (Pref) Components.First(c => c.name == name);
		public Div FindDiv(string name) => (Div) Components.First(c => c.name == name);
		public Page FindPage(string name) => (Page) Components.First(c => c.name == name);

	}
}
