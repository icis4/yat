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
// YAT 2.0 Gamma 2 Development Version 1.99.35
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
using System.Text;

using MKY.Xml.Serialization;

#endregion

namespace YAT.Model.Utilities
{
	/// <summary>
	/// Static utility class providing XML reader functionality for YAT.
	/// </summary>
	public static class XmlReaderHelper
	{
		/// <summary></summary>
		public static string[] LinesFromFile(string xmlFilePath)
		{
			object deserializedLines = null;
			deserializedLines = XmlSerializerEx.TolerantDeserializeFromFile(xmlFilePath, typeof(List<Domain.DisplayLine>));

			List<Domain.DisplayLine> lines = deserializedLines as List<Domain.DisplayLine>;
			if (lines != null)
			{
				StringBuilder sb;
				List<string> linesString = new List<string>();
				foreach (Domain.DisplayLine line in lines)
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
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
