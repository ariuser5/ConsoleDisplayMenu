using ConsoleDisplayMenu;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace UnitTests
{
	[TestClass]
	public class ConsoleDisplayMenuTest
	{
		[TestMethod]
		public void JsonObject_ReadMetaComponents_Test() {

			var myMeta = "Label2: ${Sample\\Methods.vb>Methods.GetThree(#{preset0})} combined value here";
			//var result = JsonObject.Deserialize(myMeta) as Script;

			//Assert.IsTrue(result.args.Count() == 3);

		}

		[TestMethod]
		public void JsonObject_ReadJsonComponents_Test() {

			string myJson = @"{
	'name': 'MainMenuPage',
	'type': 'Page',
	'script': 'Sample\\Methods.vb>Methods.MainMenu()',
	'layout': 'Vertical',
	'components': [
		{
			'type': 'Div',
			'components' : []
		},
		{
			'type': 'Div',
			'components' : []
		}
	]
}";


			//var result = JsonObject.ReadPropertyArray(myJson, "components");

			//Assert.IsTrue(result.Count() == 2);

		}
	}
}
