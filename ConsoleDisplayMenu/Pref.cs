using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{

	public enum PresetReferenceType
	{
		Value, Name, Property
	}

	public class Pref : JsonObject
	{

		public static implicit operator Pref(string meta) {
			return new Pref() {
				prefType = PresetReferenceType.Value,
				target = meta
			};
		}

		public static string PresetsSource = "Presets.json";


		public static void Reset(string target) {
			var json = File.ReadAllText(PresetsSource);
			var presets = JsonConvert.DeserializeObject<IEnumerable<Preset>>(json);
			var selection = presets.First(p => p.name == target);

			selection.value = selection.defaultValue;

			using(StreamWriter sw = new StreamWriter(PresetsSource)) {
				json = JsonConvert.SerializeObject(presets, Formatting.Indented);

				sw.Write(json);
			}
		}

		public static void Overwrite(string target, object value) {
			var json = File.ReadAllText(PresetsSource);
			var presets = JsonConvert.DeserializeObject<IEnumerable<Preset>>(json);
			var selection = presets.First(p => p.name == target);

			selection.value = value;

			using(StreamWriter sw = new StreamWriter(PresetsSource)) {
				json = JsonConvert.SerializeObject(presets, Formatting.Indented);

				sw.Write(json);
			}
		}

		public static object Read(string target, PresetReferenceType prefType = default(PresetReferenceType)) {
			var json = File.ReadAllText(PresetsSource);
			var presets = JsonConvert.DeserializeObject<IEnumerable<Preset>>(json);
			var selection = presets.First(p => p.name == target);

			switch(prefType) {
				case PresetReferenceType.Value: return selection.value;
				case PresetReferenceType.Name: return selection.name;
				case PresetReferenceType.Property: return string.Format("{0}: {1}", selection.name, selection.value);
				default: throw new NotImplementedException();
			}
		}






		[JsonProperty(Order = 2)]
		public string target;

		[JsonProperty(Order = 3)]
		public PresetReferenceType prefType;




		[JsonConstructor]
		public Pref(
			string name = null,
			string target = null,
			PresetReferenceType prefType = default(PresetReferenceType)
			) : base(name, JsonObjectType.Pref) {

			this.prefType = prefType;
			this.target = target;
		}



		public void WriteValue(string value) {
			Overwrite(target, value);
		}


		public override object Evaluate() {
			return Read(target, prefType);
		}

		public override string ToString() {
			return "Pref_" + name;
		}


		private class Preset
		{
			[JsonProperty(Order = 0)]
			public string name;
			[JsonProperty(Order = 1)]
			public object value;
			[JsonProperty(Order = 2)]
			public object defaultValue;
		}

	}

}
