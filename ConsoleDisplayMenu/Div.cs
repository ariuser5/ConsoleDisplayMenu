using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	public class Div : Container
	{

		public Div() : base(JsonObjectType.Div) {
			LeftMargin = 0;
			TopMargin = 0;
			RightMargin = 0;
			BottomMargin = 0;
		}

		public override string ToString() {
			return "Div_" + name;
		}

	}
}
