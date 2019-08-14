using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	internal class PresetObject
	{

		[JsonProperty]
		public string name;
		[JsonProperty]
		public string value;
		[JsonProperty]
		public string defaultValue;

	}
}
