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
// YAT Version 2.4.0
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
using System.Globalization;
using System.IO;
using System.Text;

using MKY;
using MKY.IO;
using MKY.Xml.Serialization;

#endregion

namespace YAT.Domain.Utilities
{
	/// <summary>
	/// Static utility class providing XML reader functionality for YAT.
	/// </summary>
	public static class XmlReaderHelper
	{
		/// <exception cref="FileNotFoundException">
		/// A <see cref="FileNotFoundException"/> is thrown if the file could not be found.
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">
		/// A <see cref="UnauthorizedAccessException"/> is thrown if the file could not be accessed.
		/// </exception>
		/// <exception cref="InvalidDataException">
		/// A <see cref="InvalidDataException"/> is thrown if the file is empty or doesn't match the expected XML schema.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static int LinesFromFile(string filePath, out string[] lines)
		{
			if (!File.Exists(filePath))
				throw (new FileNotFoundException("File not found!"));

			if (!FileEx.IsReadable(filePath))
				throw (new UnauthorizedAccessException("File not readable!"));

			if (FileEx.Size(filePath) <= 0)
				throw (new InvalidDataException("File is empty!"));

			var sb = new StringBuilder();

			// First, try to deserialize from raw XML file:
			try
			{
				if (LinesFromRawFile(filePath, out lines) > 0)
					return (lines.Length);

				sb.AppendLine("File does not match the " + ApplicationEx.CommonName + " raw XML transfer schema.");
				sb.AppendLine();
			}
			catch (Exception exRaw)
			{
				sb.AppendLine(exRaw.Message);
				sb.AppendLine();
			}

			// If raw XML fails, try to deserialize from text XML file:
			try
			{
				if (LinesFromTextFile(filePath, out lines) > 0)
					return (lines.Length);

				sb.AppendLine("File does not match the " + ApplicationEx.CommonName + " text XML transfer schema.");
				sb.AppendLine();
			}
			catch (Exception exText)
			{
				sb.AppendLine(exText.Message);
				sb.AppendLine();
			}

			// If both fail, throw:
			sb.Append("Could not retrieve data from file!");
			throw (new InvalidDataException(sb.ToString()));
		}

		private static int LinesFromRawFile(string filePath, out string[] lines)
		{
			var type = typeof(List<XmlTransferRawLine>);
			var deserializedLines = XmlSerializerEx.TolerantDeserializeFromFile(type, filePath);
			var rawLines = (deserializedLines as List<XmlTransferRawLine>);
			if (rawLines != null)
			{
				var l = new List<string>(rawLines.Count); // Preset the required capacity to improve memory management.
				foreach (var rawLine in rawLines)
				{
					if (rawLine.Content != null)
					{
						var sb = new StringBuilder();
						sb.Append(@"\h(");
						sb.Append(ByteHelper.FormatHexString(rawLine.Content, showRadix: false));
						sb.Append(@")\!(NoEOL)"); // Ensure not to send EOL twice!
						l.Add(sb.ToString());
					}
				}
				lines = l.ToArray();
				return (lines.Length);
			}
			else
			{
				lines = null;
				return (-1);
			}
		}

		private static int LinesFromTextFile(string filePath, out string[] lines)
		{
			var type = typeof(List<XmlTransferTextLine>);
			var deserializedLines = XmlSerializerEx.TolerantDeserializeFromFile(type, filePath);
			var textLines = (deserializedLines as List<XmlTransferTextLine>);
			if (textLines != null)
			{
				var l = new List<string>(textLines.Count); // Preset the required capacity to improve memory management.
				foreach (var t in textLines)
				{
					if (t.Text != null)
						l.Add(t.Text);
				}
				lines = l.ToArray();
				return (lines.Length);
			}
			else
			{
				lines = null;
				return (-1);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
