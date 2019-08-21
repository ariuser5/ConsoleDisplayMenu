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

		public static implicit operator Text(string meta) {
			return new Text() { value = meta };
		}




		[JsonProperty(Order = 2)]
		public string value;



		[JsonConstructor]
		public Text(
			string name = null,
			string value = null
			) : base(name, JsonObjectType.Text) {

			this.value = value;
		}


		public override object Evaluate() {
			return value;
		}

		public override string ToString() {
			return "Text_" + name;
		}

	}
}
