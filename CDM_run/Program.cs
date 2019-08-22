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


			Page.Evaluate(pagePath);
			Console.WriteLine("End of program");
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();

		}
	}
}
