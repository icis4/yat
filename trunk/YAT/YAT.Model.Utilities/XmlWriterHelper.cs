//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

using MKY.Xml;
using MKY.Xml.Serialization;

using YAT.Domain;

#endregion

namespace YAT.Model.Utilities
{
	/// <summary>
	/// Static utility class providing XML writer functionality for YAT.
	/// </summary>
	public static class XmlWriterHelper
	{
		/// <returns>Returns <c>true</c> if the line could succesfully be converted.</returns>
		private static bool LineFromDisplayToTransfer(DisplayLine displayLine, out XmlTransferNeatLine transferLine)
		{
			bool success = true;

			// Note that display elements are text-only and no longer contain the underlying typed
			// information such as the time-stamp of the origin. Since the XML schema is strongly-
			// typed again, the items need to be reconstructed. Not optimal, but simply a trade-off
			// between display and log performance. After all, XML logging is probably rarly used.

			string textStr      = "";
			string errorStr     = "";
			string dateStr      = "";
			string timeStr      = "";
			string directionStr = "";
			string lengthStr    = "";

			bool containsTx = false;
			bool containsRx = false;

			foreach (DisplayElement e in displayLine)
			{
				// Try to cast to the more frequent Tx/Rx elements first, in order to improve speed!
				{
					var casted = (e as DisplayElement.TxData);
					if (casted != null) {
						textStr += casted.Text;
						containsTx = true;
						continue; // Immediately continue, makes no sense to also try other types!
					}
				}
				{
					var casted = (e as DisplayElement.TxControl);
					if (casted != null) {
						textStr += casted.Text;
						containsTx = true;
						continue; // Immediately continue, makes no sense to also try other types!
					}
				}
				{
					var casted = (e as DisplayElement.RxData);
					if (casted != null) {
						textStr += casted.Text;
						containsRx = true;
						continue; // Immediately continue, makes no sense to also try other types!
					}
				}
				{
					var casted = (e as DisplayElement.RxControl);
					if (casted != null) {
						textStr += casted.Text;
						containsRx = true;
						continue; // Immediately continue, makes no sense to also try other types!
					}
				}
				{
					var casted = (e as DisplayElement.ErrorInfo);
					if (casted != null) {
						errorStr += casted.Text;
						continue; // Immediately continue, makes no sense to also try other types!
					}
				}
				// Then try to cast to the singleton elements:
				{
					var casted = (e as DisplayElement.DateInfo);
					if (casted != null)
						dateStr = casted.Text;
				}
				{
					var casted = (e as DisplayElement.TimeInfo);
					if (casted != null)
						timeStr = casted.Text;
				}
				{
					var casted = (e as DisplayElement.DirectionInfo);
					if (casted != null)
						directionStr = casted.Text;
				}
				{
					var casted = (e as DisplayElement.Length);
					if (casted != null)
						lengthStr = casted.Text;
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

			transferLine = new XmlTransferNeatLine(timeStamp, direction, length, textStr, errorStr);

			return (success);
		}

		/// <returns>Returns the number of lines that could succesfully be converted.</returns>
		private static int LinesFromDisplayToTransfer(List<DisplayLine> displayLines, out List<XmlTransferNeatLine> transferLines)
		{
			transferLines = new List<XmlTransferNeatLine>();
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

		/// <returns>Returns the number of lines that could succesfully be written to the file.</returns>
		public static int LinesToFileNeat(List<DisplayLine> displayLines, string filePath, bool addSchema)
		{
			List<XmlTransferNeatLine> transferLines;
			int count = LinesFromDisplayToTransfer(displayLines, out transferLines);
			if (count > 0)
			{
				Type type = typeof(List<XmlTransferNeatLine>);
				XmlSerializerEx.SerializeToFile(filePath, type, transferLines);

				if (addSchema)
					SchemaToFile(type, Path.GetFullPath(filePath), Path.GetFileNameWithoutExtension(filePath));

				return (count);
			}
			else
			{
				return (0);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public static void SchemaToFile(Type type, string path, string fileName)
		{
			XmlDocument document = XmlDocumentEx.CreateDefaultDocument(type);
			int n = document.Schemas.Schemas().Count;
			int i = 0;
			foreach (XmlSchema schema in document.Schemas.Schemas())
			{
				string filePath;
				if (n <= 1)
					filePath = path + fileName + ".xsd";
				else
					filePath = path + fileName + "-" + i + ".xsd";

				using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
				{
					schema.Write(sw);
				}

				i++;
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
