using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	internal class JsonObject
	{

		public enum JsonObjectType
		{
			Text, Pref, Div, Page
		}

		public static JsonObject Deserialize(string json) {
			JsonObject jsonObj = JsonConvert.DeserializeObject<JsonObject>(json);

			switch(jsonObj.type) {
				case JsonObjectType.Text:
					jsonObj = (Text) json;
					break;
				case JsonObjectType.Pref:
					jsonObj = (Pref) json;
					break;
				case JsonObjectType.Div:
					jsonObj = (Div) json;
					break;
				case JsonObjectType.Page:
					jsonObj = (Page) json;
					break;
			}

			return jsonObj;
		}

		public static string ReadInnerJson(string json) {

		}



		[JsonIgnore]
		public string json;
		[JsonProperty]
		public string tag;
		[JsonProperty]
		public JsonObjectType type;







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
				}
			}


			protected void InitializeChildren() {
				//todo

				children = new List<JsonObject>();
			}

			public JsonObject Find(string tag) => Components.First(c => c.tag == tag);
			public T Find<T>(string tag) where T : JsonObject => (T) Components.First(c => c.tag == tag);
			public Text FindText(string tag) => (Text) Components.First(c => c.tag == tag);
			public Pref FindPref(string tag) => (Pref) Components.First(c => c.tag == tag);
			public Div FindDiv(string tag) => (Div) Components.First(c => c.tag == tag);
			public Page FindPage(string tag) => (Page) Components.First(c => c.tag == tag);

		}

		public class Text : JsonObject
		{

			public static implicit operator Text(string json) {
				Text newText = JsonConvert.DeserializeObject<Text>(json);
				newText.json = json;

				return newText;
			}

			[JsonProperty]
			public string value;

			public override string ToString() {
				return value;
			}

		}

		public class Pref : JsonObject
		{
			public enum PrefType
			{
				Value, Tag, Description
			}


			public static implicit operator Pref(string json) {
				Pref newPref = JsonConvert.DeserializeObject<Pref>(json);
				newPref.json = json;

				return newPref;
			}

			[JsonProperty]
			public string target;
			[JsonProperty]
			public PrefType prefType;


			public void WriteValue(string value) {

			}


			public string ReadValue() {

			}

		}

		public class Div : Container
		{
			public static implicit operator Div(string json) {
				Div newDiv = JsonConvert.DeserializeObject<Div>(json);
				newDiv.json = json;
				newDiv.InitializeChildren();

				return newDiv;
			}


			public Div() {
				LeftMargin = 0;
				TopMargin = 0;
				RightMargin = 0;
				BottomMargin = 0;
			}

		}

		public class Page : Container
		{

			public static implicit operator Page(string json) {
				Page newPage = JsonConvert.DeserializeObject<Page>(json);
				newPage.json = json;
				newPage.InitializeChildren();

				return newPage;
			}

			[JsonProperty]
			public string worker;


			public Page() : base() {
				Width = 50;
				Height = 50;
			}

		}

	}
}
