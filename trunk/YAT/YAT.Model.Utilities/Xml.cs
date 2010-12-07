//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Model.Utilities
{
	/// <summary>
	/// Static utility class providing XML reader functionality for YAT.
	/// </summary>
	public static class XmlReader
	{
		/// <summary></summary>
		public static string[] LinesFromXmlFile(string xmlFilePath)
		{
			object lines = null;
			using (FileStream fs = new FileStream(xmlFilePath, FileMode.Open))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(List<Domain.DisplayLine>));
				lines = serializer.Deserialize(fs);
			}

			StringBuilder sb;
			List<string> linesString = new List<string>();
			foreach (Domain.DisplayLine line in (List<Domain.DisplayLine>)lines)
			{
				sb = new StringBuilder();
				foreach (Domain.DisplayElement de in line)
				{
					if (de.IsData)
						sb.Append(de.Text);
				}
				linesString.Add(sb.ToString());
			}
			return (linesString.ToArray());
		}
	}

	/// <summary>
	/// Static utility class providing XML writer functionality for YAT.
	/// </summary>
	public class XmlWriter
	{
		/// <summary></summary>
		public static void LinesToXmlFile(List<Domain.DisplayLine> lines, string xmlFilePath)
		{
			using (FileStream fs = new FileStream(xmlFilePath, FileMode.Create))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(List<Domain.DisplayLine>));
				serializer.Serialize(fs, lines);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
