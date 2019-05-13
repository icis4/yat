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
// MKY Version 1.0.26 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using MKY.Text;

#endregion

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

		/// <remarks>
		/// Using UTF-8 encoding with (Windows) or without (Unix, Linux,...) BOM.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static void SerializeToFile(string filePath, Type type, object obj)
		{
			SerializeToFile(filePath, type, obj, EncodingEx.EnvironmentRecommendedUTF8Encoding);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static void SerializeToFile(string filePath, Type type, object obj, Encoding encoding)
		{
			using (var sw = new StreamWriter(filePath, false, encoding))
			{
				var xws = new XmlWriterSettings();
				xws.Indent = true;

				using (var xw = XmlWriter.Create(sw, xws)) // Use dedicated XML writer to e.g. preserve whitespace in XML content!
				{
					var serializer = new XmlSerializer(type);
					serializer.Serialize(xw, obj);
				}
			}
		}

		//------------------------------------------------------------------------------------------
		// Deserialize
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Assuming UTF-8 with or without BOM.
		/// </remarks>
		public static object DeserializeFromFile(string filePath, Type type)
		{
			return (DeserializeFromFile(filePath, Encoding.UTF8, true, type));
		}

		/// <summary></summary>
		public static object DeserializeFromFile(string filePath, Encoding encoding, bool detectEncodingFromByteOrderMarks, Type type)
		{
			object settings = null;
			using (var sr = new StreamReader(filePath, encoding, detectEncodingFromByteOrderMarks))
			{
				using (var xr = XmlReader.Create(sr)) // Use dedicated XML reader to e.g. preserve whitespace in XML content!
				{
					var serializer = new XmlSerializer(type);
					settings = serializer.Deserialize(xr);
				}
			}
			return (settings);
		}

		/// <remarks>
		/// For details on tolerant serialization <see cref="TolerantXmlSerializer"/>.
		/// </remarks>
		/// <remarks>
		/// Assuming UTF-8 with or without BOM.
		/// </remarks>
		public static object TolerantDeserializeFromFile(string filePath, Type type)
		{
			return (TolerantDeserializeFromFile(filePath, Encoding.UTF8, true, type));
		}

		/// <remarks>
		/// For details on tolerant serialization <see cref="TolerantXmlSerializer"/>.
		/// </remarks>
		public static object TolerantDeserializeFromFile(string filePath, Encoding encoding, bool detectEncodingFromByteOrderMarks, Type type)
		{
			object settings = null;
			using (var sr = new StreamReader(filePath, encoding, detectEncodingFromByteOrderMarks))
			{
				using (var xr = XmlReader.Create(sr)) // Use dedicated XML reader to e.g. preserve whitespace in XML content!
				{
					var serializer = new TolerantXmlSerializer(type);
					settings = serializer.Deserialize(xr);
				}
			}
			return (settings);
		}

		/// <remarks>
		/// For details on tolerant serialization <see cref="AlternateTolerantXmlSerializer"/>.
		/// </remarks>
		/// <remarks>
		/// Assuming UTF-8 with or without BOM.
		/// </remarks>
		public static object AlternateTolerantDeserializeFromFile(string filePath, Type type, IEnumerable<AlternateXmlElement> alternateXmlElements)
		{
			return (AlternateTolerantDeserializeFromFile(filePath, Encoding.UTF8, true, type, alternateXmlElements));
		}

		/// <remarks>
		/// For details on tolerant serialization <see cref="AlternateTolerantXmlSerializer"/>.
		/// </remarks>
		public static object AlternateTolerantDeserializeFromFile(string filePath, Encoding encoding, bool detectEncodingFromByteOrderMarks, Type type, IEnumerable<AlternateXmlElement> alternateXmlElements)
		{
			object settings = null;
			using (var sr = new StreamReader(filePath, encoding, detectEncodingFromByteOrderMarks))
			{
				using (var xr = XmlReader.Create(sr)) // Use dedicated XML reader to e.g. preserve whitespace in XML content!
				{
					var serializer = new AlternateTolerantXmlSerializer(type, alternateXmlElements);
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
