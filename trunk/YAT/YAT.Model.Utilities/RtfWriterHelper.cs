//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
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
using System.Drawing;
using System.Windows.Forms;

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
		private static Font directionFont;
		private static Font lineLengthFont;
		private static Font whiteSpacesFont;
		private static Font errorFont;

		/// <remarks>
		/// Pragmatic implementation of copying RTF to the clipboard. 'netrtfwriter' is only used for stream-based logging.
		/// </remarks>
		public static void LinesToClipboard(List<Domain.DisplayLine> lines, Settings.FormatSettings settings)
		{
			RichTextBox richTextProvider = LinesToRichTextBox(lines, settings);
			richTextProvider.SelectAll();
			richTextProvider.Copy();
		}

		/// <remarks>
		/// Pragmatic implementation of saving RTF to a file. 'netrtfwriter' is only used for stream-based logging.
		/// </remarks>
		public static void LinesToFile(List<Domain.DisplayLine> lines, string filePath, Settings.FormatSettings settings)
		{
			RichTextBox richTextProvider = LinesToRichTextBox(lines, settings);
			richTextProvider.SaveFile(filePath, RichTextBoxStreamType.RichText);
		}

		/// <summary></summary>
		internal static RichTextBox LinesToRichTextBox(List<Domain.DisplayLine> lines, Settings.FormatSettings settings)
		{
			RichTextBox richTextProvider = new RichTextBox();
			foreach (Domain.DisplayLine line in lines)
				AppendDisplayLine(richTextProvider, line, settings);

			return (richTextProvider);
		}

		private static void AppendDisplayLine(RichTextBox richTextProvider, Domain.DisplayLine line, Settings.FormatSettings settings)
		{
			AppendDisplayElements(richTextProvider, line, settings);
		}

		private static void AppendDisplayElements(RichTextBox richTextProvider, List<Domain.DisplayElement> elements, Settings.FormatSettings settings)
		{
			foreach (Domain.DisplayElement de in elements)
				AppendDisplayElement(richTextProvider, de, settings);
		}

		private static void AppendDisplayElement(RichTextBox richTextProvider, Domain.DisplayElement element, Settings.FormatSettings settings)
		{
			Font font;
			Color color;
			SetFontAndColor(element, settings, out font, out color);
			richTextProvider.SelectionFont  = font;
			richTextProvider.SelectionColor = color;

			// Handle line break according to current system:
			if (element is Domain.DisplayElement.LineBreak)
				richTextProvider.AppendText(Environment.NewLine);
			else
				richTextProvider.AppendText(element.Text);
		}

		private static void SetFontAndColor(Domain.DisplayElement element, Settings.FormatSettings settings, out Font font, out Color color)
		{
			string fontName = settings.Font.Name;
			float fontSize = settings.Font.Size;
			FontStyle fontStyle;

			if      (element is Domain.DisplayElement.TxData)
			{
				fontStyle = settings.TxDataFormat.FontStyle;
				color     = settings.TxDataFormat.Color;
				font      = SetFont(ref txDataFont, fontName, fontSize, fontStyle);
			}
			else if (element is Domain.DisplayElement.TxControl)
			{
				fontStyle = settings.TxControlFormat.FontStyle;
				color     = settings.TxControlFormat.Color;
				font      = SetFont(ref txControlFont, fontName, fontSize, fontStyle);
			}
			else if (element is Domain.DisplayElement.RxData)
			{
				fontStyle = settings.RxDataFormat.FontStyle;
				color     = settings.RxDataFormat.Color;
				font      = SetFont(ref rxDataFont, fontName, fontSize, fontStyle);
			}
			else if (element is Domain.DisplayElement.RxControl)
			{
				fontStyle = settings.RxControlFormat.FontStyle;
				color     = settings.RxControlFormat.Color;
				font      = SetFont(ref rxControlFont, fontName, fontSize, fontStyle);
			}
			else if (element is Domain.DisplayElement.DateInfo)
			{
				fontStyle = settings.DateFormat.FontStyle;
				color     = settings.DateFormat.Color;
				font      = SetFont(ref dateFont, fontName, fontSize, fontStyle);
			}
			else if (element is Domain.DisplayElement.TimeInfo)
			{
				fontStyle = settings.TimeFormat.FontStyle;
				color     = settings.TimeFormat.Color;
				font      = SetFont(ref timeFont, fontName, fontSize, fontStyle);
			}
			else if (element is Domain.DisplayElement.DirectionStamp)
			{
				fontStyle = settings.DirectionFormat.FontStyle;
				color     = settings.DirectionFormat.Color;
				font      = SetFont(ref directionFont, fontName, fontSize, fontStyle);
			}
			else if (element is Domain.DisplayElement.Length)
			{
				fontStyle = settings.LengthFormat.FontStyle;
				color     = settings.LengthFormat.Color;
				font      = SetFont(ref lineLengthFont, fontName, fontSize, fontStyle);
			}
			else if ((element is Domain.DisplayElement.LeftMargin) ||
			         (element is Domain.DisplayElement.Space) ||
			         (element is Domain.DisplayElement.RightMargin) ||
			         (element is Domain.DisplayElement.LineBreak))
			{
				fontStyle = settings.WhiteSpacesFormat.FontStyle;
				color     = settings.WhiteSpacesFormat.Color;
				font      = SetFont(ref whiteSpacesFont, fontName, fontSize, fontStyle);
			}
			else if (element is Domain.DisplayElement.IOError)
			{
				fontStyle = settings.ErrorFormat.FontStyle;
				color     = settings.ErrorFormat.Color;
				font      = SetFont(ref errorFont, fontName, fontSize, fontStyle);
			}
			else
			{
				throw (new NotImplementedException("Unknown DisplayElement!"));
			}
		}

		private static Font SetFont(ref Font cachedFont, string fontName, float fontSize, FontStyle fontStyle)
		{
			// Create the font.
			if (cachedFont == null)
			{
				cachedFont = new Font(fontName, fontSize, fontStyle);
			}
			else if ((cachedFont.Name != fontName) ||
					 (cachedFont.Size != fontSize) ||
					 (cachedFont.Style != fontStyle))
			{
				// The font has changed, dispose of the cached font and create a new one.
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
