﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2'' Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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

using MKY.Xml.Serialization;

using YAT.Domain;

#endregion

namespace YAT.Model.Utilities
{
	/// <summary>
	/// Static utility class providing XML writer functionality for YAT.
	/// </summary>
	public static class XmlWriterHelperNeat
	{
		/// <returns>Returns the number of lines that could successfully be written to the file.</returns>
		public static int LinesToFile(List<DisplayLine> displayLines, string filePath, bool addSchema)
		{
			List<XmlTransferNeatLine> transferLines;
			int count = LinesFromDisplayToTransfer(displayLines, out transferLines);
			if (count > 0)
			{
				Type type = typeof(List<XmlTransferNeatLine>);
				XmlSerializerEx.SerializeToFile(filePath, type, transferLines);

				if (addSchema)
					XmlHelper.SchemaToFile(type, Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));

				return (count);
			}
			else
			{
				return (0);
			}
		}

		/// <returns>Returns the number of lines that could successfully be converted.</returns>
		private static int LinesFromDisplayToTransfer(List<DisplayLine> displayLines, out List<XmlTransferNeatLine> transferLines)
		{
			transferLines = new List<XmlTransferNeatLine>(displayLines.Count); // Preset the required capacity to improve memory management.
			foreach (DisplayLine dl in displayLines)
			{
				XmlTransferNeatLine tl;
				if (LineFromDisplayToTransfer(dl, out tl))
					transferLines.Add(tl);
				else
					break; // Immediately break, 'output' will only contain successfully converted lines.
			}

			return (transferLines.Count);
		}

		/// <returns>Returns <c>true</c> if the line could successfully be converted.</returns>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool LineFromDisplayToTransfer(DisplayLine displayLine, out XmlTransferNeatLine transferLine)
		{
			// Note that display elements are text-only and no longer contain the underlying typed
			// information such as the time-stamp of the origin. Since the XML schema is strongly-
			// typed again, the items need to be reconstructed. Not optimal, but simply a trade-off
			// between display and log performance. After all, XML logging is probably rarly used.

			bool success = true;

			string textStr      = "";
			string errorStr     = "";

			string dateStr      = "";
			string timeStr      = "";
			string portStr      = "";
			string directionStr = "";
			string lengthStr    = "";

			bool containsTx = false;
			bool containsRx = false;

			foreach (DisplayElement e in displayLine)
			{
				// Try to cast to the more frequent Tx/Rx elements first, in order to improve speed!
				{
					var casted = (e as DisplayElement.TxData);
					if (casted != null)
					{
						textStr += casted.Text;
						containsTx = true;
						continue; // Immediately continue, makes no sense to also try other types!
					}
				}
				{
					var casted = (e as DisplayElement.TxControl);
					if (casted != null)
					{
						textStr += casted.Text;
						containsTx = true;
						continue; // Immediately continue, makes no sense to also try other types!
					}
				}
				{
					var casted = (e as DisplayElement.RxData);
					if (casted != null)
					{
						textStr += casted.Text;
						containsRx = true;
						continue; // Immediately continue, makes no sense to also try other types!
					}
				}
				{
					var casted = (e as DisplayElement.RxControl);
					if (casted != null)
					{
						textStr += casted.Text;
						containsRx = true;
						continue; // Immediately continue, makes no sense to also try other types!
					}
				}
				{
					var casted = (e as DisplayElement.ErrorInfo);
					if (casted != null)
					{
						errorStr += casted.Text;
						continue; // Immediately continue, makes no sense to also try other types!
					}
				}

				// Then try to cast to the singleton elements:
				{
					var casted = (e as DisplayElement.DateInfo);
					if (casted != null)
					{
						dateStr = casted.Text;
						continue; // Immediately continue, makes no sense to also try other types!
					}
				}
				{
					var casted = (e as DisplayElement.TimeInfo);
					if (casted != null)
					{
						timeStr = casted.Text;
						continue; // Immediately continue, makes no sense to also try other types!
					}
				}
				{
					var casted = (e as DisplayElement.PortInfo);
					if (casted != null)
					{
						portStr = casted.Text;
						continue; // Immediately continue, makes no sense to also try other types!
					}
				}
				{
					var casted = (e as DisplayElement.DirectionInfo);
					if (casted != null)
					{
						directionStr = casted.Text;
						continue; // Immediately continue, makes no sense to also try other types!
					}
				}
				{
					var casted = (e as DisplayElement.DataLength);
					if (casted != null)
					{
						lengthStr = casted.Text;
						continue; // Immediately continue, makes no sense to also try other types!
					}
				}

				// All white-space elements do not need to be processed.
			}

			// Trim () from the singleton elements and try to create strongly-typed elements:

			DateTime timeStamp = DateTime.MinValue;
			Direction direction = Direction.None;
			int length = -1;

			if (!string.IsNullOrEmpty(dateStr) || !string.IsNullOrEmpty(timeStr))
			{
				if (!string.IsNullOrEmpty(dateStr)) {
					dateStr = dateStr.TrimStart('(');
					dateStr = dateStr.TrimEnd(')');
				}

				if (!string.IsNullOrEmpty(timeStr)) {
					timeStr = timeStr.TrimStart('(');
					timeStr = timeStr.TrimEnd(')');
				}

				string mergedStr = "";
				string mergedFmt = "";
				if (!string.IsNullOrEmpty(dateStr)) {
					mergedStr += dateStr;
					mergedFmt += DisplayElement.DateInfo.Format;
				}
				if (!string.IsNullOrEmpty(mergedStr)) {
					mergedStr += " ";
					mergedFmt += " ";
				}
				if (!string.IsNullOrEmpty(timeStr)) {
					mergedStr += timeStr;
					mergedFmt += DisplayElement.TimeInfo.Format;
				}

				if (!DateTime.TryParseExact(mergedStr, mergedFmt, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out timeStamp))
					success = false;
			}

			if (!string.IsNullOrEmpty(directionStr))
			{
				directionStr = directionStr.TrimStart('(');
				directionStr = directionStr.TrimEnd(')');

				if (!DirectionEx.TryParse(directionStr, out direction))
					success = false;
			}
			else
			{
				if (containsTx && containsRx)
					direction = Direction.Bidir;
				else if (containsTx)
					direction = Direction.Tx;
				else if (containsRx)
					direction = Direction.Rx;
				else
					direction = Direction.None;
			}

			if (!string.IsNullOrEmpty(lengthStr))
			{
				lengthStr = lengthStr.TrimStart('(');
				lengthStr = lengthStr.TrimEnd(')');

				if (!int.TryParse(lengthStr, out length))
					success = false;
			}

			transferLine = new XmlTransferNeatLine(timeStamp, portStr, direction, textStr, errorStr, length);

			return (success);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================