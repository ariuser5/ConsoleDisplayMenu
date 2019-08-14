using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	public class Text : JsonObject
	{

		public static implicit operator Text(string json) {
			Text newText = new Text() {
				json = json,
				name = "Text" + unnamedTextCount,
				value = json
			};
			unnamedTextCount++;

			return newText;
		}


		[JsonProperty]
		public string value;


		public Text() : base(JsonObjectType.Text) { }


		public override string ToString() {
			return "Text_" + name;
		}

	}
}
