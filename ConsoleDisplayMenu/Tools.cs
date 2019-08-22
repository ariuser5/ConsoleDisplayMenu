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
			string accumulate = null;
			int depth = 0;

			foreach(char ch in source) {

				if(ch == begin) {
					depth++;
					if(depth == 1) {
						accumulate = string.Empty;
						continue;
					}

				} else if(ch == end) {

					if(depth > 0) {
						depth--;

						if(depth == 0) return accumulate;
					}

				}

				if(depth > 0) accumulate += ch;
			}

			return null;
		}


		public static IEnumerable<String> SplitOnce(string source, string delimiter) {
			string[] rets = { string.Empty, string.Empty };
			int index = 0;
			int i = 0;
			int j = 0;

			while(i < source.Length - delimiter.Length) {
				if(index == 0 && source[i] == delimiter[j]) {

					while(j < delimiter.Length) {
						if(source[i + j] != delimiter[j]) break;

						j++;
					}

					if(j == delimiter.Length) {
						index = 1;
						i = i + j;
					}

					j = 0;

				}

				rets[index] += source[i];
				i++;
			}

			return rets.Distinct();
		}

		public static IEnumerable<string> SplitOnce(string source, char delimiter) {
			return SplitOnce(source, delimiter);
		}

		public static string GetUniqueName(
			IEnumerable<string> refList,
			string preferedName = null,
			string defaultName = "Unnamed") {

			var current = string.Empty;
			var count = 1;

			if(string.IsNullOrEmpty(preferedName)) preferedName = defaultName;

			current = preferedName;

			while(true) {
				if(!refList.Contains(current)) return current;

				current = string.Format("{0}({1})", preferedName, count);
				count++;
			}

		}

	}
}
