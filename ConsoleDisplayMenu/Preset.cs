using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	public class Preset : JsonObject
	{
		public enum PresetType
		{
			Value, Name, Property
		}

		public static implicit operator Preset(string json) {
			Preset newPreset = new Preset() {
				json = json,
				name = "Preset" + unnamedPresetCount,
				target = json.Split('{')[1].Split('}')[0]
			};
			unnamedPresetCount++;

			return newPreset;
		}



		[JsonProperty]
		public string target;
		[JsonProperty]
		public PresetType presetType;


		public Preset() : base(JsonObjectType.Preset) { }


		public void WriteValue(string value) {
			//todo
			//Write the value into PresetsFile
		}


		public string ReadValue() {
			//todo
			//Read the value from PresetsFile
			return null;
		}

		public override string ToString() {
			return "Pref_" + name;
		}

	}
}
