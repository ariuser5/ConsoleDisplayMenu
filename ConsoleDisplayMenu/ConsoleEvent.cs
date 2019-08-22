using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	public enum ConsoleEventRaiseType { OnConfirmation, Immediately }

	[JsonObject]
	public class ConsoleEvent
	{

		public static implicit operator ConsoleEvent(string meta) {
			var properties = meta.Split(':');
			var trigger = string.Empty;
			var raiseType = default(ConsoleEventRaiseType);
			var methodCall = string.Empty;

			if(properties.Count() == 2) {
				trigger = properties[0];
				methodCall = properties[1];

			} else if(properties.Count() == 3) {
				trigger = properties[0];
				methodCall = properties[1];
				raiseType = (ConsoleEventRaiseType) Enum.Parse(typeof(ConsoleEventRaiseType), properties[2]);

			} else throw new ArgumentException("Meta string could not be parsed");

			return new ConsoleEvent(null, trigger, raiseType, methodCall);
		}

		private static List<ConsoleEvent> instances = new List<ConsoleEvent>();



		[JsonProperty(Order = 0)]
		public string name;

		[JsonProperty(Order = 1)]
		public string trigger;

		[JsonProperty(Order = 2)]
		public ConsoleEventRaiseType raiseType;

		[JsonProperty(Order = 3)]
		public MethodCall methodCall;



		[JsonConstructor]
		public ConsoleEvent(
			string name = null,
			string trigger = null,
			ConsoleEventRaiseType raiseType = default(ConsoleEventRaiseType),
			object methodCall = null) {

			this.name = Tools.GetUniqueName(instances.Select(i => i.name), name, "ConsoleEvent");
			this.trigger = trigger;
			this.raiseType = raiseType;
			this.methodCall = methodCall.ToString();
		}

		internal virtual void Dump() => instances.Remove(this);


		public override string ToString() {
			return "ConsoleEvent_" + name;
		}
	}
}
