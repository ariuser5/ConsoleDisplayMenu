using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplayMenu
{
	public class Page : RenderContainer
	{


		public static object Evaluate(string source) {
			instances.Clear();

			var json = System.IO.File.ReadAllText(source);
			var page = Deserialize(json);

			return page.Evaluate();
		}


		[JsonProperty(Order = 4)]
		public string script;

		[JsonProperty(Order = 5)]
		public readonly List<ConsoleEvent> events;



		[JsonConstructor]
		public Page(
			string name = null,
			LayoutType layout = default(LayoutType),
			string script = null,
			int width = 0,
			int height = 0,
			int leftMargin = 0,
			int topMargin = 0,
			int rightMargin = 0,
			int bottomMargin = 0,
			[JsonProperty("components")]IEnumerable<object> children = null,
			[JsonProperty("events")]List<ConsoleEvent> events = null
			) : base(name, JsonObjectType.Page, children) {

			this.layout = layout;
			this.script = script;
			this.events = events;
			this.Width = width;
			this.Height = height;
			this.LeftMargin = leftMargin;
			this.TopMargin = topMargin;
			this.RightMargin = rightMargin;
			this.BottomMargin = bottomMargin;
		}



		public void Render() {
			Console.Clear();
			Console.Write(base.Evaluate());
		}

		public override object Evaluate() {
			ConsoleEvent catchEvent;
			ConsoleKeyInfo inKey;
			var command = string.Empty;

			Render();

			while(true) {
				inKey = Console.ReadKey();
				command += inKey.KeyChar;

				catchEvent = MonitorInput(command);

				if(catchEvent != null) break;

				if(inKey.KeyChar == '\r') {
					command = string.Empty;
					Render();
				}
			}

			return catchEvent;
		}

		public ConsoleEvent MonitorInput(string command) {
			IEnumerable<ConsoleEvent> listeningEvents = events; ;

			if(command.Last() != '\r')
				listeningEvents = events.Where(e => e.raiseType == ConsoleEventRaiseType.Immediately);

			return listeningEvents.FirstOrDefault(e => e.trigger == command);
		}


		public override string ToString() {
			return "Page_" + name;
		}

	}
}
