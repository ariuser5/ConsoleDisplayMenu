using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	public class Unit : JsonObject
	{

		public static implicit operator Unit(string meta) {
			return new Unit() { value = meta };
		}




		[JsonProperty(Order = 2)]
		public object value;



		[JsonConstructor]
		public Unit(
			string name = null,
			object value = null
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
