using System;

namespace LocalizationParser
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//Console.WriteLine ("Hello World!");
			var parser = new Parser ("test.csv");
			parser.Parse ();
		}
	}
}
