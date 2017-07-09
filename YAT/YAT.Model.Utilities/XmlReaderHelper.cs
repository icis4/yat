﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Delta Version 1.99.80
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

using MKY.IO;
using MKY.Xml.Serialization;

#endregion

namespace YAT.Model.Utilities
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
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

				sb.AppendLine("File does not match the YAT raw XML transfer schema.");
				sb.AppendLine();
			}
			catch (Exception exRaw)
			{
				sb.AppendLine(exRaw.Message);
				sb.AppendLine();
			}

			// If raw XML fails, try to deserialize from neat XML file:
			try
			{
				if (LinesFromNeatFile(filePath, out lines) > 0)
					return (lines.Length);

				sb.AppendLine("File does not match the YAT neat XML transfer schema.");
				sb.AppendLine();
			}
			catch (Exception exNeat)
			{
				sb.AppendLine(exNeat.Message);
				sb.AppendLine();
			}

			// If both fail, throw:
			sb.Append("Could not retrieve data from file!");
			throw (new InvalidDataException(sb.ToString()));
		}

		private static int LinesFromRawFile(string filePath, out string[] lines)
		{
			Type type = typeof(List<XmlTransferRawLine>);
			object deserializedLines = XmlSerializerEx.TolerantDeserializeFromFile(filePath, type);
			var rawLines = (deserializedLines as List<XmlTransferRawLine>);
			if (rawLines != null)
			{
				List<string> l = new List<string>(rawLines.Count); // Preset the required capacity to improve memory management.
				foreach (XmlTransferRawLine rawLine in rawLines)
				{
					if (rawLine.Data != null)
					{
						var sb = new StringBuilder();
						sb.Append(@"\h(");
						bool isFirst = true;
						foreach (byte b in rawLine.Data)
						{
							if (isFirst)
								isFirst = false;
							else
								sb.Append(" ");

							sb.Append(b.ToString("X2", CultureInfo.InvariantCulture));
						}
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

		private static int LinesFromNeatFile(string filePath, out string[] lines)
		{
			Type type = typeof(List<XmlTransferNeatLine>);
			object deserializedLines = XmlSerializerEx.TolerantDeserializeFromFile(filePath, type);
			var neatLines = (deserializedLines as List<XmlTransferNeatLine>);
			if (neatLines != null)
			{
				List<string> l = new List<string>(neatLines.Count); // Preset the required capacity to improve memory management.
				foreach (XmlTransferNeatLine neatLine in neatLines)
				{
					if (neatLine.Text != null)
						l.Add(neatLine.Text);
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
