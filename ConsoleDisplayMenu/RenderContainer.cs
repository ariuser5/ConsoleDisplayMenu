using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	public enum LayoutType
	{
		Horizontal,
		Vertical
	}

	public abstract class RenderContainer : Container
	{

		[JsonProperty(Order = 2)]
		public LayoutType layout;


		[JsonProperty(Order = 3)]
		public int Width;
		[JsonProperty(Order = 4)]
		public int Height;

		[JsonProperty(Order = 5)]
		public int LeftMargin;
		[JsonProperty(Order = 6)]
		public int TopMargin;
		[JsonProperty(Order = 7)]
		public int RightMargin;
		[JsonProperty(Order = 8)]
		public int BottomMargin;



		[JsonIgnore]
		public IEnumerable<JsonObject> Children {
			get => children;
			set => BaseChildren = value;
		}



		protected RenderContainer(string name, JsonObjectType type, IEnumerable<object> children) : base(name, type, children) { }


		public override object Evaluate() {
			var sb = string.Empty;

			foreach(JsonObject child in children) {
				sb += child.Evaluate().ToString();

				switch(layout) {
					case LayoutType.Horizontal:
						sb += " ";
						break;

					case LayoutType.Vertical:
						sb += '\n';
						break;

					default: throw new NotImplementedException();
				}
			}

			return sb;
		}

	}
}
