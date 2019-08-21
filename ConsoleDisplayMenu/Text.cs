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
			return new Text() { value = json };
		}




		[JsonProperty(Order = 2)]
		public string value;



		[JsonConstructor]
		private Text(
			string name,
			string value = ""
			) : base(name, JsonObjectType.Text) {

			this.value = value;
			instances.Add(this);
		}

		public Text() : this(null) {
			instances.Add(this);
		}


		public override object Evaluate() {
			return value;
		}

		public override string ToString() {
			return "Text_" + name;
		}

		~Text() => instances.Remove(this);

	}
}
