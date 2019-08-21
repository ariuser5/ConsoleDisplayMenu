﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	public class Pref : JsonObject, IReferenceValue
	{
		public enum PresetReferenceType
		{
			Value, Name, Property
		}

		public static implicit operator Pref(string json) {
			return new Pref() {
				presetType = PresetReferenceType.Value,
				target = json.Split('{')[1].Split('}')[0]
			};
		}



		[JsonProperty(Order = 2)]
		public string target;
		[JsonProperty(Order = 3)]
		public PresetReferenceType presetType;




		[JsonConstructor]
		private Pref(
			string name,
			string target = "",
			PresetReferenceType presetType = default(PresetReferenceType)
			) : base(name, JsonObjectType.Pref) {

			this.presetType = presetType;
			this.target = target;

			instances.Add(this);
		}

		public Pref() : this(null) { }




		public void WriteValue(string value) {
			var jsonString = File.ReadAllText(PresetsFile);
			var presets = JsonConvert.DeserializeObject<IEnumerable<Preset>>(jsonString);
			var selection = presets.First(preset => preset.name == target);

			selection.value = value;

			using(StreamWriter streamWriter = new StreamWriter(PresetsFile)) {
				var json = JsonConvert.SerializeObject(presets, Formatting.Indented);

				streamWriter.Write(json);
			}
		}


		public override object Evaluate() {

			var jsonString = File.ReadAllText(PresetsFile);
			var presets = JsonConvert.DeserializeObject<IEnumerable<Preset>>(jsonString);
			var selection = presets.First(preset => preset.name == target);

			switch(presetType) {
				case PresetReferenceType.Value: return selection.value;
				case PresetReferenceType.Name: return selection.name;
				case PresetReferenceType.Property: return string.Format("{0}: {1}", selection.name, selection.value);
				default: throw new NotImplementedException();
			}
		}

		public override string ToString() {
			return "Pref_" + name;
		}

		~Pref() => instances.Remove(this);


		private class Preset
		{
			[JsonProperty(Order = 0)]
			public string name;
			[JsonProperty(Order = 1)]
			public string value;
			[JsonProperty(Order = 2)]
			public string defaultValue;
		}

	}

}
