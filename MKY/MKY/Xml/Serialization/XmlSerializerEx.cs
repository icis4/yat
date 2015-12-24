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

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MKY.Xml.Serialization
{
	/// <summary>
	/// XML serialization extensions.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class XmlSerializerEx
	{
		//------------------------------------------------------------------------------------------
		// Serialize
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static void SerializeToFile(string filePath, Type type, object obj)
		{
			using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
			{
				XmlSerializer serializer = new XmlSerializer(type);
				serializer.Serialize(sw, obj);
			}
		}

		//------------------------------------------------------------------------------------------
		// Deserialize
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static object DeserializeFromFile(string filePath, Type type)
		{
			object settings = null;
			using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8, true))
			{
				XmlSerializer serializer = new XmlSerializer(type);
				settings = serializer.Deserialize(sr);
			}
			return (settings);
		}

		/// <remarks>
		/// For details on tolerant serialization <see cref="TolerantXmlSerializer"/>.
		/// </remarks>
		public static object TolerantDeserializeFromFile(string filePath, Type type)
		{
			object settings = null;
			using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8, true))
			{
				TolerantXmlSerializer serializer = new TolerantXmlSerializer(type);
				settings = serializer.Deserialize(sr);
			}
			return (settings);
		}

		/// <remarks>
		/// For details on tolerant serialization <see cref="AlternateTolerantXmlSerializer"/>.
		/// </remarks>
		public static object AlternateTolerantDeserializeFromFile(string filePath, Type type, AlternateXmlElement[] alternateXmlElements)
		{
			object settings = null;
			using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8, true))
			{
				AlternateTolerantXmlSerializer serializer = new AlternateTolerantXmlSerializer(type, alternateXmlElements);
				settings = serializer.Deserialize(sr);
			}
			return (settings);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
