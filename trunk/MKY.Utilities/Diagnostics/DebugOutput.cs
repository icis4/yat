using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace MKY.Utilities.Diagnostics
{
	/// <summary></summary>
	static public class DebugOutput
	{
		/// <summary></summary>
		static public void WriteException(object obj, Exception ex)
		{
	
		#if (DEBUG)

			string line;
			StringReader sr;
			Debug.Write("Exception in ");
			Debug.WriteLine(obj.GetType().FullName);
			Debug.Indent();
			Debug.Write("Type: ");
			Debug.WriteLine(ex.GetType().ToString());
			Debug.WriteLine("Message:");
			Debug.Indent();
			sr = new StringReader(ex.Message);
			do
			{
				line = sr.ReadLine();
				if (line != null)
					Debug.WriteLine(line);
			}
			while (line != null);
			Debug.Unindent();
			Debug.Write("Source: ");
			Debug.WriteLine(ex.Source);
			Debug.WriteLine("Stack:");
			sr = new StringReader(ex.StackTrace);
			do
			{
				line = sr.ReadLine();
				if (line != null)
					Debug.WriteLine(line);
			}
			while (line != null);
			Debug.Unindent();

		#endif

		}
	}
}
