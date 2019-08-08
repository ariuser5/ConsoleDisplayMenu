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

		private static string currentPage = "??";

		private static void Main(string[] args) {

			ConsoleMenu.Render(currentPage);
			Console.ReadKey();

		}
	}
}
