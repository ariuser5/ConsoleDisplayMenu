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
			var json = System.IO.File.ReadAllText(source);
			var page = Deserialize(json);

			if(page.type == JsonObjectType.Page) return (Page) page;
			else throw new Exception(
				string.Format("Json in source file cannot be deserialized as {0} object", typeof(Page).ToString())
			);
		}


		[JsonProperty(Order = 4)]
		public Script script;



		[JsonConstructor]
		public Page(
			string name = null,
			LayoutType layout = default(LayoutType),
			Script script = null,
			int width = 0,
			int height = 0,
			int leftMargin = 0,
			int topMargin = 0,
			int rightMargin = 0,
			int bottomMargin = 0,
			IEnumerable<object> components = null
			) : base(name, JsonObjectType.Page, components) {

			this.layout = layout;
			this.script = script;
			this.Width = width;
			this.Height = height;
			this.LeftMargin = leftMargin;
			this.TopMargin = topMargin;
			this.RightMargin = rightMargin;
			this.BottomMargin = bottomMargin;
		}



		public override object Evaluate() {
			Console.Clear();
			Console.Write(base.Evaluate());

			return script?.Evaluate();
		}


		public override string ToString() {
			return "Page_" + name;
		}

	}
}
