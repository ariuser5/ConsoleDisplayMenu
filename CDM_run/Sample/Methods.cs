using System;


public class Methods
{
	public static object MainMenuScript() {
		Console.WriteLine("From Methods.MainMenuScript");

		ConsoleKeyInfo inKey;

		while(true) {
			inKey = Console.ReadKey();

			switch(inKey.KeyChar) {
				case '1': return "Sample\\GreetPage.json";
				case '2': return "Sample\\BlankPage.json";
				case '3': goto Exiting;
			}
		}

	Exiting:
		return null;
	}

	public static int GetThree() { return 3; }

	public static object ReturnArg(object arg) { return arg; }

}
