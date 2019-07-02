using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LocalizationParser
{
	public class Parser
	{
		string fileName;
		public Parser (string file)
		{
			fileName = file;
		}
		List<string> Locals = new List<string>();

		public List<List<LocalValue>> Values;
		public void Parse()
		{
			
			//try{
			Values = new List<List<LocalValue>> ();
			var lines = File.ReadLines (fileName).ToArray();
			if (lines.Count () < 2)
				return;
			Locals = lines [1].Split (new string[]{","}, StringSplitOptions.None).ToList();
			for (int i = 0; i < Locals.Count; i++) {
				Values.Add(new List<LocalValue>());
			}
			for(int i = 2; i < lines.Length;i++)
			{
				//Get the files out

				Console.WriteLine(i);
				var values = ParseCsvRow(lines[i]);//.Split(new string[]{","}, StringSplitOptions.None);
				string constant = values[0];//.Replace("'","''");
				string comments = values[1];//.Replace("'","''");

			//	if(constant.ToLower().Contains("no thanks"))
			//	{
//					Regex csvSplit = new Regex("(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)", RegexOptions.Compiled);
//					
//					foreach (Match match in csvSplit.Matches(lines[i]))
//					{
//						//Console.WriteLine(match.Value);
//						//Console.WriteLine(match.Value.TrimStart(',').Replace("\"",""));
//					}
				
			//	}

				for(int l = 2; l < Values.Count();l++)
				{
					if(l >= values.Length)
						continue;
					var value = values[l];//.Replace("'","''");
					if(!string.IsNullOrEmpty(value) && value != "\"" )
						Values[l].Add( new LocalValue(){Constant = constant, Value = value, Comment = constant, Local = Locals[l]});
				}

			}
			writeFiles ();
			Console.WriteLine (Values);
			//}
			//catch(Exception ex)
			//{
			//	Console.WriteLine(ex);
			//}
			Console.WriteLine("Completed");
		}
		public static string[] ParseCsvRow(string r)
		{
			
			string[] c;
			string t;
			List<string> resp = new List<string>();
			bool cont = false;
			string cs = "";
			
			c = r.Split(new char[] { ',' }, StringSplitOptions.None);
			
			foreach (string y in c)
			{
				string x = y;
				if(x.Length == 0)
					Console.WriteLine("");

				
				if (cont)
				{
					// End of field
					if (x.EndsWith("\""))
					{
						cs += "," + x.Substring(0, x.Length - 1);
						resp.Add(cs);
						cs = "";
						cont = false;
						continue;
						
					}
					else
					{
						// Field still not ended
						cs += "," + x;
						continue;
					}
				}
				
				// Fully encapsulated with no comma within
				if (x.StartsWith("\"") && x.EndsWith("\"") && x.Length > 1)
				{
					if ((x.EndsWith("\"\"") && !x.EndsWith("\"\"\"")) && x != "\"\"")
					{
						cont = true;
						cs = x;
						continue;
					}
					
					resp.Add(x.Substring(1, x.Length - 2));
					continue;
				}
				
				// Start of encapsulation but comma has split it into at least next field
				if (x.StartsWith("\"") && !x.EndsWith("\""))
				{
					cont = true;
					cs += x.Substring(1);
					continue;
				}
				
				// Non encapsulated complete field
				resp.Add(x);
				
			}
			if(r.Contains("Now Playing"))
				Console.WriteLine("");
			return resp.ToArray();
			
		}

		void writeFiles()
		{
			for (int i = 0; i< Values.Count; i++) {
				writeList(Locals[i],Values[i]);
			}
		}
		void writeList(string local, List<LocalValue> values)
		{
			if(string.IsNullOrEmpty(local))
				return;
			if (Directory.Exists (local))
				Directory.Delete (local,true);
			Directory.CreateDirectory (local);

			StringBuilder sb = new StringBuilder ();
			foreach (var line in values) {
				sb.AppendLine(string.Format("/* {0} */",line.Comment));
				sb.AppendLine();
				sb.AppendLine(string.Format("\"{0}\" = \"{1}\";",line.Constant,line.Value));
				sb.AppendLine();
			}
			System.IO.TextWriter w = new System.IO.StreamWriter(Path.Combine(local,"Localizable.strings"));
			w.Write(sb.ToString());
			w.Flush();
			w.Close();
		}
	}
}

