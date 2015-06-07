//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 1' Version 1.99.33
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

#endregion

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
			object deserializedLines = null;
			using (FileStream fs = new FileStream(xmlFilePath, FileMode.Open))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(List<Domain.DisplayLine>));
				deserializedLines = serializer.Deserialize(fs);
			}

			List<Domain.DisplayLine> lines = deserializedLines as List<Domain.DisplayLine>;
			if (lines != null)
			{
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

			return (null);
		}
	}

	/// <summary>
	/// Static utility class providing XML writer functionality for YAT.
	/// </summary>
	public static class XmlWriter
	{
		/// <summary></summary>
		public static void LinesToXmlFile(List<Domain.DisplayLine> lines, string xmlFilePath)
		{
			using (StreamWriter sw = new StreamWriter(xmlFilePath, false, Encoding.UTF8))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(List<Domain.DisplayLine>));
				serializer.Serialize(sw, lines);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
