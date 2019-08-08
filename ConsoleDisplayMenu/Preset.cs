using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	internal class Preset
	{

		[JsonProperty]
		public string tag;
		[JsonProperty]
		public string value;
		[JsonProperty]
		public string defaultValue;

	}
}
