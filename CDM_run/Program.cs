using ConsoleDisplayMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDM_run
{
	internal class Program
	{

		private static string pagePath = "Sample\\MainMenuPage1.json";


		private static void Main(string[] args) {

			var currPage = Page.Import(pagePath);

			JsonObject.Render(currPage);
			Console.ReadKey();

		}
	}
}
