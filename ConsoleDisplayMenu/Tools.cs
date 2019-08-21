using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	internal class Tools
	{

		public static string ReadBetween(string source, char begin, char end) {
			var builder = string.Empty;
			var depth = 0;

			foreach(char ch in source) {

				if(ch == begin) {
					depth++;
					if(depth == 1) continue;

				} else if(ch == end) {
					depth--;
					if(depth == 0) return builder;

				} else {
					if(depth > 0) builder += ch;
				}
			}

			return null;
		}

	}
}
