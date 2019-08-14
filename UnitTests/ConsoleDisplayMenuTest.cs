using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace UnitTests
{
	[TestClass]
	public class ConsoleDisplayMenuTest
	{
		[TestMethod]
		public void JsonObject_ReadMetaComponents() {

			var myJson = "Label2: ${Sample\\Methods.vb>Methods.GetThree(#{preset0})} combined value here";
			var result = ConsoleDisplayMenu.JsonObject.ReadMetaComponents(myJson);

			Assert.IsTrue(result.Count() == 3);

		}
	}
}
