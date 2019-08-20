using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	public class Div : Container
	{

		//public static implicit operator Div(string json) {
		//	Div newDiv = new Div() {
		//		inner = JsonToMeta(json),
		//		metaDefined = true,
		//		name = GetAvailableId(JsonObjectType.Pref),
		//		layout = LayoutType.Horizontal
		//	};
		//	newDiv.DeserializeComponents();

		//	return newDiv;
		//}




		[JsonConstructor]
		private Div(
			string name,
			LayoutType layout = default(LayoutType),
			int width = 0,
			int height = 0,
			int leftMargin = 0,
			int topMargin = 0,
			int rightMargin = 0,
			int bottomMargin = 0,
			IEnumerable<JsonObject> components = null
			) : base(name, JsonObjectType.Div) {

			this.layout = layout;
			this.Width = width;
			this.Height = height;
			this.LeftMargin = leftMargin;
			this.TopMargin = topMargin;
			this.RightMargin = rightMargin;
			this.BottomMargin = bottomMargin;

			instances.Add(this);
		}

		public Div() : this(null) { }



		public override string ToString() {
			return "Div_" + name;
		}

		~Div() => instances.Remove(this);

	}
}
