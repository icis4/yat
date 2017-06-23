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
// YAT 2.0 Gamma 3 Development Version 1.99.53
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
using System.Drawing;
using System.Windows.Forms;

using MKY;

using YAT.Domain;

#endregion

namespace YAT.Model.Utilities
{
	/// <summary>
	/// Static utility class providing RTF writer functionality for YAT.
	/// </summary>
	public static class RtfWriterHelper
	{
		/// <remarks>
		/// For performance reasons, cache fonts.
		/// </remarks>
		private static Font txDataFont;
		private static Font txControlFont;
		private static Font rxDataFont;
		private static Font rxControlFont;
		private static Font dateFont;
		private static Font timeFont;
		private static Font portFont;
		private static Font directionFont;
		private static Font lengthFont;
		private static Font whiteSpacesFont;
		private static Font errorFont;

		/// <remarks>
		/// Pragmatic implementation of copying RTF to the clipboard. 'netrtfwriter' is only used for stream-based logging.
		/// </remarks>
		public static int LinesToClipboard(List<DisplayLine> lines, Settings.FormatSettings settings)
		{
			var richTextProvider = LinesToRichTextBox(lines, settings);
			richTextProvider.SelectAll();
			richTextProvider.Copy();

			return (lines.Count); // Assume success, an exception should otherwise be thrown above.
		}

		/// <remarks>
		/// Pragmatic implementation of saving RTF to a file. 'netrtfwriter' is only used for stream-based logging.
		/// </remarks>
		public static int LinesToFile(List<DisplayLine> lines, string filePath, Settings.FormatSettings settings)
		{
			var richTextProvider = LinesToRichTextBox(lines, settings);
			richTextProvider.SaveFile(filePath, RichTextBoxStreamType.RichText);

			return (lines.Count); // Assume success, an exception should otherwise be thrown above.
		}

		/// <summary></summary>
		internal static RichTextBox LinesToRichTextBox(List<DisplayLine> lines, Settings.FormatSettings settings)
		{
			var richTextProvider = new RichTextBox();
			richTextProvider.Font      = settings.Font;
			richTextProvider.BackColor = settings.BackColor;

			foreach (DisplayLine line in lines)
				AppendDisplayLine(richTextProvider, line, settings);

			return (richTextProvider);
		}

		private static void AppendDisplayLine(RichTextBox richTextProvider, DisplayLine line, Settings.FormatSettings settings)
		{
			AppendDisplayElements(richTextProvider, line, settings);
		}

		private static void AppendDisplayElements(RichTextBox richTextProvider, List<DisplayElement> elements, Settings.FormatSettings settings)
		{
			foreach (var de in elements)
				AppendDisplayElement(richTextProvider, de, settings);
		}

		private static void AppendDisplayElement(RichTextBox richTextProvider, DisplayElement element, Settings.FormatSettings settings)
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

		private static void SetDrawingObjects(DisplayElement element, Settings.FormatSettings settings, out Font font, out Color color)
		{
			string fontName = settings.Font.Name;
			float fontSize = settings.Font.Size;
			FontStyle fontStyle;

			if      (element is DisplayElement.TxData)
			{
				fontStyle = settings.TxDataFormat.FontStyle;
				color     = settings.TxDataFormat.Color;
				font      = CacheAndAssignIfChanged(ref txDataFont, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.TxControl)
			{
				fontStyle = settings.TxControlFormat.FontStyle;
				color     = settings.TxControlFormat.Color;
				font      = CacheAndAssignIfChanged(ref txControlFont, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.RxData)
			{
				fontStyle = settings.RxDataFormat.FontStyle;
				color     = settings.RxDataFormat.Color;
				font      = CacheAndAssignIfChanged(ref rxDataFont, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.RxControl)
			{
				fontStyle = settings.RxControlFormat.FontStyle;
				color     = settings.RxControlFormat.Color;
				font      = CacheAndAssignIfChanged(ref rxControlFont, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.DateInfo)
			{
				fontStyle = settings.DateFormat.FontStyle;
				color     = settings.DateFormat.Color;
				font      = CacheAndAssignIfChanged(ref dateFont, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.TimeInfo)
			{
				fontStyle = settings.TimeFormat.FontStyle;
				color     = settings.TimeFormat.Color;
				font      = CacheAndAssignIfChanged(ref timeFont, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.PortInfo)
			{
				fontStyle = settings.PortFormat.FontStyle;
				color     = settings.PortFormat.Color;
				font      = CacheAndAssignIfChanged(ref portFont, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.DirectionInfo)
			{
				fontStyle = settings.DirectionFormat.FontStyle;
				color     = settings.DirectionFormat.Color;
				font      = CacheAndAssignIfChanged(ref directionFont, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.DataLength)
			{
				fontStyle = settings.LengthFormat.FontStyle;
				color     = settings.LengthFormat.Color;
				font      = CacheAndAssignIfChanged(ref lengthFont, fontName, fontSize, fontStyle);
			}
			else if ((element is DisplayElement.Nonentity) ||
			         (element is DisplayElement.DataSpace) ||
			         (element is DisplayElement.InfoSeparator) ||
			         (element is DisplayElement.LineStart) ||
			         (element is DisplayElement.LineBreak))
			{
				fontStyle = settings.WhiteSpacesFormat.FontStyle;
				color     = settings.WhiteSpacesFormat.Color;
				font      = CacheAndAssignIfChanged(ref whiteSpacesFont, fontName, fontSize, fontStyle);
			}
			else if (element is DisplayElement.ErrorInfo)
			{
				fontStyle = settings.ErrorFormat.FontStyle;
				color     = settings.ErrorFormat.Color;
				font      = CacheAndAssignIfChanged(ref errorFont, fontName, fontSize, fontStyle);
			}
			else
			{
				throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + element.GetType() + "' is an invalid display element!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		private static Font CacheAndAssignIfChanged(ref Font cachedFont, string fontName, float fontSize, FontStyle fontStyle)
		{
			// Create the font:
			if (cachedFont == null)
			{
				cachedFont = new Font(fontName, fontSize, fontStyle);
			}
			else if ((cachedFont.Name != fontName) ||
					 (cachedFont.Size != fontSize) ||
					 (cachedFont.Style != fontStyle))
			{
				// The font has changed, dispose of the cached font and create a new one:
				cachedFont.Dispose();
				cachedFont = new Font(fontName, fontSize, fontStyle);
			}

			return (cachedFont);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
