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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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
using System.Drawing;
using System.Windows.Forms;

using MKY;
using MKY.Drawing;

using YAT.Domain;
using YAT.Format.Settings;

#endregion

namespace YAT.View.Utilities
{
	/// <summary>
	/// Static utility class providing RTF writer functionality for YAT.
	/// </summary>
	public static class RtfWriterHelper
	{
		/// <remarks>
		/// For performance reasons, cache fonts.
		/// </remarks>
		private static Font staticTxDataFontCache;
		private static Font staticTxControlFontCache;
		private static Font staticRxDataFontCache;
		private static Font staticRxControlFontCache;
		private static Font staticTimeStampFontCache;
		private static Font staticTimeSpanFontCache;
		private static Font staticTimeDeltaFontCache;
		private static Font staticTimeDurationFontCache;
		private static Font staticDeviceFontCache;
		private static Font staticDirectionFontCache;
		private static Font staticLengthFontCache;
		private static Font staticIOControlFontCache;
		private static Font staticErrorFontCache;
		private static Font staticWhiteSpacesFontCache;

		/// <remarks>
		/// Pragmatic implementation of copying RTF to the clipboard. 'netrtfwriter' is only used for stream-based logging.
		/// </remarks>
		public static int CopyLinesToClipboard(DisplayLineCollection lines, FormatSettings settings)
		{
			var richTextProvider = CopyLinesToRichTextBox(lines, settings);
			richTextProvider.SelectAll();
			richTextProvider.Copy();

			return (lines.Count); // Assume success, an exception should otherwise be thrown above.
		}

		/// <remarks>
		/// Pragmatic implementation of saving RTF to a file. 'netrtfwriter' is only used for stream-based logging.
		/// </remarks>
		public static int SaveLinesToFile(DisplayLineCollection lines, string filePath, FormatSettings settings)
		{
			var richTextProvider = CopyLinesToRichTextBox(lines, settings);
			richTextProvider.SaveFile(filePath, RichTextBoxStreamType.RichText);

			return (lines.Count); // Assume success, an exception should otherwise be thrown above.
		}

		/// <summary></summary>
		internal static RichTextBox CopyLinesToRichTextBox(DisplayLineCollection lines, FormatSettings settings)
		{
			var richTextProvider = new RichTextBox();
			richTextProvider.Font      = settings.Font;
			richTextProvider.BackColor = settings.BackColor;

			foreach (var line in lines)
				AppendDisplayLine(richTextProvider, line, settings);

			return (richTextProvider);
		}

		private static void AppendDisplayLine(RichTextBox richTextProvider, DisplayLine line, FormatSettings settings)
		{
			AppendDisplayElements(richTextProvider, line, settings);
		}

		private static void AppendDisplayElements(RichTextBox richTextProvider, DisplayElementCollection elements, FormatSettings settings)
		{
			foreach (var de in elements)
				AppendDisplayElement(richTextProvider, de, settings);
		}

		private static void AppendDisplayElement(RichTextBox richTextProvider, DisplayElement element, FormatSettings settings)
		{
			Font font;
			Color color;
			SetDrawingObjects(element, settings, out font, out color);
			richTextProvider.SelectionFont  = font;
			richTextProvider.SelectionColor = color;

			// Handle line break according to current system:
			if (element is DisplayElement.LineBreak)
				richTextProvider.AppendText(Environment.NewLine);
			else if (!string.IsNullOrEmpty(element.Text))
				richTextProvider.AppendText(element.Text);
		}

