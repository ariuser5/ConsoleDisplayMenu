using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	public class Div : RenderContainer
	{


		[JsonConstructor]
		public Div(
			string name = null,
			LayoutType layout = default(LayoutType),
			int width = 0,
			int height = 0,
			int leftMargin = 0,
			int topMargin = 0,
			int rightMargin = 0,
			int bottomMargin = 0,
			[JsonProperty("components")]IEnumerable<object> children = null
			) : base(name, JsonObjectType.Div, children) {

			this.layout = layout;
			this.Width = width;
			this.Height = height;
			this.LeftMargin = leftMargin;
			this.TopMargin = topMargin;
			this.RightMargin = rightMargin;
			this.BottomMargin = bottomMargin;
		}


		public override string ToString() {
			return "Div_" + name;
		}

	}
}
