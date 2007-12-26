using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace YAT.Model.Utilities
{
	/// <summary>
	/// Static utility class providing XML reader functionality for YAT
	/// </summary>
	public static class XmlReader
	{
		/// <summary></summary>
		public static string[] LinesFromXmlFile(string xmlFilePath)
		{
			object lines = null;
			using (FileStream fs = new FileStream(xmlFilePath, FileMode.Open))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(List<List<Domain.DisplayElement>>));
				lines = serializer.Deserialize(fs);
			}

			StringBuilder sb;
			List<string> linesString = new List<string>();
			foreach (List<Domain.DisplayElement> line in (List<List<Domain.DisplayElement>>)lines)
			{
				sb = new StringBuilder();
				foreach (Domain.DisplayElement de in line)
				{
					if (de.IsDataElement)
						sb.Append(de.Text);
				}
				linesString.Add(sb.ToString());
			}
			return (linesString.ToArray());
		}
	}

	/// <summary>
	/// Static utility class providing XML writer functionality for YAT
	/// </summary>
	public class XmlWriter
	{
		/// <summary></summary>
		public static void LinesToXmlFile(List<List<Domain.DisplayElement>> lines, string xmlFilePath)
		{
			using (FileStream fs = new FileStream(xmlFilePath, FileMode.Create))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(List<List<Domain.DisplayElement>>));
				serializer.Serialize(fs, lines);
			}
		}
	}
}
