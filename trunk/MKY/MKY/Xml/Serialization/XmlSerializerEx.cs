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
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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

using MKY.Diagnostics;
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
		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/> and <see cref="object"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static void SerializeToFile(Type type, object obj, string filePath)
		{
			SerializeToFile(type, obj, filePath, EncodingEx.EnvironmentRecommendedUTF8Encoding);
		}

		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/> and <see cref="object"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static void SerializeToFile(Type type, object obj, string filePath, Encoding encoding)
		{
			using (var sw = new StreamWriter(filePath, false, encoding))
			{
				SerializeToWriter(type, obj, sw);
			}
		}

		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/> and <see cref="object"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Symmetricity with other methods.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static void SerializeToString(Type type, object obj, IFormatProvider formatProvider, ref StringBuilder sb)
		{
			using (var sw = new StringWriter(sb, formatProvider))
			{
				SerializeToWriter(type, obj, sw);
			}
		}

		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/> and <see cref="object"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static void SerializeToWriter(Type type, object obj, TextWriter output)
		{
			var xws = new XmlWriterSettings();
			xws.Indent = true;

			using (var xw = XmlWriter.Create(output, xws)) // Use dedicated XML writer to e.g. preserve whitespace in XML content!
			{
				var serializer = new XmlSerializer(type);
				serializer.Serialize(xw, obj);
			}
		}

		//------------------------------------------------------------------------------------------
		// Deserialize
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Assuming UTF-8 with or without BOM.
		/// </remarks>
		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		public static object DeserializeFromFile(Type type, string filePath)
		{
			return (DeserializeFromFile(type, filePath, Encoding.UTF8, true));
		}

		/// <remarks>
		/// Not implementable using default arguments because <see cref="Encoding.UTF8"/> is non-const.
		/// </remarks>
		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "What's wrong with 'non-const'?")]
		public static object DeserializeFromFile(Type type, string filePath, Encoding encoding, bool detectEncodingFromByteOrderMarks)
		{
			using (var sr = new StreamReader(filePath, encoding, detectEncodingFromByteOrderMarks))
			{
				return (DeserializeFromReader(type, sr));
			}
		}

		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		public static object DeserializeFromString(Type type, string s)
		{
			using (var sr = new StringReader(s))
			{
				return (DeserializeFromReader(type, sr));
			}
		}

		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		public static object DeserializeFromReader(Type type, TextReader input)
		{
			using (var xr = XmlReader.Create(input)) // Use dedicated XML reader to e.g. preserve whitespace in XML content!
			{
				var serializer = new XmlSerializer(type);
				return (serializer.Deserialize(xr));
			}
		}

		/// <remarks>
		/// For details on tolerant serialization <see cref="TolerantXmlSerializer"/>.
		/// </remarks>
		/// <remarks>
		/// Assuming UTF-8 with or without BOM.
		/// </remarks>
		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		public static object TolerantDeserializeFromFile(Type type, string filePath)
		{
			return (TolerantDeserializeFromFile(type, filePath, Encoding.UTF8, true));
		}

		/// <remarks>
		/// For details on tolerant serialization <see cref="TolerantXmlSerializer"/>.
		/// </remarks>
		/// <remarks>
		/// Not implementable using default arguments because <see cref="Encoding.UTF8"/> is non-const.
		/// </remarks>
		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		public static object TolerantDeserializeFromFile(Type type, string filePath, Encoding encoding, bool detectEncodingFromByteOrderMarks)
		{
			using (var sr = new StreamReader(filePath, encoding, detectEncodingFromByteOrderMarks))
			{
				return (TolerantDeserializeFromReader(type, sr));
			}
		}

		/// <remarks>
		/// For details on tolerant serialization <see cref="TolerantXmlSerializer"/>.
		/// </remarks>
		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		public static object TolerantDeserializeFromString(Type type, string s)
		{
			using (var sr = new StringReader(s))
			{
				return (TolerantDeserializeFromReader(type, sr));
			}
		}

		/// <remarks>
		/// For details on tolerant serialization <see cref="TolerantXmlSerializer"/>.
		/// </remarks>
		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		public static object TolerantDeserializeFromReader(Type type, TextReader input)
		{
			using (var xr = XmlReader.Create(input)) // Use dedicated XML reader to e.g. preserve whitespace in XML content!
			{
				var serializer = new TolerantXmlSerializer(type);
				return (serializer.Deserialize(xr));
			}
		}

		/// <remarks>
		/// For details on tolerant serialization <see cref="AlternateTolerantXmlSerializer"/>.
		/// </remarks>
		/// <remarks>
		/// Assuming UTF-8 with or without BOM.
		/// </remarks>
		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		public static object AlternateTolerantDeserializeFromFile(Type type, IEnumerable<AlternateXmlElement> alternateXmlElements, string filePath)
		{
			return (AlternateTolerantDeserializeFromFile(type, alternateXmlElements, filePath, Encoding.UTF8, true));
		}

		/// <remarks>
		/// For details on tolerant serialization <see cref="AlternateTolerantXmlSerializer"/>.
		/// </remarks>
		/// <remarks>
		/// Not implementable using default arguments because <see cref="Encoding.UTF8"/> is non-const.
		/// </remarks>
		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		public static object AlternateTolerantDeserializeFromFile(Type type, IEnumerable<AlternateXmlElement> alternateXmlElements, string filePath, Encoding encoding, bool detectEncodingFromByteOrderMarks)
		{
			using (var sr = new StreamReader(filePath, encoding, detectEncodingFromByteOrderMarks))
			{
				return (AlternateTolerantDeserializeFromReader(type, alternateXmlElements, sr));
			}
		}

		/// <remarks>
		/// For details on tolerant serialization <see cref="AlternateTolerantXmlSerializer"/>.
		/// </remarks>
		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		public static object AlternateTolerantDeserializeFromString(Type type, IEnumerable<AlternateXmlElement> alternateXmlElements, string s)
		{
			using (var sr = new StringReader(s))
			{
				return (AlternateTolerantDeserializeFromReader(type, alternateXmlElements, sr));
			}
		}

		/// <remarks>
		/// For details on tolerant serialization <see cref="AlternateTolerantXmlSerializer"/>.
		/// </remarks>
		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/> and <see cref="object"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		public static object AlternateTolerantDeserializeFromReader(Type type, IEnumerable<AlternateXmlElement> alternateXmlElements, TextReader input)
		{
			using (var xr = XmlReader.Create(input)) // Use dedicated XML reader to e.g. preserve whitespace in XML content!
			{
				var serializer = new AlternateTolerantXmlSerializer(type, alternateXmlElements);
				return (serializer.Deserialize(xr));
			}
		}

		/// <summary>
		/// This method loads settings from a reader. If the schema of the settings doesn't match,
		/// this method tries to load the settings using tolerant XML interpretation by making use
		/// of <see cref="TolerantXmlSerializer"/> or <see cref="AlternateTolerantXmlSerializer"/>.
		/// </summary>
		/// <remarks>
		/// Assuming UTF-8 with or without BOM.
		/// </remarks>
		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/> and <see cref="object"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		public static object DeserializeFromFileInsisting(Type type, IEnumerable<AlternateXmlElement> alternateXmlElements, string filePath)
		{
			return (DeserializeFromFileInsisting(type, alternateXmlElements, filePath, Encoding.UTF8, true));
		}

		/// <summary>
		/// This method loads settings from a reader. If the schema of the settings doesn't match,
		/// this method tries to load the settings using tolerant XML interpretation by making use
		/// of <see cref="TolerantXmlSerializer"/> or <see cref="AlternateTolerantXmlSerializer"/>.
		/// </summary>
		/// <remarks>
		/// Not implementable using default arguments because <see cref="Encoding.UTF8"/> is non-const.
		/// </remarks>
		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/> and <see cref="object"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "What's wrong with 'non-const'?")]
		public static object DeserializeFromFileInsisting(Type type, IEnumerable<AlternateXmlElement> alternateXmlElements, string filePath, Encoding encoding, bool detectEncodingFromByteOrderMarks)
		{
			using (var sr = new StreamReader(filePath, encoding, detectEncodingFromByteOrderMarks))
			{
				return (DeserializeFromReaderInsisting(type, alternateXmlElements, sr));
			}
		}

		/// <summary>
		/// This method loads settings from a reader. If the schema of the settings doesn't match,
		/// this method tries to load the settings using tolerant XML interpretation by making use
		/// of <see cref="TolerantXmlSerializer"/> or <see cref="AlternateTolerantXmlSerializer"/>.
		/// </summary>
		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/> and <see cref="object"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		public static object DeserializeFromStringInsisting(Type type, IEnumerable<AlternateXmlElement> alternateXmlElements, string s)
		{
			using (var sr = new StringReader(s))
			{
				return (DeserializeFromReaderInsisting(type, alternateXmlElements, sr));
			}
		}

		/// <summary>
		/// This method loads settings from a reader. If the schema of the settings doesn't match,
		/// this method tries to load the settings using tolerant XML interpretation by making use
		/// of <see cref="TolerantXmlSerializer"/> or <see cref="AlternateTolerantXmlSerializer"/>.
		/// </summary>
		/// <remarks>
		/// There are some issues with tolerant XML interpretation in case of lists. See YAT bug
		/// #3537940>#232 "Issues with TolerantXmlSerializer" for details. The following solutions
		/// could fix these issues:
		///  > Fix these issues in <see cref="TolerantXmlSerializer"/>
		///  > Implement a different variant of tolerant XML interpretation
		///     > Use of the default XML serialization to only process parts of the XML tree
		///  > Use of SerializationBinder (feature request #3392369)
		///  > Use of XSLT
		///     > Requires that every setting's schema is archived
		///     > Requires an incremental XSLT chain from every former schema
		///       (Alternatively, an immediate XSLT from every former schema)
		///
		/// Decision 2012-06: For the moment, the current solution is kept, rationale:
		///  > Creating an XSLT is time consuming for each release
		///  > Creating an XSLT fully or partly automatically requires an (expensive) tool
		///  > Current solution isn't perfect but good enough and easy to handle (no efforts)
		///  > Current solution also works for other software that makes use of MKY or YAT code
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		/// <remarks>
		/// Using non-generic generic signature using <see cref="Type"/> and <see cref="object"/>
		/// same as <see cref="XmlSerializer"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		public static object DeserializeFromReaderInsisting(Type type, IEnumerable<AlternateXmlElement> alternateXmlElements, TextReader input)
		{
			// First, always try standard deserialization:
			//  > This is the fastest way of deserialization
			//  > Tolerant deserialization has some severe issues (see comments above)
			//
			// However:
			// Standard deserialization might succeed even with an outdated schema! Then,
			// available alternate elements are not considered. But this issue is considered
			// less severe than the issues described above.
			try
			{
				return (DeserializeFromReader(type, input));
			}
			catch (Exception exStandard)
			{
				DebugEx.WriteException(type, exStandard, "Standard deserialization has failed, trying tolerantly.");

				if (alternateXmlElements == null)
				{
					// Try to open existing file with tolerant deserialization:
					try
					{
						return (TolerantDeserializeFromReader(type, input));
					}
					catch (Exception exTolerant)
					{
						DebugEx.WriteException(type, exTolerant, "Tolerant deserialization has failed, rethrowing!");
						throw; // Rethrow!
					}
				}
				else
				{
					// Try to open existing file with alternate-tolerant deserialization:
					try
					{
						return (AlternateTolerantDeserializeFromReader(type, alternateXmlElements, input));
					}
					catch (Exception exAlternateTolerant)
					{
						DebugEx.WriteException(type, exAlternateTolerant, "Alternate-tolerant deserialization has failed, rethrowing!");
						throw; // Rethrow!
					}
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