		private static void SetDrawingObjects(DisplayElement element, FormatSettings settings, out Font font, out Color color)
		{
			string fontName = settings.Font.Name;
			float fontSize = settings.Font.Size;
			FontStyle fontStyle;

			if      (element is DisplayElement.TxData)
			{
				fontStyle = settings.TxDataFormat.FontStyle;
				color     = settings.TxDataFormat.Color;
				font      = DrawingEx.UpdateCacheIfAnyHasChanged(ref staticTxDataFontCache, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.TxControl)
			{
				fontStyle = settings.TxControlFormat.FontStyle;
				color     = settings.TxControlFormat.Color;
				font      = DrawingEx.UpdateCacheIfAnyHasChanged(ref staticTxControlFontCache, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.RxData)
			{
				fontStyle = settings.RxDataFormat.FontStyle;
				color     = settings.RxDataFormat.Color;
				font      = DrawingEx.UpdateCacheIfAnyHasChanged(ref staticRxDataFontCache, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.RxControl)
			{
				fontStyle = settings.RxControlFormat.FontStyle;
				color     = settings.RxControlFormat.Color;
				font      = DrawingEx.UpdateCacheIfAnyHasChanged(ref staticRxControlFontCache, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.TimeStampInfo)
			{
				fontStyle = settings.TimeStampFormat.FontStyle;
				color     = settings.TimeStampFormat.Color;
				font      = DrawingEx.UpdateCacheIfAnyHasChanged(ref staticTimeStampFontCache, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.TimeSpanInfo)
			{
				fontStyle = settings.TimeSpanFormat.FontStyle;
				color     = settings.TimeSpanFormat.Color;
				font      = DrawingEx.UpdateCacheIfAnyHasChanged(ref staticTimeSpanFontCache, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.TimeDeltaInfo)
			{
				fontStyle = settings.TimeDeltaFormat.FontStyle;
				color     = settings.TimeDeltaFormat.Color;
				font      = DrawingEx.UpdateCacheIfAnyHasChanged(ref staticTimeDeltaFontCache, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.TimeDurationInfo)
			{
				fontStyle = settings.TimeDurationFormat.FontStyle;
				color     = settings.TimeDurationFormat.Color;
				font      = DrawingEx.UpdateCacheIfAnyHasChanged(ref staticTimeDurationFontCache, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.DeviceInfo)
			{
				fontStyle = settings.DeviceFormat.FontStyle;
				color     = settings.DeviceFormat.Color;
				font      = DrawingEx.UpdateCacheIfAnyHasChanged(ref staticDeviceFontCache, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.DirectionInfo)
			{
				fontStyle = settings.DirectionFormat.FontStyle;
				color     = settings.DirectionFormat.Color;
				font      = DrawingEx.UpdateCacheIfAnyHasChanged(ref staticDirectionFontCache, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.ContentLength)
			{
				fontStyle = settings.LengthFormat.FontStyle;
				color     = settings.LengthFormat.Color;
				font      = DrawingEx.UpdateCacheIfAnyHasChanged(ref staticLengthFontCache, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.IOControlInfo)
			{
				fontStyle = settings.IOControlFormat.FontStyle;
				color     = settings.IOControlFormat.Color;
				font      = DrawingEx.UpdateCacheIfAnyHasChanged(ref staticIOControlFontCache, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.ErrorInfo)
			{
				fontStyle = settings.ErrorFormat.FontStyle;
				color     = settings.ErrorFormat.Color;
				font      = DrawingEx.UpdateCacheIfAnyHasChanged(ref staticErrorFontCache, fontName, fontSize, fontStyle);
			}
			else if ((element is DisplayElement.Nonentity)        ||
			         (element is DisplayElement.ContentSeparator) ||
			         (element is DisplayElement.InfoSeparator)    ||
			         (element is DisplayElement.LineStart)        ||
			         (element is DisplayElement.LineBreak))
			{
				fontStyle = settings.WhiteSpacesFormat.FontStyle;
				color     = settings.WhiteSpacesFormat.Color;
				font      = DrawingEx.UpdateCacheIfAnyHasChanged(ref staticWhiteSpacesFontCache, fontName, fontSize, fontStyle);
			}
			else
			{
				throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + element.GetType() + "' is a display element that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
