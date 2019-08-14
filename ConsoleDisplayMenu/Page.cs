using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	public class Page : Container
	{

		public static Page Import(string source) {
			var jsonSrc = System.IO.File.ReadAllText(source);

			return (Page) new JsonObject(JsonObjectType.Page) { json = jsonSrc }.ConvertTo(JsonObjectType.Page);
		}


		[JsonProperty]
		public Script script;


		public Page() : base(JsonObjectType.Page) {
			Width = 50;
			Height = 50;
		}

		public override string ToString() {
			return "Page_" + name;
		}

	}
}
