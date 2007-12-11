using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace YAT.Model.Utilities
{
	public static class XmlReader
	{
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

	public class XmlWriter
	{
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
