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
// YAT Version 2.1.1 Development
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

using MKY.Xml.Schema;
using MKY.Xml.Serialization;

using YAT.Domain;
using YAT.Domain.Utilities;

#endregion

namespace YAT.Log.Utilities
{
	/// <summary>
	/// Static utility class providing XML writer functionality for YAT.
	/// </summary>
	public static class XmlWriterHelperText
	{
		/// <returns>Returns the number of lines that could successfully be written to the file.</returns>
		public static int SaveLinesToFile(DisplayLineCollection displayLines, string filePath, bool addSchema)
		{
			List<XmlTransferTextLine> transferLines;
			int count = ConvertLines(displayLines, out transferLines);
			if (count > 0)
			{
				Type type = typeof(List<XmlTransferTextLine>);
				XmlSerializerEx.SerializeToFile(type, transferLines, filePath);

				if (addSchema)
					XmlSchemaEx.ToFile(type, Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));

				return (count);
			}
			else
			{
				return (0);
			}
		}

		/// <returns>Returns the number of lines that could successfully be converted.</returns>
		private static int ConvertLines(DisplayLineCollection displayLines, out List<XmlTransferTextLine> transferLines)
		{
			transferLines = new List<XmlTransferTextLine>(displayLines.Count); // Preset the required capacity to improve memory management.
			foreach (var dl in displayLines)
			{
				XmlTransferTextLine tl;
				if (ConvertLine(dl, out tl))
					transferLines.Add(tl);
				else
					break; // Immediately break, 'output' will only contain successfully converted lines.
			}

			return (transferLines.Count);
		}

		/// <returns>Returns <c>true</c> if the line could successfully be converted.</returns>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool ConvertLine(DisplayLine displayLine, out XmlTransferTextLine transferLine)
		{
			// Note that display elements don't contain all underlying typed information such as the
			// device string of the originating chunk. Since the XML schema is strongly-typed again,
			// such items need to be reconstructed. Not optimal, but kind of a trade-off between the
			// amount of date for displaying and log performance. Considered acceptable, XML logging
			// is probably rarly used.

			bool success = true;

			string textStr = "";
			string errorStr = "";

			DateTime timeStamp = DisplayElement.TimeStampDefault;
			string deviceStr = "";
			int length = 0;

			bool containsTx = false;
			bool containsRx = false;

			foreach (var de in displayLine)
			{
				// Try to cast to the more frequent Tx/Rx elements first, in order to improve speed.
				{
					var casted = (de as DisplayElement.TxData);
					if (casted != null)
					{
						textStr += casted.Text;
						containsTx = true;
						continue; // Immediately continue, makes no sense to also try other types.
					}
				}
				{
					var casted = (de as DisplayElement.TxControl);
					if (casted != null)
					{
						textStr += casted.Text;
						containsTx = true;
						continue; // Immediately continue, makes no sense to also try other types.
					}
				}
				{
					var casted = (de as DisplayElement.RxData);
					if (casted != null)
					{
						textStr += casted.Text;
						containsRx = true;
						continue; // Immediately continue, makes no sense to also try other types.
					}
				}
				{
					var casted = (de as DisplayElement.RxControl);
					if (casted != null)
					{
						textStr += casted.Text;
						containsRx = true;
						continue; // Immediately continue, makes no sense to also try other types.
					}
				}
				{
					var casted = (de as DisplayElement.IOControlInfo);
					if (casted != null)
					{
						textStr += casted.Text;
						continue; // Immediately continue, makes no sense to also try other types.
					}
				}
				{
					var casted = (de as DisplayElement.ErrorInfo);
					if (casted != null)
					{
						errorStr += casted.Text;
						continue; // Immediately continue, makes no sense to also try other types.
					}
				}

				// Then try to cast to the singleton elements:
				{
					var casted = (de as DisplayElement.TimeStampInfo);
					if (casted != null)
					{
						timeStamp = casted.TimeStamp;
						continue; // Immediately continue, makes no sense to also try other types.
					}
				}
				{
					var casted = (de as DisplayElement.DeviceInfo);
					if (casted != null)
					{
						deviceStr = casted.Text;
						continue; // Immediately continue, makes no sense to also try other types.
					}
				}
				{
					var casted = (de as DisplayElement.DataLength);
					if (casted != null)
					{
						length = casted.Length;
						continue; // Immediately continue, makes no sense to also try other types.
					}
				}

				// All white-space elements do not need to be processed.
				// 'TimeSpanInfo' is not used with 'XmlTransferTextLine'.
				// 'TimeDeltaInfo' is not used with 'XmlTransferTextLine'.
				// 'DirectionInfo' is handled below.
			}

			// In case the meta information elements are not contained (e.g. if not shown),
			// fall-back to the 'DisplayElemetColletion' property:
			if (timeStamp == DisplayElement.TimeStampDefault) { timeStamp = displayLine.TimeStamp; }
			if (length    == 0)                               { length    = displayLine.CharCount; } // 'CharCount' for 'Text' log.

			// Direction could also be retrieved from 'displayLine.Direction', but that would require
			// another loop over the whole collection, thus evaluating here base on the flags:
			Direction direction;
			if (containsTx && containsRx)
				direction = Direction.Bidir;
			else if (containsTx)
				direction = Direction.Tx;
			else if (containsRx)
				direction = Direction.Rx;
			else
				direction = Direction.None;

			transferLine = new XmlTransferTextLine(timeStamp, deviceStr, direction, textStr, errorStr, length);

			return (success);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
