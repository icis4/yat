//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.18
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;
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
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static void SerializeToFile(string filePath, Type type, object obj)
		{
			using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
			{
				XmlWriterSettings xws = new XmlWriterSettings();
				xws.Indent = true;

				using (XmlWriter xw = XmlWriter.Create(sw, xws)) // Use dedicated XML writer to e.g. preserve whitespace!
				{
					XmlSerializer serializer = new XmlSerializer(type);
					serializer.Serialize(xw, obj);
				}
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
				using (XmlReader xr = XmlReader.Create(sr)) // Use dedicated XML reader to e.g. preserve whitespace!
				{
					XmlSerializer serializer = new XmlSerializer(type);
					settings = serializer.Deserialize(xr);
				}
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
				using (XmlReader xr = XmlReader.Create(sr)) // Use dedicated XML reader to e.g. preserve whitespace!
				{
					TolerantXmlSerializer serializer = new TolerantXmlSerializer(type);
					settings = serializer.Deserialize(xr);
				}
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
				using (XmlReader xr = XmlReader.Create(sr)) // Use dedicated XML reader to e.g. preserve whitespace!
				{
					AlternateTolerantXmlSerializer serializer = new AlternateTolerantXmlSerializer(type, alternateXmlElements);
					settings = serializer.Deserialize(xr);
				}
			}
			return (settings);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
